namespace CarbonCore.ContentServices.Contracts
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.IO;

    public interface IDatabaseService : IDisposable
    {
        void Initialize(CarbonFile file);

        void Save<T>(ref T entry) where T : IDatabaseEntry;

        void Save<T>(IList<T> entries) where T : IDatabaseEntry;

        T Load<T>(int id) where T : IDatabaseEntry;

        IList<T> Load<T>(IList<int> idValues = null) where T : IDatabaseEntry;

        void Delete<T>(IList<int> idValues = null) where T : IDatabaseEntry;
    }
}
