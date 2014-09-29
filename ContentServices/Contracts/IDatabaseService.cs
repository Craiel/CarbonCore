namespace CarbonCore.ContentServices.Contracts
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.IO;

    public interface IDatabaseService : IDisposable
    {
        void Initialize(CarbonFile file);

        void Save<T>(ref T entry) where T : IDatabaseEntry;
        
        T Load<T>(object key, bool loadFull = false) where T : IDatabaseEntry;
        IList<T> Load<T>(IList<object> keys = null, bool loadFull = false) where T : IDatabaseEntry;

        void Delete<T>(object key) where T : IDatabaseEntry;
        
        int Count<T>(IList<object> keys = null) where T : IDatabaseEntry;

        void Drop<T>();

        void SaveAsync<T>(IList<T> entries) where T : IDatabaseEntry;
        void LoadAsync<T>(Action<IList<T>> callback, IList<object> keys = null, bool loadFull = false) where T : IDatabaseEntry;
        void DeleteAsync<T>(IList<object> keys) where T : IDatabaseEntry;

        IList<string> GetTables();
    }
}
