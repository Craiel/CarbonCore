namespace CarbonCore.ContentServices.Contracts
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.IO;

    public interface IDatabaseService : IDisposable
    {
        void Initialize(CarbonFile file);

        bool Save<T>(ref T entry) where T : IDatabaseEntry;
        bool Save<T>(IList<T> entries) where T : IDatabaseEntry;

        T Load<T>(object key) where T : IDatabaseEntry;
        IList<T> Load<T>(IList<object> keys = null) where T : IDatabaseEntry;

        bool Delete<T>(IList<object> keys) where T : IDatabaseEntry;

        int Count<T>(IList<object> keys = null) where T : IDatabaseEntry;

        bool Drop<T>();

        IList<string> GetTables();
    }
}
