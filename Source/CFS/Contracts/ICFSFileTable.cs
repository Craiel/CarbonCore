namespace CarbonCore.CFS.Contracts
{
    using System;
    using System.Collections.Generic;

    public interface ICFSFileTable
    {
        ulong Count { get; }

        DateTime LastChangeTime { get; }

        IList<ICFSEntry> GetEntries();

        void Update(ICFSEntry entry);

        void Delete(ICFSEntry entry);

        void VerifyConsistency(ICFSFileTable other);
    }
}
