namespace CarbonCore.ContentServices.Contracts
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.IO;

    public interface IDatabaseService : IDisposable
    {
        void Initialize(CarbonFile file);

        void Save<T>(ref T entry) where T : IDatabaseEntry;

        T Load<T>(object key) where T : IDatabaseEntry;

        void Delete<T>(IList<int> idValues = null) where T : IDatabaseEntry;
    }
}
