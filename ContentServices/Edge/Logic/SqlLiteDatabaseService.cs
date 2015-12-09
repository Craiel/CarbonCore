namespace CarbonCore.ContentServices.Edge.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Edge.Contracts;
    using CarbonCore.ContentServices.Logic;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils;
    using CarbonCore.Utils.Contracts;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Database;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.IO;

    public class SqlLiteDatabaseService : ISqlLiteDatabaseService
    {
        private readonly ISqlLiteConnector connector;

        private readonly IDictionary<string, DatabaseEntryDescriptor> checkedTables;

        private readonly object threadLock = new object();

        private readonly Queue<SqlLiteDatabaseServiceAction> pendingActions;

        private readonly IDictionary<string, int> nextPrimaryKeyLookup;

        private CarbonFile activeFile;

        private Thread writerThread;

        private bool writerThreadActive;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SqlLiteDatabaseService(IFactory factory)
        {
            this.connector = factory.Resolve<ISqlLiteConnector>();

            this.checkedTables = new Dictionary<string, DatabaseEntryDescriptor>();

            this.pendingActions = new Queue<SqlLiteDatabaseServiceAction>();

            this.nextPrimaryKeyLookup = new Dictionary<string, int>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Initialize(CarbonFile file)
        {
            this.activeFile = file;
            this.connector.SetFile(this.activeFile);
            this.connector.Connect();

            // Create the async writing thread
            this.writerThreadActive = true;
            this.writerThread = new Thread(this.WriterThreadMain) { Name = "DatabaseServiceWriter", IsBackground = true };
            this.writerThread.Start();
        }

        public void Save<T>(ref T entry, bool async = false) where T : IDatabaseEntry
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            IList<SqlLiteDatabaseServiceAction> actions = this.DoSave(descriptor, entry);
            this.pendingActions.EnqueueRange(actions);
            if (async)
            {
                return;
            }

            // Wait until all actions are written
            this.ProcessPendingWrites(actions.Last());
        }

        public void Save<T>(IList<T> entries, bool async = false) where T : IDatabaseEntry
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            IList<SqlLiteDatabaseServiceAction> actions = new List<SqlLiteDatabaseServiceAction>();
            foreach (T entry in entries)
            {
                IList<SqlLiteDatabaseServiceAction> entryActions = this.DoSave(descriptor, entry);
                actions.AddRange(entryActions);
            }

            this.pendingActions.EnqueueRange(actions);

            if (async)
            {
                return;
            }

            this.ProcessPendingWrites(actions.Last());
        }

        public T Load<T>(object key, bool loadFull = false) where T : IDatabaseEntry
        {
            System.Diagnostics.Trace.Assert(key != null, "Single load must have key supplied");

            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            IList<IDatabaseEntry> results = this.DoLoad(descriptor, new List<object> { key }, loadFull);

            if (results == null || results.Count <= 0)
            {
                System.Diagnostics.Trace.TraceWarning("Load<T> returned no result for key {0} on {1}", key, descriptor.Type);
                return default(T);
            }

            return (T)results[0];
        }

        public IList<T> Load<T>(IList<object> keys = null, bool loadFull = false) where T : IDatabaseEntry
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            IList<IDatabaseEntry> results = this.DoLoad(descriptor, keys, loadFull);
            return results.Cast<T>().ToList();
        }
        
        public void Delete<T>(object key, bool async = false) where T : IDatabaseEntry
        {
            System.Diagnostics.Trace.Assert(key != null, "Delete needs values, Drop table if you want to clear!");

            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            SqlLiteDatabaseServiceAction action = this.DoDelete(descriptor, new List<object> { key });
            if (async)
            {
                return;
            }

            this.ProcessPendingWrites(action);
        }

        public void Delete<T>(IList<object> keys, bool async = false) where T : IDatabaseEntry
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            SqlLiteDatabaseServiceAction action = this.DoDelete(descriptor, keys);
            if (async)
            {
                return;
            }

            this.ProcessPendingWrites(action);
        }
        
        public int Count<T>(IList<object> keys = null) where T : IDatabaseEntry
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            return this.DoCount(descriptor, keys);
        }

        public void Drop<T>()
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();

            var statement = new SQLiteStatement(SqlStatementType.Drop);
            statement.Table(descriptor.TableName);

            var action = new SqlLiteDatabaseServiceAction(statement) { IgnoreFailure = true };
            this.pendingActions.Enqueue(action);
            this.ProcessPendingWrites(action);

            if (!action.Success)
            {
                throw new InvalidOperationException("Drop failed", action.Exception);
            }
        }

        public IList<string> GetTables()
        {
            return this.DoGetTables();
        }

        public void WaitForAsyncActions()
        {
            this.ProcessPendingWrites();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (this.writerThread != null)
            {
                this.writerThreadActive = false;
                this.ProcessPendingWrites();
                this.writerThread = null;
            }

            if (this.connector != null)
            {
                this.connector.Dispose();
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private IList<object[]> GetTableInfo(string tableName)
        {
            using (DbCommand command = this.connector.CreateCommand())
            {
                command.CommandText = string.Format(ContentServices.Constants.StatementTableInfo, tableName);
                IList<object[]> info = new List<object[]>();
                using (DbDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                {
                    while (reader.Read())
                    {
                        var data = new object[reader.FieldCount];
                        if (reader.GetValues(data) != reader.FieldCount)
                        {
                            throw new InvalidOperationException("GetValues returned unexpected field count");
                        }

                        info.Add(data);
                    }
                }

                return info;
            }
        }

        private IList<string> DoGetTables()
        {
            IList<string> results = new List<string>();
            
            var statement = new SQLiteStatement(SqlStatementType.Select);
            statement.Table("sqlite_master");
            statement.What("name");
            statement.WhereConstraint(new SqlStatementConstraint("type", "table"));

            using (DbCommand command = this.connector.CreateCommand(statement))
            {
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(reader["name"].ToString());
                    }
                }
            }

            return results;
        }

        private SqlLiteDatabaseServiceAction CreateTable(DatabaseEntryDescriptor descriptor)
        {
            var statement = new SQLiteStatement(SqlStatementType.Create) { DisableRowId = true };

            foreach (DatabaseEntryElementDescriptor element in descriptor.Elements)
            {
                string properties = DatabaseUtils.GetDatabaseTypeString(element.DatabaseType);
                if (element == descriptor.PrimaryKey)
                {
                    properties = string.Format("{0} {1} {2}", properties, ContentServices.Constants.StatementPrimaryKey, ContentServices.Constants.StatementNotNull);
                }

                statement.What(element.Name, properties);
            }

            statement.Table(descriptor.TableName);

            var action = new SqlLiteDatabaseServiceAction(statement);
            this.pendingActions.Enqueue(action);
            return action;
        }

        private void CheckTable(DatabaseEntryDescriptor descriptor)
        {
            string tableName = descriptor.TableName;
            if (this.checkedTables.ContainsKey(tableName))
            {
                return;
            }

            // Fetch the table's information
            IList<object[]> info = this.GetTableInfo(tableName);

            bool needRecreate = descriptor.Elements.Count != info.Count;
            IList<string> columnSegments = new List<string>();
            for (int i = 0; i < descriptor.Elements.Count; i++)
            {
                string tableType = DatabaseUtils.GetDatabaseTypeString(descriptor.Elements[i].PropertyType);
                string tableColumn = descriptor.Elements[i].Name;
                string tableTypeOptions = string.Empty;

                if (descriptor.PrimaryKey.Attribute.Equals(descriptor.Elements[i].Attribute))
                {
                    // If the column is part of the primary key we have to mark it as not null
                    if (!tableType.Contains(this.connector.NotNullStatement))
                    {
                        tableType = string.Concat(tableType, this.connector.NotNullStatement);
                    }

                    tableTypeOptions = ContentServices.Constants.StatementPrimaryKey;
                }

                // If we still assume to be consistent check more details
                if (!needRecreate)
                {
                    if (!tableType.Replace(this.connector.NotNullStatement, string.Empty).Equals(info[i][2] as string, StringComparison.OrdinalIgnoreCase))
                    {
                        needRecreate = true;
                    }

                    if (!tableColumn.Equals(info[i][1] as string, StringComparison.OrdinalIgnoreCase))
                    {
                        needRecreate = true;
                    }
                }

                columnSegments.Add(string.Format("{0} {1} {2}", tableColumn, tableType, tableTypeOptions));
            }
            
            if (needRecreate && info.Count > 0)
            {
                throw new InvalidOperationException("Table was inconsistent but re-creation is not yet supported!");
            }

            if (needRecreate)
            {
                System.Diagnostics.Trace.TraceWarning("Table {0} needs to be re-created", tableName);

                SqlLiteDatabaseServiceAction action = this.CreateTable(descriptor);
                this.ProcessPendingWrites(action);
            }

            this.checkedTables.Add(descriptor.TableName, descriptor);
        }

        private SQLiteStatement BuildInsertStatement(DatabaseEntryDescriptor descriptor, IDatabaseEntry entry)
        {
            System.Diagnostics.Trace.Assert(descriptor.PrimaryKey.Attribute.PrimaryKeyMode == PrimaryKeyMode.Autoincrement, "Primary key needs to be supplied unless AutoIncrement");

            // Insert if we have no primary key and auto increment
            var statement = new SQLiteStatement(SqlStatementType.Insert);
            foreach (DatabaseEntryElementDescriptor element in descriptor.Elements)
            {
                statement.With(element.Name, element.GetValue(entry));
            }

            statement.Table(descriptor.TableName);
            return statement;
        }

        private SQLiteStatement BuildUpdateStatement(DatabaseEntryDescriptor descriptor, IDatabaseEntry entry)
        {
            SQLiteStatement statement;
            object primaryKeyValue = descriptor.PrimaryKey.GetValue(entry);
            if (descriptor.PrimaryKey.Attribute.PrimaryKeyMode != PrimaryKeyMode.Autoincrement)
            {
                // Assigning so we have to check if the entry exists, this is not optimal!
                System.Diagnostics.Trace.TraceWarning("Performing looking for exist check on {0}", descriptor.Type);
                bool needUpdate = this.DoCount(descriptor, new List<object> { primaryKeyValue }) == 1;
                statement = needUpdate ? new SQLiteStatement(SqlStatementType.Update) : new SQLiteStatement(SqlStatementType.Insert);
            }
            else
            {
                statement = new SQLiteStatement(SqlStatementType.Update);
            }

            statement.WhereConstraint(new SqlStatementConstraint(descriptor.PrimaryKey.Name, primaryKeyValue));

            foreach (DatabaseEntryElementDescriptor element in descriptor.Elements)
            {
                statement.With(element.Name, element.GetValue(entry));
            }

            statement.Table(descriptor.TableName);
            return statement;
        }

        private IList<SqlLiteDatabaseServiceAction> DoSave(DatabaseEntryDescriptor descriptor, IDatabaseEntry entry)
        {
            IList<SqlLiteDatabaseServiceAction> results = new List<SqlLiteDatabaseServiceAction>();
            object primaryKeyValue = descriptor.PrimaryKey.GetValue(entry);
            bool hasPrimaryKey = primaryKeyValue != null;

            // Update the primary key if it's an insert statement into auto increment table
            if (descriptor.PrimaryKey.Attribute.PrimaryKeyMode != PrimaryKeyMode.Autoincrement && !hasPrimaryKey)
            {
                throw new InvalidDataException("Entry needs to have primary key assigned");
            }

            SQLiteStatement statement;
            if (!hasPrimaryKey)
            {
                primaryKeyValue = this.GetNextPrimaryKey(descriptor);

                // Assign the primary key and rebuild the statement with the new info
                descriptor.PrimaryKey.SetValue(entry, primaryKeyValue);
                statement = this.BuildInsertStatement(descriptor, entry);
            }
            else
            {
                statement = this.BuildUpdateStatement(descriptor, entry);
            }

            results.Add(new SqlLiteDatabaseServiceAction(statement));

            // Process the joined properties
            foreach (DatabaseEntryJoinedElementDescriptor joinedElement in descriptor.JoinedElements)
            {
                DatabaseEntryDescriptor joinedDescriptor = DatabaseEntryDescriptor.GetDescriptor(joinedElement.InternalType);
                System.Diagnostics.Trace.Assert(joinedDescriptor != null, "Joined entry must have a valid Database Descriptor");

                // Make sure to check the joined table, it may not be created yet
                this.CheckTable(joinedDescriptor);

                // Note: this is not really fast, if the joined instance is not supplied a lookup is performed to delete it
                var instance = (IDatabaseEntry)joinedElement.GetValue(entry);
                if (instance == null)
                {
                    var joinedDeleteStatement = new SQLiteStatement(SqlStatementType.Delete);
                    joinedDeleteStatement.Table(joinedDescriptor.TableName);
                    joinedDeleteStatement.WhereConstraint(new SqlStatementConstraint(joinedElement.ForeignKeyColumn, primaryKeyValue));
                    this.pendingActions.Enqueue(new SqlLiteDatabaseServiceAction(joinedDeleteStatement) { IgnoreFailure = true });
                }
                else
                {
                    joinedElement.ForeignKeyProperty.SetValue(instance, primaryKeyValue);
                    IList<SqlLiteDatabaseServiceAction> subResults = this.DoSave(joinedDescriptor, instance);
                    results.AddRange(subResults);
                }
            }

            return results;
        }

        private void BuildBaseWhereClause(SQLiteStatement target, DatabaseEntryDescriptor descriptor, IList<object> keys)
        {
            if (keys != null && keys.Count > 0)
            {
                if (keys.Count == 1)
                {
                    if (keys[0] != descriptor.PrimaryKey.InternalType.GetDefault())
                    {
                        target.WhereConstraint(new SqlStatementConstraint(descriptor.PrimaryKey.Name, keys[0]));
                    }
                }
                else
                {
                    System.Diagnostics.Trace.Assert(keys.Count < 1000, "More than a thousand entries might not be giving the proper result!");

                    target.WhereConstraint(new SqlStatementConstraint(descriptor.PrimaryKey.Name, keys));
                }
            }
        }
        
        private int DoCount(DatabaseEntryDescriptor descriptor, IList<object> keys)
        {
            var statement = new SQLiteStatement(SqlStatementType.Select);
            statement.Table(descriptor.TableName);
            this.BuildBaseWhereClause(statement, descriptor, keys);
            statement.What("count(*)");

            using (DbCommand command = this.connector.CreateCommand(statement))
            {
                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return 0;
                    }

                    reader.Read();
                    return Convert.ToInt32(reader[0]);
                }
            }
        }

        private IList<IDatabaseEntry> DoLoad(DatabaseEntryDescriptor descriptor, IList<object> keys, bool loadFull)
        {
            var statement = new SQLiteStatement(SqlStatementType.Select);
            statement.Table(descriptor.TableName);
            this.BuildBaseWhereClause(statement, descriptor, keys);

            foreach (DatabaseEntryElementDescriptor element in descriptor.Elements)
            {
                statement.What(element.Name);
            }

            IList<IDatabaseEntry> results = new List<IDatabaseEntry>();

            using (DbCommand command = this.connector.CreateCommand(statement))
            {
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var entry = (IDatabaseEntry)Activator.CreateInstance(descriptor.Type);
                        foreach (DatabaseEntryElementDescriptor element in descriptor.Elements)
                        {
                            object value = DatabaseUtils.GetInternalValue(
                                element.DatabaseType, reader[element.Name]);
                            element.SetValue(entry, value);
                        }

                        results.Add(entry);
                    }
                }
            }

            if (!loadFull)
            {
                return results;
            }

            // Make a list of the result set primary keys
            IDictionary<object, IDatabaseEntry> primaryKeyMap = new Dictionary<object, IDatabaseEntry>();
            foreach (IDatabaseEntry entry in results)
            {
                primaryKeyMap.Add(descriptor.PrimaryKey.GetValue(entry), entry);
            }

            // Make sure we don't attempt this with more than a thousand for now
            System.Diagnostics.Trace.Assert(primaryKeyMap.Count > 0, "Load full does not support more than a thousand entries right now, got " + results.Count);

            // Process the joined properties
            foreach (DatabaseEntryJoinedElementDescriptor joinedElement in descriptor.JoinedElements)
            {
                DatabaseEntryDescriptor joinedDescriptor = DatabaseEntryDescriptor.GetDescriptor(joinedElement.InternalType);
                System.Diagnostics.Trace.Assert(joinedDescriptor != null, "Joined entry must have a valid Database Descriptor");

                // Make sure to check the joined table, it may not be created yet
                this.CheckTable(joinedDescriptor);

                // Fetch the list of primary keys that match the results
                var lookupKeyClause = new KeyValuePair<string, IList<object>>(joinedElement.ForeignKeyColumn, primaryKeyMap.Keys.ToList());
                IEnumerable<object[]> joinedResults = this.DoLoadFields(
                        joinedDescriptor,
                        new List<KeyValuePair<string, IList<object>>> { lookupKeyClause },
                        joinedDescriptor.PrimaryKey.Name);

                IList<object> joinedPrimaryKeys = new List<object>();
                foreach (object[] result in joinedResults)
                {
                    joinedPrimaryKeys.Add(result[0]);
                }

                if (joinedPrimaryKeys.Count <= 0)
                {
                    continue;
                }

                // Fetch the actual instances
                IList<IDatabaseEntry> joinedEntries = this.DoLoad(joinedDescriptor, joinedPrimaryKeys, true);
                System.Diagnostics.Trace.Assert(joinedEntries.Count == joinedPrimaryKeys.Count);

                // Set the classes to the loaded instances
                foreach (IDatabaseEntry joinedEntry in joinedEntries)
                {
                    object primaryKey = joinedElement.ForeignKeyProperty.GetValue(joinedEntry);
                    System.Diagnostics.Trace.Assert(primaryKeyMap.ContainsKey(primaryKey));

                    joinedElement.SetValue(primaryKeyMap[primaryKey], joinedEntry);
                }
            }

            return results;
        }

        private SqlLiteDatabaseServiceAction DoDelete(DatabaseEntryDescriptor descriptor, IList<object> keys)
        {
            var statement = new SQLiteStatement(SqlStatementType.Delete);
            statement.Table(descriptor.TableName);
            this.BuildBaseWhereClause(statement, descriptor, keys);

            var action = new SqlLiteDatabaseServiceAction(statement);
            this.pendingActions.Enqueue(action);

            return action;
        }

        private IEnumerable<object[]> DoLoadFields(DatabaseEntryDescriptor descriptor, IEnumerable<KeyValuePair<string, IList<object>>> whereConstraints, params string[] fieldNames)
        {
            System.Diagnostics.Trace.Assert(fieldNames != null && fieldNames.Length > 0);

            var statement = new SQLiteStatement(SqlStatementType.Select);
            statement.Table(descriptor.TableName);
            if (whereConstraints != null)
            {
                foreach (KeyValuePair<string, IList<object>> pair in whereConstraints)
                {
                    if (pair.Value.Count > 1)
                    {
                        statement.WhereConstraint(new SqlStatementConstraint(pair.Key, pair.Value));
                    }
                    else
                    {
                        statement.WhereConstraint(new SqlStatementConstraint(pair.Key, pair.Value[0]));
                    }
                }
            }

            foreach (string name in fieldNames)
            {
                statement.What(name);
            }

            IList<object[]> results = new List<object[]>();

            using (DbCommand command = this.connector.CreateCommand(statement))
            {
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var values = new object[fieldNames.Length];
                        for (int i = 0; i < fieldNames.Length; i++)
                        {
                            values[i] = reader[i];
                        }

                        results.Add(values);
                    }
                }
            }

            return results;
        }

        private int GetNextPrimaryKey(DatabaseEntryDescriptor descriptor)
        {
            lock (this.threadLock)
            {
                if (!this.nextPrimaryKeyLookup.ContainsKey(descriptor.TableName))
                {
                    var statement = new SQLiteStatement(SqlStatementType.Select);
                    statement.Table(descriptor.TableName);
                    statement.What(string.Format("MAX({0})", descriptor.PrimaryKey.Name));
                    try
                    {
                        using (DbCommand command = this.connector.CreateCommand(statement))
                        {
                            using (DbDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader[0] == DBNull.Value)
                                    {
                                        this.nextPrimaryKeyLookup[descriptor.TableName] = -1;
                                    }
                                    else
                                    {
                                        this.nextPrimaryKeyLookup[descriptor.TableName] = Convert.ToInt32(reader[0]);
                                    }

                                    break;
                                }
                            }
                        }
                    }
                    catch (SQLiteException)
                    {
                        // We assume the table does not exist in this case
                        this.nextPrimaryKeyLookup[descriptor.TableName] = -1;
                    }
                }

                this.nextPrimaryKeyLookup[descriptor.TableName]++;
                System.Diagnostics.Trace.TraceInformation(
                    "Handing out primary key {0} for {1}",
                    this.nextPrimaryKeyLookup[descriptor.TableName],
                    descriptor.TableName);

                return this.nextPrimaryKeyLookup[descriptor.TableName];
            }
        }

        private void ProcessPendingWrites(SqlLiteDatabaseServiceAction targetAction = null)
        {
            if (targetAction == null)
            {
                while (this.pendingActions.Count > 0)
                {
                    Thread.Sleep(10);
                }
            }
            else
            {
                while (this.pendingActions.Contains(targetAction))
                {
                    Thread.Sleep(10);
                }
            }
        }

        private void WriterThreadMain()
        {
            while (this.writerThreadActive || this.pendingActions.Count > 0)
            {
                if (this.pendingActions.Count <= 0)
                {
                    Thread.Sleep(2);
                    continue;
                }

                IList<SqlLiteDatabaseServiceAction> actionBulk = new List<SqlLiteDatabaseServiceAction>();
                var currentPending = new Queue<SqlLiteDatabaseServiceAction>(this.pendingActions);
                while (currentPending.Count > 0)
                {
                    SqlLiteDatabaseServiceAction action = currentPending.Dequeue();
                    if (!action.CanBatch || 
                        (action.Statement.Type != SqlStatementType.Insert
                        && action.Statement.Type != SqlStatementType.Update
                        && action.Statement.Type != SqlStatementType.Delete))
                    {
                        if (actionBulk.Count <= 0)
                        {
                            actionBulk.Add(action);
                        }

                        break;
                    }

                    actionBulk.Add(action);
                }

                if (actionBulk.Count == 1)
                {
                    this.CommitAction(actionBulk[0]);
                }
                else
                {
                    // Commit the bulk
                    this.CommitActionBatch(actionBulk);
                }

                // Dequeue the actions now
                for (int i = 0; i < actionBulk.Count; i++)
                {
                    actionBulk[i].WasExecuted = true;
                    this.pendingActions.Dequeue();
                }
            }
        }

        private void CommitAction(SqlLiteDatabaseServiceAction action)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("Executing: {0}", action.Statement);
                using (var profileRegion = new ProfileRegion("DBWrite") { Discard = true })
                {
                    using (DbCommand command = this.connector.CreateCommand(action.Statement))
                    {
                        action.Result = command.ExecuteNonQuery();
                    }

                    action.ExecutionTime = profileRegion.ElapsedMilliseconds;
                    action.Success = true;
                }
            }
            catch (SQLiteException e)
            {
                action.Exception = e;
                action.Success = false;
                if (!action.IgnoreFailure)
                {
                    throw;
                }
            }
        }

        private void CommitActionBatch(IList<SqlLiteDatabaseServiceAction> actions)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Executing batch: {0}", actions.Count);
                using (var profileRegion = new ProfileRegion("DBWrite") { Discard = true })
                {
                    IList<ISqlStatement> statements = new List<ISqlStatement>();
                    foreach (SqlLiteDatabaseServiceAction action in actions)
                    {
                        statements.Add(action.Statement);
                    }

                    using (DbCommand command = this.connector.CreateCommand(statements))
                    {
                        System.Diagnostics.Debug.WriteLine("  - {0}", command.CommandText);
                        int result = command.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine("  = {0}", result);
                    }

                    foreach (SqlLiteDatabaseServiceAction action in actions)
                    {
                        // We divide the execution time accross the bulk, not 100% accurate but good enough for batching
                        action.ExecutionTime = profileRegion.ElapsedMilliseconds / actions.Count;
                        action.Success = true;
                    }
                }
            }
            catch (SQLiteException e)
            {
                using (DbCommand command = this.connector.CreateCommand())
                {
                    command.CommandText = ContentServices.Constants.StatementRollback;
                    command.ExecuteNonQuery();
                }

                foreach (SqlLiteDatabaseServiceAction action in actions)
                {
                    action.Exception = e;
                    action.Success = false;
                    if (!action.IgnoreFailure)
                    {
                        throw;
                    }

                    break;
                }
            }
        }
    }
}
