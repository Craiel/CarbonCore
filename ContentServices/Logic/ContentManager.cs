namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using System.Globalization;
    using System.Text;

    using CarbonCore.Utils.IO;

    using Core.Engine.Contracts.Resource;
    using Core.Engine.Resource.Content;
    using Core.Engine.Resource.Generic;

    public class ContentManager : IContentManager
    {
        private const string SqlNotNull = " NOT NULL";
        private const string SqlLastId = "SELECT last_insert_rowid()";
        
        private readonly SQLiteFactory factory;
        private readonly IList<string> checkedTableList;

        private readonly IDictionary<int, ContentLink> contentLinkCache;

        private readonly IDictionary<int, ContentQueryResult> contentQueryCache;

        private CarbonFile file;

        private SQLiteConnection connection;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ContentManager()
        {
            this.factory = new SQLiteFactory();
            this.checkedTableList = new List<string>();
            this.contentLinkCache = new Dictionary<int, ContentLink>();
            this.contentQueryCache = new Dictionary<int, ContentQueryResult>();
        }

        public void Dispose()
        {
            this.Disconnect();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ContentQueryResult<T> TypedLoad<T>(ContentQuery<T> criteria) where T : ICarbonContent
        {
            return new ContentQueryResult<T>(this, this.GetCommand(criteria));
        }

        public ContentQueryResult Load(ContentQuery criteria, bool useResultCache = true)
        {
            if (useResultCache)
            {
                int hash = criteria.GetHashCode();
                if (this.contentQueryCache.ContainsKey(hash))
                {
                    return this.contentQueryCache[hash];
                }

                var result = new ContentQueryResult(this, this.GetCommand(criteria));
                this.contentQueryCache.Add(hash, result);
                
                return result;
            }

            return new ContentQueryResult(this, this.GetCommand(criteria));
        }

        public T Load<T>(ContentLink link)
            where T : ICarbonContent
        {
            if (link.ContentId == null || link.Type == ContentLinkType.Unknown)
            {
                throw new ArgumentException("Invalid ContentLink");
            }

            switch (link.Type)
            {
                case ContentLinkType.Resource:
                    {
                        if (typeof(T) != typeof(ResourceEntry))
                        {
                            throw new InvalidOperationException();
                        }

                        break;
                    }

                default:
                    {
                        throw new DataException("Unknown content link type: " + link.Type);
                    }
            }

            return this.TypedLoad(new ContentQuery<T>().IsEqual("Id", link.ContentId)).UniqueResult();
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

        public ContentLink ResolveLink(int id)
        {
            lock (this.contentLinkCache)
            {
                if (!this.contentLinkCache.ContainsKey(id))
                {
                    ContentQueryResult result = this.Load(new ContentQuery(typeof(ContentLink)).IsEqual("Id", id));
                    this.contentLinkCache.Add(id, (ContentLink)result.UniqueResult(typeof(ContentLink)));
                }

                return this.contentLinkCache[id];
            }
        }

        public void ClearCache()
        {
            this.contentLinkCache.Clear();
            this.contentQueryCache.Clear();
        }

        public void Initialize(CarbonFile contentFile)
        {
            this.file = contentFile;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private SQLiteCommand GetCommand(ContentQuery criteria)
        {
            this.Connect();
            this.CheckTable(criteria.Type);

            SQLiteCommand command = this.connection.CreateCommand();
            command.CommandText = this.BuildSelectStatement(criteria);

            System.Diagnostics.Trace.TraceInformation("ConentManager.Load<{0}>: {1}", criteria.Type, command.CommandText);
            return command;
        }

        private string BuildSelectStatement(ContentQuery criteria)
        {
            string what = this.BuildSelect(criteria.Type);
            string where = this.BuildWhereClause(criteria.Criterion);
            string order = this.BuildOrder(criteria.Order);

            return this.AssembleStatement(what, where, order);
        }

        private string BuildInsertStatement(ICarbonContent content)
        {
            string insert = this.BuildInsert(content.GetType());
            string what = string.Join(", ", this.ProcessValuesForStorage(content));
            return string.Format("{0} VALUES ({1})", insert, what);
        }

        private string BuildUpdateStatement(ICarbonContent content)
        {
            IList<string> values = this.ProcessValuesForStorage(content);

            // Get the table
            string tableName = ContentReflection.GetTableName(content.GetType());

            // Build the primary key where clause so we update the proper entry
            ContentReflectionProperty keyInfo = ContentReflection.GetPrimaryKeyPropertyInfo(content.GetType());
            string where = string.Format("{0} = {1}", keyInfo.Name, keyInfo.Info.GetValue(content));

            // Fetch all values to update
            IList<ContentReflectionProperty> properties = ContentReflection.GetPropertyInfos(content.GetType());
            IList<string> segments = new List<string>();
            for (int i = 0; i < properties.Count; i++)
            {
                segments.Add(string.Format("{0} = {1}", properties[i].Name, values[i]));
            }

            return string.Format("UPDATE {0} SET {1} WHERE {2}", tableName, string.Join(", ", segments), where);
        }

        private string BuildDeleteStatement(ICarbonContent content)
        {
            // Get the table
            string tableName = ContentReflection.GetTableName(content.GetType());

            // Build the primary key where clause so we update the proper entry
            ContentReflectionProperty keyInfo = ContentReflection.GetPrimaryKeyPropertyInfo(content.GetType());
            string where = string.Format("{0} = {1}", keyInfo.Name, keyInfo.Info.GetValue(content));

            return string.Format("DELETE FROM {0} WHERE {1}", tableName, where);
        }

        private string AssembleStatement(string what, string where, string order)
        {
            var builder = new StringBuilder(what);
            if (!string.IsNullOrEmpty(where))
            {
                builder.AppendFormat(" WHERE {0}", where);
            }

            if (!string.IsNullOrEmpty(order))
            {
                builder.AppendFormat(" ORDER BY {0}", order);
            }

            return builder.ToString();
        }

        private string BuildSelect(Type targetType)
        {
            string tableName = ContentReflection.GetTableName(targetType);

            IList<ContentReflectionProperty> properties = ContentReflection.GetPropertyInfos(targetType);
            var builder = new StringBuilder();
            foreach (ContentReflectionProperty property in properties)
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(property.Name);
            }

            return string.Format("SELECT {0} FROM {1}", builder, tableName);
        }

        private string BuildInsert(Type targetType)
        {
            string tableName = ContentReflection.GetTableName(targetType);

            IList<ContentReflectionProperty> properties = ContentReflection.GetPropertyInfos(targetType);
            var builder = new StringBuilder();
            foreach (ContentReflectionProperty property in properties)
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(property.Name);
            }

            return string.Format("INSERT INTO {0} ({1})", tableName, builder);
        }

        private string BuildWhereClause(IEnumerable<ContentCriterion> criteria)
        {
            IList<string> segments = new List<string>();
            foreach (ContentCriterion criterion in criteria)
            {
                switch (criterion.Type)
                {
                    case CriterionType.Equals:
                        {
                            segments.Add(this.BuildEqualsSegment(criterion));
                            break;
                        }

                    case CriterionType.Contains:
                        {
                            segments.Add(this.BuildContainsSegment(criterion));
                            break;
                        }
                }
            }

            return string.Join(" AND ", segments);
        }

        private string BuildEqualsSegment(ContentCriterion criterion)
        {
            if (criterion.Values == null || criterion.Values.Length == 0 ||
                (criterion.Values.Length == 1 && criterion.Values[0] == null))
            {
                return criterion.Negate
                           ? string.Format("{0} IS NOT NULL", criterion.PropertyInfo.Name)
                           : string.Format("{0} IS NULL", criterion.PropertyInfo.Name);
            }

            if (criterion.Values.Length > 1)
            {
                System.Diagnostics.Trace.TraceWarning(
                    "Equals with multiple values detected for {0}, excess values will be ignored",
                    criterion.PropertyInfo.Name);
            }

            string segmentString = string.Format("{0} = {1}", criterion.PropertyInfo.Name, this.TranslateValue(criterion.Values[0]));
            if (criterion.Negate)
            {
                segmentString = string.Concat("NOT ", segmentString);
            }

            return segmentString;
        }

        private string BuildContainsSegment(ContentCriterion criterion)
        {
            if (criterion.Values == null || criterion.Values.Length == 0)
            {
                throw new DataException("Contains Segment can not be build without values");
            }

            if (criterion.Values.Length == 1)
            {
                System.Diagnostics.Trace.TraceWarning("Equals statement with single value detected for {0}", criterion.PropertyInfo.Name);
            }

            IList<string> containValues = new List<string>();
            for (int i = 0; i < criterion.Values.Length; i++)
            {
                containValues.Add(this.TranslateValue(criterion.Values[i]));
            }

            string segmentString = string.Format("{0} in ({1})", criterion.PropertyInfo.Name, string.Join(", ", containValues));
            if (criterion.Negate)
            {
                segmentString = string.Concat("NOT ", segmentString);
            }

            return segmentString;
        }

        private IList<string> ProcessValuesForStorage(ICarbonContent entry)
        {
            IList<ContentReflectionProperty> properties = ContentReflection.GetPropertyInfos(entry.GetType());
            IList<string> segments = new List<string>();
            foreach (ContentReflectionProperty property in properties)
            {
                object value = property.Info.GetValue(entry);
                segments.Add(this.TranslateValue(value));
                property.Info.SetValue(entry, value);
            }

            return segments;
        }

        private string TranslateValue(object value)
        {
            if (value == null)
            {
                return "NULL";
            }

            Type type = value.GetType();
            if (type == typeof(ContentLink))
            {
                this.Save((ContentLink)value);
                return ((ContentLink)value).Id.ToString();
            }

            if (type == typeof(DateTime))
            {
                return ((DateTime)value).Ticks.ToString(CultureInfo.InvariantCulture);
            }

            if (type.IsEnum)
            {
                return ((int)value).ToString(CultureInfo.InvariantCulture);
            }

            return string.Format("'{0}'", value);
        }

        private string BuildOrder(IEnumerable<ContentOrder> orders)
        {
            IList<string> segments = new List<string>();
            foreach (ContentOrder order in orders)
            {
                segments.Add(string.Format("ORDER BY {0} {1}", order.PropertyInfo.Name, order.Ascending ? "ASC" : "DESC"));
            }

            return string.Join(" ", segments);
        }

        private void Connect()
        {
            if (this.connection != null)
            {
                return;
            }

            // Todo:
            // - Load the database from the resource manager
            // - Pull the data into the memory database
            // - Release the resource
            // 
            // this.connection.ConnectionString = "Data Source=:memory:";
            this.connection = this.factory.CreateConnection() as SQLiteConnection;
            if (this.connection == null)
            {
                System.Diagnostics.Trace.TraceError("Could not create connection");
                return;
            }

            this.connection.ConnectionString = string.Format("Data Source={0}", this.file);
            this.connection.Open();
        }

        private void Disconnect()
        {
            if (this.connection != null)
            {
                this.connection.Dispose();
                this.connection = null;
            }

            this.checkedTableList.Clear();
        }
        
        private void CheckTable(Type type)
        {
            string tableName = ContentReflection.GetTableName(type);
            if (this.checkedTableList.Contains(tableName))
            {
                return;
            }
            
            IList<ContentReflectionProperty> properties = ContentReflection.GetPropertyInfos(type);
            SQLiteCommand command = this.connection.CreateCommand();
            command.CommandText = string.Format("PRAGMA table_info({0})", tableName);
            IList<object[]> pragmaInfo = new List<object[]>();
            using (SQLiteDataReader reader = command.ExecuteReader(CommandBehavior.Default))
            {
                while (reader.Read())
                {
                    var data = new object[reader.FieldCount];
                    if (reader.GetValues(data) != reader.FieldCount)
                    {
                        throw new InvalidOperationException("GetValues returned unexpected field count");
                    }

                    pragmaInfo.Add(data);
                }
            }

            bool needRecreate = properties.Count != pragmaInfo.Count;
            IList<string> columnSegments = new List<string>();
            IList<string> primaryKeySegments = new List<string>();
            for (int i = 0; i < properties.Count; i++)
            {
                string tableType = this.GetTableType(properties[i].Info.PropertyType);
                string tableColumn = properties[i].Name;

                if (properties[i].PrimaryKey != PrimaryKeyMode.None)
                {
                    // If the column is part of the primary key we have to mark it as not null
                    if (!tableType.Contains(SqlNotNull))
                    {
                        tableType = string.Concat(tableType, SqlNotNull);
                    }

                    switch (properties[i].PrimaryKey)
                    {
                        case PrimaryKeyMode.AutoIncrement:
                            {
                                primaryKeySegments.Add(string.Format("{0} autoincrement", tableColumn));
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
                    if (!tableType.Replace(SqlNotNull, string.Empty).Equals(pragmaInfo[i][2] as string, StringComparison.OrdinalIgnoreCase))
                    {
                        needRecreate = true;
                    }

                    if (!tableColumn.Equals(pragmaInfo[i][1] as string, StringComparison.OrdinalIgnoreCase))
                    {
                        needRecreate = true;
                    }
                }

                columnSegments.Add(string.Format("{0} {1}", tableColumn, tableType));
            }

            if (primaryKeySegments.Count > 0)
            {
                columnSegments.Add(string.Format("PRIMARY KEY ({0})", string.Join(",", primaryKeySegments)));
            }

            if (needRecreate && pragmaInfo.Count > 0)
            {
                throw new InvalidOperationException("Table was inconsistent but re-creation is not yet supported!");
            }

            if (needRecreate)
            {
                System.Diagnostics.Trace.TraceInformation("Table {0} needs to be re-created", tableName);
                command = this.connection.CreateCommand();
                command.CommandText = string.Format("CREATE TABLE {0}({1})", tableName, string.Join(",", columnSegments));
                command.ExecuteNonQuery();
            }

            this.checkedTableList.Add(tableName);
        }

        private string GetTableType(Type internalType)
        {
            string arguments = string.Empty;
            if (internalType.IsGenericType && internalType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                internalType = internalType.GetGenericArguments()[0];
            }
            else
            {
                if (!internalType.IsClass)
                {
                    arguments = " NOT NULL";
                }
            }

            if (internalType == typeof(string))
            {
                return string.Concat("VARCHAR", arguments);
            }

            if (internalType.IsEnum
                || internalType == typeof(int)
                || internalType == typeof(uint)
                || internalType == typeof(long)
                || internalType == typeof(ulong)
                || internalType == typeof(bool)
                || internalType == typeof(ContentLink)
                || internalType == typeof(DateTime))
            {
                return string.Concat("INTEGER", arguments);
            }

            if (internalType == typeof(float))
            {
                return string.Concat("FLOAT", arguments);
            }

            if (internalType == typeof(byte[]))
            {
                return string.Concat("BLOB", arguments);
            }

            throw new DataException("Type for value is not implemented: " + internalType);
        }
    }
}
