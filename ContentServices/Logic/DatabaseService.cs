namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Text;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.IO;

    public class DatabaseService : IDatabaseService
    {
        private const string StatementTableInfo = "PRAGMA table_info({0})";
        private const string StatementTableCreate = "CREATE TABLE {0}({1})";
        private const string StatementPrimaryKey = "PRIMARY KEY ({0})";
        private const string StatementSelectBase = "SELECT {0} FROM {1}";
        private const string StatementUpdateBase = "UPDATE {0} SET {1} WHERE {2}";
        private const string StatementDeleteBase = "DELETE FROM {0} WHERE {1}";
        private const string StatementInsertBase = "INSERT INTO {0} ({1})";
        private const string StatementValues = "{0} VALUES ({1})";
        private const string StatementWhere = " WHERE {0}";
        private const string StatementOrderBy = " ORDER BY {0} {1}";
        private const string StatementAnd = " AND ";
        private const string StatementNotNull = "{0} IS NOT NULL";
        private const string StatementNull = "{0} IS NULL";
        private const string StatementNegate = "NOT ";
        private const string StatementIn = "{0} IN ({1})";
        private const string ValueDelimiter = ",";

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
            if (this.connector != null)
            {
                this.connector.Dispose();
            }
        }

        public void Initialize(CarbonFile file)
        {
            this.activeFile = file;
            this.connector.SetFile(this.activeFile);
            this.connector.Connect();
        }

        public void Save<T>(ref T entry) where T : IDatabaseEntry
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            throw new NotImplementedException();
        }

        public void Save<T>(IList<T> entries) where T : IDatabaseEntry
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            throw new NotImplementedException();
        }

        public T Load<T>(int id) where T : IDatabaseEntry
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            throw new NotImplementedException();
        }

        public IList<T> Load<T>(IList<int> idValues = null) where T : IDatabaseEntry
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            throw new NotImplementedException();
        }

        public void Delete<T>(IList<int> idValues = null) where T : IDatabaseEntry
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            throw new NotImplementedException();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void CheckTable(DatabaseEntryDescriptor descriptor)
        {
            string tableName = descriptor.TableName;
            if (this.checkedTables.ContainsKey(tableName))
            {
                return;
            }

            DbCommand command = this.connector.CreateCommand();
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

            bool needRecreate = descriptor.Elements.Count != info.Count;
            IList<string> columnSegments = new List<string>();
            IList<string> primaryKeySegments = new List<string>();
            for (int i = 0; i < descriptor.Elements.Count; i++)
            {
                string tableType = DatabaseUtils.GetDatabaseType(descriptor.Elements[i].Property.PropertyType).ToString();
                string tableColumn = descriptor.Elements[i].Name;

                if (descriptor.PrimaryKey.Attribute.Equals(descriptor.Elements[i].Attribute))
                {
                    // If the column is part of the primary key we have to mark it as not null
                    if (!tableType.Contains(this.connector.NotNullStatement))
                    {
                        tableType = string.Concat(tableType, this.connector.NotNullStatement);
                    }

                    switch (descriptor.PrimaryKey.Attribute.PrimaryKeyMode)
                    {
                        case PrimaryKeyMode.Autoincrement:
                            {
                                primaryKeySegments.Add(string.Format("{0} AUTOINCREMENT", tableColumn));
                                break;
                            }

                        default:
                            {
                                primaryKeySegments.Add(tableColumn);
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

                columnSegments.Add(string.Format("{0} {1}", tableColumn, tableType));
            }

            if (primaryKeySegments.Count > 0)
            {
                columnSegments.Add(string.Format(StatementPrimaryKey, string.Join(ValueDelimiter, primaryKeySegments)));
            }

            if (needRecreate && info.Count > 0)
            {
                throw new InvalidOperationException("Table was inconsistent but re-creation is not yet supported!");
            }

            if (needRecreate)
            {
                System.Diagnostics.Trace.TraceInformation("Table {0} needs to be re-created", tableName);
                command = this.connector.CreateCommand();
                command.CommandText = string.Format(StatementTableCreate, tableName, string.Join(",", columnSegments));
                command.ExecuteNonQuery();
            }

            this.checkedTables.Add(descriptor.TableName, descriptor);
        }

        private string BuildSelectStatement(DatabaseQuery criteria)
        {
            string what = this.BuildSelect(criteria.Descriptor);
            string where = this.BuildWhereClause(criteria.Criterion);
            string order = this.BuildOrder(criteria.Order);

            return this.AssembleStatement(what, where, order);
        }

        private string BuildInsertStatement(IDatabaseEntry entry)
        {
            string insert = this.BuildInsert(entry.GetDescriptor());
            string what = string.Join(ValueDelimiter, this.ProcessValuesForStorage(entry));
            return string.Format(StatementValues, insert, what);
        }

        private string BuildUpdateStatement(IDatabaseEntry entry)
        {
            // Get the descriptor
            DatabaseEntryDescriptor descriptor = entry.GetDescriptor();

            // Build the primary key where clause so we update the proper entry
            string where = string.Format("{0} = {1}", descriptor.PrimaryKey.Name, descriptor.PrimaryKey.Property.GetValue(entry));

            // Fetch all values to update
            IList<string> values = this.ProcessValuesForStorage(entry);
            IList<string> segments = new List<string>();
            for (int i = 0; i < descriptor.Elements.Count; i++)
            {
                segments.Add(string.Format("{0} = {1}", descriptor.Elements[i].Name, values[i]));
            }

            return string.Format(StatementUpdateBase, descriptor.TableName, string.Join(ValueDelimiter, segments), where);
        }

        private string BuildDeleteStatement(IDatabaseEntry entry)
        {
            // Get the descriptor
            var descriptor = entry.GetDescriptor();

            // Build the primary key where clause so we update the proper entry
            string where = string.Format("{0} = {1}", descriptor.PrimaryKey, descriptor.PrimaryKey.Property.GetValue(entry));

            return string.Format(StatementDeleteBase, descriptor.TableName, where);
        }

        private string AssembleStatement(string what, string where, string order)
        {
            var builder = new StringBuilder(what);
            if (!string.IsNullOrEmpty(where))
            {
                builder.AppendFormat(StatementWhere, where);
            }

            if (!string.IsNullOrEmpty(order))
            {
                builder.AppendFormat(StatementOrderBy, order, string.Empty);
            }

            return builder.ToString();
        }

        private string BuildSelect(DatabaseEntryDescriptor descriptor)
        {
            var builder = new StringBuilder();
            foreach (DatabaseEntryElementDescriptor element in descriptor.Elements)
            {
                if (builder.Length > 0)
                {
                    builder.Append(ValueDelimiter);
                }

                builder.Append(element.Name);
            }

            return string.Format(StatementSelectBase, builder, descriptor.TableName);
        }

        private string BuildInsert(DatabaseEntryDescriptor descriptor)
        {
            var builder = new StringBuilder();
            foreach (DatabaseEntryElementDescriptor element in descriptor.Elements)
            {
                if (builder.Length > 0)
                {
                    builder.Append(ValueDelimiter);
                }

                builder.Append(element);
            }

            return string.Format(StatementInsertBase, descriptor.TableName, builder);
        }

        private string BuildWhereClause(IEnumerable<DatabaseQueryCriterion> criteria)
        {
            IList<string> segments = new List<string>();
            foreach (DatabaseQueryCriterion criterion in criteria)
            {
                switch (criterion.Type)
                {
                    case DatabaseQueryCriterionType.Equals:
                        {
                            segments.Add(this.BuildEqualsSegment(criterion));
                            break;
                        }

                    case DatabaseQueryCriterionType.Contains:
                        {
                            segments.Add(this.BuildContainsSegment(criterion));
                            break;
                        }
                }
            }

            return string.Join(StatementAnd, segments);
        }

        private string BuildEqualsSegment(DatabaseQueryCriterion criterion)
        {
            if (criterion.Values == null || criterion.Values.Length == 0 ||
                (criterion.Values.Length == 1 && criterion.Values[0] == null))
            {
                return criterion.Negate
                           ? string.Format(StatementNotNull, criterion.Name)
                           : string.Format(StatementNull, criterion.Name);
            }

            if (criterion.Values.Length > 1)
            {
                System.Diagnostics.Trace.TraceWarning(
                    "Equals with multiple values detected for {0}, excess values will be ignored",
                    criterion.Name);
            }

            string segmentString = string.Format("{0} = {1}", criterion.Name, DatabaseUtils.TranslateValue(criterion.DatabaseType, criterion.Values[0]));
            if (criterion.Negate)
            {
                segmentString = string.Concat(StatementNegate, segmentString);
            }

            return segmentString;
        }

        private string BuildContainsSegment(DatabaseQueryCriterion criterion)
        {
            if (criterion.Values == null || criterion.Values.Length == 0)
            {
                throw new DataException("Contains Segment can not be build without values");
            }

            if (criterion.Values.Length == 1)
            {
                System.Diagnostics.Trace.TraceWarning("Equals statement with single value detected for {0}", criterion.Name);
            }

            IList<string> containValues = new List<string>();
            for (int i = 0; i < criterion.Values.Length; i++)
            {
                containValues.Add(DatabaseUtils.TranslateValue(criterion.DatabaseType, criterion.Values[i]).ToString());
            }

            string segmentString = string.Format(StatementIn, criterion.Name, string.Join(ValueDelimiter, containValues));
            if (criterion.Negate)
            {
                segmentString = string.Concat(StatementNegate, segmentString);
            }

            return segmentString;
        }

        private IList<string> ProcessValuesForStorage(IDatabaseEntry entry)
        {
            DatabaseEntryDescriptor descriptor = entry.GetDescriptor();
            IList<string> segments = new List<string>();
            foreach (DatabaseEntryElementDescriptor element in descriptor.Elements)
            {
                object value = element.Property.GetValue(entry);
                segments.Add(DatabaseUtils.TranslateValue(DatabaseUtils.GetDatabaseType(descriptor.Type), value).ToString());
                element.Property.SetValue(entry, value);
            }

            return segments;
        }
        
        private string BuildOrder(IEnumerable<DatabaseQueryOrder> orders)
        {
            IList<string> segments = new List<string>();
            foreach (DatabaseQueryOrder order in orders)
            {
                segments.Add(string.Format(StatementOrderBy, order.Name, order.Ascending ? "ASC" : "DESC"));
            }

            return string.Join(" ", segments);
        }

        private void DoSave(IDatabaseEntry entry)
        {
            /*DatabaseEntryDescriptor descriptor = entry.GetDescriptor();

            bool needPrimaryKeyUpdate = false;
            DbCommand command = this.connector.CreateCommand();
            if (descriptor.PrimaryKey.Property.GetValue(entry) == null)
            {
                command.CommandText = this.BuildInsertStatement(content);
                needPrimaryKeyUpdate = true;
            }
            else
            {
                command.CommandText = this.BuildUpdateStatement(content);
            }

            System.Diagnostics.Trace.TraceInformation("ContentManager.Save<{0}>: {1}", content.GetType(), command.CommandText);
            int affected = command.ExecuteNonQuery();
            if (affected != 1)
            {
                throw new InvalidOperationException("Expected 1 row affected but got " + affected);
            }

            if (needPrimaryKeyUpdate)
            {
                command = this.connector.CreateCommand();
                command.CommandText = SqlLastId;
                DbDataReader reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    throw new InvalidOperationException("Failed to retrieve the id for saved object");
                }

                int lastId = reader.GetInt32(0);
                primaryKeyProperty.Info.SetValue(content, lastId);
            }

            content.LockChangeState();*/
        }

        /*public ContentQueryResult<T> TypedLoad<T>(ContentQuery<T> criteria) where T : ICarbonContent
        {
            return new ContentQueryResult<T>(this, this.GetCommand(criteria));
        }

        public DatabaseQueryResult Load(DatabaseQuery criteria, bool useResultCache = true)
        {
            if (useResultCache)
            {
                int hash = criteria.GetHashCode();
                if (this.contentQueryCache.ContainsKey(hash))
                {
                    return this.contentQueryCache[hash];
                }

                var result = new DatabaseQueryResult(this, this.GetCommand(criteria));
                this.contentQueryCache.Add(hash, result);
                
                return result;
            }

            return new DatabaseQueryResult(this, this.GetCommand(criteria));
        }
        
        public void Save(ICarbonContent content)
        {
            this.Connect();
            this.CheckTable(content.GetType());

            bool needPrimaryKeyUpdate = false;
            SQLiteCommand command = this.connection.CreateCommand();
            ContentReflectionProperty primaryKeyProperty = ContentReflection.GetPrimaryKeyPropertyInfo(content.GetType());
            if (primaryKeyProperty.Info.GetValue(content) == null)
            {
                command.CommandText = this.BuildInsertStatement(content);
                needPrimaryKeyUpdate = true;
            }
            else
            {
                command.CommandText = this.BuildUpdateStatement(content);
            }

            System.Diagnostics.Trace.TraceInformation("ContentManager.Save<{0}>: {1}", content.GetType(), command.CommandText);
            int affected = command.ExecuteNonQuery();
            if (affected != 1)
            {
                throw new InvalidOperationException("Expected 1 row affected but got " + affected);
            }

            if (needPrimaryKeyUpdate)
            {
                command = this.connection.CreateCommand();
                command.CommandText = SqlLastId;
                SQLiteDataReader reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    throw new InvalidOperationException("Failed to retrieve the id for saved object");
                }

                int lastId = reader.GetInt32(0);
                primaryKeyProperty.Info.SetValue(content, lastId);
            }

            content.LockChangeState();
        }

        public void Delete(ICarbonContent content)
        {
            this.Connect();
            this.CheckTable(content.GetType());

            SQLiteCommand command = this.connection.CreateCommand();
            command.CommandText = this.BuildDeleteStatement(content);

            System.Diagnostics.Trace.TraceInformation("ContentManager.Delete<{0}>: {1}", content.GetType(), command.CommandText);
            int affected = command.ExecuteNonQuery();
            if (affected != 1)
            {
                throw new InvalidOperationException("Expected 1 row affected but got " + affected);
            }

            content.Invalidate();
        }


        public void ClearCache()
        {
            this.contentQueryCache.Clear();
        }

        public void Initialize(CarbonFile contentFile)
        {
            this.file = contentFile;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        

        
        
        

        */
    }
}
