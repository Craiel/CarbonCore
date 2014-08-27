namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

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

            this.DoSave(descriptor, entry);
        }

        public T Load<T>(object key) where T : IDatabaseEntry
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            this.CheckTable(descriptor);

            IList<IDatabaseEntry> results = this.DoLoad(descriptor, key);
            System.Diagnostics.Trace.Assert(results.Count == 1, "Expected 1 result!");
            return (T)results[0];
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
        private IList<object[]> GetTableInfo(string tableName)
        {
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

            return info;
        }

        private void DropTable(DatabaseEntryDescriptor descriptor)
        {
            DbCommand command = this.connector.CreateCommand();

            var statement = new SQLiteStatement(SqlStatementType.Drop);
            statement.Table(descriptor.TableName);
            statement.IntoCommand(command);

            command.ExecuteNonQuery();
        }

        private void CreateTable(DatabaseEntryDescriptor descriptor)
        {
            DbCommand command = this.connector.CreateCommand();

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
            statement.IntoCommand(command);

            command.ExecuteNonQuery();
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
                System.Diagnostics.Trace.TraceInformation("Table {0} needs to be re-created", tableName);

                this.CreateTable(descriptor);
            }

            this.checkedTables.Add(descriptor.TableName, descriptor);
        }

        private void DoSave(DatabaseEntryDescriptor descriptor, IDatabaseEntry entry)
        {
            bool needPrimaryKeyUpdate = false;
            DbCommand command = this.connector.CreateCommand();
            SQLiteStatement statement;
            if (descriptor.PrimaryKey.Property.GetValue(entry) == null)
            {
                statement = new SQLiteStatement(SqlStatementType.Insert);
                needPrimaryKeyUpdate = true;
            }
            else
            {
                statement = new SQLiteStatement(SqlStatementType.Update);
                statement.WhereConstraint(descriptor.PrimaryKey.Name, descriptor.PrimaryKey.Property.GetValue(entry));
            }

            foreach (DatabaseEntryElementDescriptor element in descriptor.Elements)
            {
                statement.With(element.Name, element.Property.GetValue(entry));
            }

            statement.Table(descriptor.TableName);
            statement.IntoCommand(command);
            
            System.Diagnostics.Trace.TraceInformation("ContentManager.Save<{0}>: {1}", entry.GetType(), command.CommandText);
            int affected = command.ExecuteNonQuery();
            if (affected != 1)
            {
                throw new InvalidOperationException("Expected 1 row affected but got " + affected);
            }

            if (needPrimaryKeyUpdate)
            {
                command = this.connector.CreateCommand();
                command.CommandText = SqlLiteConnector.SqlLastId;
                DbDataReader reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    throw new InvalidOperationException("Failed to retrieve the id for saved object");
                }

                int lastId = reader.GetInt32(0);
                descriptor.PrimaryKey.Property.SetValue(entry, lastId);
            }
        }

        private IList<IDatabaseEntry> DoLoad(DatabaseEntryDescriptor descriptor, object key)
        {
            DbCommand command = this.connector.CreateCommand();
            var statement = new SQLiteStatement(SqlStatementType.Select);

            if (key != descriptor.PrimaryKey.InternalType.GetDefault())
            {
                statement.WhereConstraint(descriptor.PrimaryKey.Name, key);
            }

            foreach (DatabaseEntryElementDescriptor element in descriptor.Elements)
            {
                statement.What(element.Name);
            }

            statement.Table(descriptor.TableName);
            statement.IntoCommand(command);

            System.Diagnostics.Trace.TraceInformation("ContentManager.Load<{0}>: {1}", descriptor.Type, command.CommandText);
            IList<IDatabaseEntry> results = new List<IDatabaseEntry>();
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

            return results;
        }
    }
}
