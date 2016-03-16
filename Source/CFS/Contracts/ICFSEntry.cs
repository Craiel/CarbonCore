namespace CarbonCore.CFS.Contracts
{
    using System;

    using CarbonCore.Utils.IO;

    public interface ICFSEntry
    {
        CarbonFile FileName { get; }

        ulong Size { get; }

        bool IsCompressed { get; }

        DateTime LastWriteTime { get; }
    }
}