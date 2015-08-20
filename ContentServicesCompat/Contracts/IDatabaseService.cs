namespace CarbonCore.ContentServices.Compat.Contracts
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Compat.IO;

    public interface IDatabaseService : IDisposable
    {
        void Initialize(CarbonFile file);

        void Save<T>(ref T entry, bool async = false) where T : IDatabaseEntry;
        void Save<T>(IList<T> entries, bool async = false) where T : IDatabaseEntry;
        
        T Load<T>(object key, bool loadFull = false) where T : IDatabaseEntry;
        IList<T> Load<T>(IList<object> keys = null, bool loadFull = false) where T : IDatabaseEntry;

        void Delete<T>(object key, bool async = false) where T : IDatabaseEntry;
        void Delete<T>(IList<object> keys, bool async = false) where T : IDatabaseEntry;
        
        int Count<T>(IList<object> keys = null) where T : IDatabaseEntry;

        void Drop<T>();
        
        IList<string> GetTables();

        void WaitForAsyncActions();
    }
}
