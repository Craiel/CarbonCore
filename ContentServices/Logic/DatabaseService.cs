namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Linq;
    using System.Reflection;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Database;
    using CarbonCore.Utils.IO;

    public class DatabaseService : IDatabaseService
    {
        private const string StatementTableInfo = "PRAGMA table_info({0})";
        private const string StatementPrimaryKey = "PRIMARY KEY";
        private const string StatementAutoIncrement = "AUTOINCREMENT";

        private readonly ISqlLiteConnector connector;

        private readonly IDictionary<string, DatabaseEntryDescriptor> checkedTables; 

        private CarbonFile activeFile;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseService(IFactory factory)
        {
            this.connector = factory.Resolve<ISqlLiteConnector>();

            this.checkedTables = new Dictionary<string, DatabaseEntryDescriptor>();
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
        }

        bool IDatabaseService.Save<T>(ref T entry)
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            using (var transaction = this.connector.BeginTransaction())
            {
                try
                {
                    if (this.DoSave(descriptor, entry))
                    {
                        transaction.Commit();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.TraceError("Save failed of {0}, {1}", entry, e);
                }

                transaction.Rollback();
            }

            return false;
        }

        public bool Save<T>(IList<T> entries) where T : IDatabaseEntry
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            using (var transaction = this.connector.BeginTransaction())
            {
                foreach (T entry in entries)
                {
                    try
                    {
                        if (this.DoSave(descriptor, entry))
                        {
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.TraceError("Save failed of {0}/{1}, {2}", entry, entries.Count, e);
                    }

                    transaction.Rollback();
                    return false;
                }

                transaction.Commit();
                return true;
            }
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
        
        public bool Delete<T>(IList<object> keys) where T : IDatabaseEntry
        {
            System.Diagnostics.Trace.Assert(keys != null && keys.Count > 0, "Delete needs values, Drop table if you want to clear!");

            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            using (var transaction = this.connector.BeginTransaction())
            {
                try
                {
                    if (this.DoDelete(descriptor, keys))
                    {
                        transaction.Commit();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.TraceError("Delete failed: {0}", e);
                }

                transaction.Rollback();
                return false;
            }
        }
        
        public int Count<T>(IList<object> keys = null) where T : IDatabaseEntry
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            return this.DoCount(descriptor, keys);
        }

        public bool Drop<T>()
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            return this.DropTable(descriptor);
        }

        public IList<string> GetTables()
        {
            return this.DoGetTables();
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
                command.CommandText = string.Format(StatementTableInfo, tableName);
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
            statement.WhereConstraint("type", "table");

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

        private bool DropTable(DatabaseEntryDescriptor descriptor)
        {
            var statement = new SQLiteStatement(SqlStatementType.Drop);
            statement.Table(descriptor.TableName);

            using (DbCommand command = this.connector.CreateCommand(statement))
            {
                try
                {
                    int result = command.ExecuteNonQuery();
                    System.Diagnostics.Trace.TraceWarning(
                        "Dropped table {0}, lost {1} entries", descriptor.TableName, result);
                }
                catch (SQLiteException e)
                {
                    System.Diagnostics.Trace.TraceWarning("Could not drop table {0}, {1}", descriptor.TableName, e);
                    return false;
                }
            }

            return true;
        }

        private void CreateTable(DatabaseEntryDescriptor descriptor)
        {
            var statement = new SQLiteStatement(SqlStatementType.Create);
            foreach (DatabaseEntryElementDescriptor element in descriptor.Elements)
            {
                string properties = DatabaseUtils.GetDatabaseTypeString(element.DatabaseType);
                if (element == descriptor.PrimaryKey)
                {
                    switch (descriptor.PrimaryKey.Attribute.PrimaryKeyMode)
                    {
                        case PrimaryKeyMode.Autoincrement:
                            {
                                properties = string.Format("{0} {1} {2}", properties, StatementPrimaryKey, StatementAutoIncrement);
                                break;
                            }

                        default:
                            {
                                properties = string.Format("{0} {1}", properties, StatementPrimaryKey);
                                break;
                            }
                    }
                }

                statement.What(element.Name, properties);
            }

            statement.Table(descriptor.TableName);

            using (DbCommand command = this.connector.CreateCommand(statement))
            {
                command.ExecuteNonQuery();
            }
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
                string tableType = DatabaseUtils.GetDatabaseTypeString(descriptor.Elements[i].Property.PropertyType);
                string tableColumn = descriptor.Elements[i].Name;
                string tableTypeOptions = string.Empty;

                if (descriptor.PrimaryKey.Attribute.Equals(descriptor.Elements[i].Attribute))
                {
                    // If the column is part of the primary key we have to mark it as not null
                    if (!tableType.Contains(this.connector.NotNullStatement))
                    {
                        tableType = string.Concat(tableType, this.connector.NotNullStatement);
                    }

                    tableTypeOptions = StatementPrimaryKey;
                    switch (descriptor.PrimaryKey.Attribute.PrimaryKeyMode)
                    {
                        case PrimaryKeyMode.Autoincrement:
                            {
                                tableTypeOptions = string.Format("{0} {1}", tableTypeOptions, StatementAutoIncrement);
                                break;
                            }
                    }
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

                this.CreateTable(descriptor);
            }

            this.checkedTables.Add(descriptor.TableName, descriptor);
        }

        private SQLiteStatement BuildInsertUpdateStatement(DatabaseEntryDescriptor descriptor, IDatabaseEntry entry)
        {
            SQLiteStatement statement;
            if (descriptor.PrimaryKey.Property.GetValue(entry) == null)
            {
                System.Diagnostics.Trace.Assert(descriptor.PrimaryKey.Attribute.PrimaryKeyMode == PrimaryKeyMode.Autoincrement, "Primary key needs to be supplied unless AutoIncrement");

                // Insert if we have no primary key and auto increment
                statement = new SQLiteStatement(SqlStatementType.Insert);
            }
            else
            {
                // We have a primary key, now check if we are auto incrementing or assigning
                object primaryKeyValue = descriptor.PrimaryKey.Property.GetValue(entry);
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

                statement.WhereConstraint(descriptor.PrimaryKey.Name, primaryKeyValue);
            }

            foreach (DatabaseEntryElementDescriptor element in descriptor.Elements)
            {
                statement.With(element.Name, element.Property.GetValue(entry));
            }

            statement.Table(descriptor.TableName);
            return statement;
        }

        private bool DoSave(DatabaseEntryDescriptor descriptor, IDatabaseEntry entry)
        {
            SQLiteStatement statement = this.BuildInsertUpdateStatement(descriptor, entry);

            // Update the primary key if it's an insert statement into auto increment table
            bool needPrimaryKeyUpdate = statement.Type == SqlStatementType.Insert
                                        && descriptor.PrimaryKey.Attribute.PrimaryKeyMode == PrimaryKeyMode.Autoincrement;

            using (DbCommand command = this.connector.CreateCommand(statement))
            {
                int affected = command.ExecuteNonQuery();
                if (affected != 1)
                {
                    System.Diagnostics.Trace.TraceError("Expected 1 row affected but got {0}", affected);
                    return false;
                }
            }

            object primaryKeyValue;
            if (needPrimaryKeyUpdate)
            {
                using (DbCommand command = this.connector.CreateCommand())
                {
                    command.CommandText = SqlLiteConnector.SqlLastId;
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            System.Diagnostics.Trace.TraceError("Failed to retrieve the id for saved object");
                            return false;
                        }

                        primaryKeyValue = DatabaseUtils.GetInternalValue(descriptor.PrimaryKey.DatabaseType, reader[0]);
                        descriptor.PrimaryKey.Property.SetValue(entry, primaryKeyValue);
                    }
                }
            }
            else
            {
                primaryKeyValue = descriptor.PrimaryKey.Property.GetValue(entry);
            }

            // Process the joined properties
            foreach (DatabaseEntryJoinedElementDescriptor joinedElement in descriptor.JoinedElements)
            {
                DatabaseEntryDescriptor joinedDescriptor = DatabaseEntryDescriptor.GetDescriptor(joinedElement.InternalType);
                System.Diagnostics.Trace.Assert(joinedDescriptor != null, "Joined entry must have a valid Database Descriptor");

                // Make sure to check the joined table, it may not be created yet
                this.CheckTable(joinedDescriptor);

                // Note: this is not really fast, if the joined instance is not supplied a lookup is performed to delete it
                var instance = (IDatabaseEntry)joinedElement.Property.GetValue(entry);
                if (instance == null)
                {
                    var lookupKeyClause = new KeyValuePair<string, IList<object>>(joinedElement.ForeignKeyColumn, new List<object> { primaryKeyValue });

                    IList<object[]> results = this.DoLoadFields(
                        joinedDescriptor,
                        new List<KeyValuePair<string, IList<object>>> { lookupKeyClause },
                        joinedDescriptor.PrimaryKey.Name);

                    System.Diagnostics.Trace.Assert(results.Count <= 1, "Expected 1 or 0 results but got " + results.Count);
                    if (results.Count == 1)
                    {
                        System.Diagnostics.Trace.TraceWarning(
                            "Found one entry but null was supplied, deleting the joined entry!");
                        if (!this.DoDelete(joinedDescriptor, new List<object> { results[0][0] }))
                        {
                            System.Diagnostics.Trace.TraceError("Failed to delete joined entry {0}", results[0][0]);
                            return false;
                        }
                    }
                }
                else
                {
                    joinedElement.ForeignKeyProperty.SetValue(instance, primaryKeyValue);
                    if (!this.DoSave(joinedDescriptor, instance))
                    {
                        System.Diagnostics.Trace.TraceError("Failed to save joined entry");
                        return false;
                    }
                }
            }

            return true;
        }

        private void BuildBaseWhereClause(SQLiteStatement target, DatabaseEntryDescriptor descriptor, IList<object> keys)
        {
            if (keys != null && keys.Count > 0)
            {
                if (keys.Count == 1)
                {
                    if (keys[0] != descriptor.PrimaryKey.InternalType.GetDefault())
                    {
                        target.WhereConstraint(descriptor.PrimaryKey.Name, keys[0]);
                    }
                }
                else
                {
                    System.Diagnostics.Trace.Assert(keys.Count < 1000, "More than a thousand entries might not be giving the proper result!");

                    target.InConstraint(descriptor.PrimaryKey.Name, keys);
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
                            object value = DatabaseUtils.GetInternalValue(element.DatabaseType, reader[element.Name]);
                            element.Property.SetValue(entry, value);
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
                primaryKeyMap.Add(descriptor.PrimaryKey.Property.GetValue(entry), entry);
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
                IList<object[]> joinedResults = this.DoLoadFields(
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

                    joinedElement.Property.SetValue(primaryKeyMap[primaryKey], joinedEntry);
                }
            }

            return results;
        }

        private bool DoDelete(DatabaseEntryDescriptor descriptor, IList<object> keys)
        {
            var statement = new SQLiteStatement(SqlStatementType.Delete);
            statement.Table(descriptor.TableName);
            this.BuildBaseWhereClause(statement, descriptor, keys);

            using (DbCommand command = this.connector.CreateCommand(statement))
            {
                int affected = command.ExecuteNonQuery();
                if (affected != keys.Count)
                {
                    System.Diagnostics.Trace.TraceError("Expected {0} rows but got {1}", keys.Count, affected);
                    return false;
                }
            }

            return true;
        }

        private IList<object[]> DoLoadFields(DatabaseEntryDescriptor descriptor, IEnumerable<KeyValuePair<string, IList<object>>> whereConstraints, params string[] fieldNames)
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
                        statement.InConstraint(pair.Key, pair.Value);
                    }
                    else
                    {
                        statement.WhereConstraint(pair.Key, pair.Value[0]);
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
    }
}
