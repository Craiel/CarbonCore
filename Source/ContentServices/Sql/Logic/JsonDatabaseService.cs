namespace CarbonCore.ContentServices.Sql.Logic
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    
    using CarbonCore.ContentServices.Sql.Contracts;
    using CarbonCore.ContentServices.Sql.Data;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.Json;

    public class JsonDatabaseService : IJsonDatabaseService
    {
        private CarbonFile databaseFile;

        private JsonDatabase database;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Dispose()
        {
            this.Dispose(true);
        }

        public void Initialize(CarbonFile file)
        {
            this.databaseFile = file;
            this.LoadDatabase();
        }

        public void Save<T>(ref T entry, bool async = false) where T : IDatabaseEntry
        {
            this.Save(new List<T> { entry }, async);
        }

        public void Save<T>(IList<T> entries, bool async = false) where T : IDatabaseEntry
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            JsonDatabaseTable table;
            if (!this.database.Tables.ContainsKey(descriptor.TableName))
            {
                table = new JsonDatabaseTable();
                this.database.Tables.Add(descriptor.TableName, table);
            }
            else
            {
                table = this.database.Tables[descriptor.TableName];
            }

            foreach (T entry in entries)
            {
                int? primaryKey = (int?)descriptor.GetPrimaryKey(entry);
                if (primaryKey == null)
                {
                    primaryKey = table.NextPrimaryKey++;
                    descriptor.SetPrimaryKey(entry, primaryKey.Value);
                }

                string contents = JsonExtensions.SaveToData(entry);
                if (table.RowData.ContainsKey(primaryKey.Value))
                {
                    table.RowData[primaryKey.Value] = contents;
                }
                else
                {
                    table.RowData.Add(primaryKey.Value, contents);
                }
            }

            this.SaveDatabase();
        }
        
        public T Load<T>(object key, bool loadFull = false) where T : IDatabaseEntry
        {
            IList<T> results = this.Load<T>(new List<object> { key }, loadFull);
            Debug.Assert(results.Count <= 1, "Expected 1 result but got " + results.Count);

            return results.FirstOrDefault();
        }

        public IList<T> Load<T>(IList<object> keys = null, bool loadFull = false) where T : IDatabaseEntry
        {
            IList<T> results = new List<T>();

            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            if (!this.database.Tables.ContainsKey(descriptor.TableName))
            {
                return results;
            }

            IList<int> keyValues = this.GetEntries(descriptor, keys);
            if (keyValues == null || keyValues.Count <= 0)
            {
                return results;
            }
            
            JsonDatabaseTable table = this.database.Tables[descriptor.TableName];
            foreach (int key in keyValues)
            {
                T entry = JsonExtensions.LoadFromData<T>(table.RowData[key]);
                results.Add(entry);
            }

            return results;
        }

        public void Delete<T>(object key, bool async = false) where T : IDatabaseEntry
        {
            this.Delete<T>(new List<object> { key }, async);
        }

        public void Delete<T>(IList<object> keys, bool async = false) where T : IDatabaseEntry
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            IList<int> entries = this.GetEntries(descriptor, keys);
            foreach (int key in entries)
            {
                this.database.Tables[descriptor.TableName].RowData.Remove(key);
            }

            this.SaveDatabase();
        }

        public int Count<T>(IList<object> keys = null) where T : IDatabaseEntry
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();

            IList<int> entries = this.GetEntries(descriptor, keys);
            return entries == null ? 0 : entries.Count;
        }

        public void Drop<T>()
        {
            DatabaseEntryDescriptor descriptor = DatabaseEntryDescriptor.GetDescriptor<T>();
            if (this.database.Tables.ContainsKey(descriptor.TableName))
            {
                this.database.Tables.Remove(descriptor.TableName);
                this.SaveDatabase();
            }
        }

        public IList<string> GetTables()
        {
            return new List<string>(this.database.Tables.Keys);
        }

        public void WaitForAsyncActions()
        {
            // Nothing to do here
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.SaveDatabase();
            }
        }

        private void SaveDatabase()
        {
            JsonExtensions.SaveToFile(this.databaseFile, this.database);
        }

        private void LoadDatabase()
        {
            this.database = this.databaseFile.Exists 
                ? JsonExtensions.LoadFromFile<JsonDatabase>(this.databaseFile) 
                : new JsonDatabase();
        }

        private IList<int> GetEntries(DatabaseEntryDescriptor descriptor, IList<object> keys)
        {
            JsonDatabaseTable table;
            if (!this.database.Tables.TryGetValue(descriptor.TableName, out table))
            {
                return null;
            }

            if (keys == null)
            {
                return new List<int>(table.RowData.Keys);
            }
            
            IList<int> intKeys = keys.Select(x => (int)x).ToList();
            IList<int> results = new List<int>();
            foreach (int key in table.RowData.Keys)
            {
                if (!intKeys.Contains(key))
                {
                    continue;
                }

                results.Add(key);
            }

            return results;
        }
    }
}
