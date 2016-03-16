namespace CarbonCore.CFS.Contracts
{
    using System;

    public interface ICFSFilter
    {
        string Name { get; set; }

        string Extension { get; set; }

        ulong? BiggerThan { get; set; }
        ulong? SmallerThan { get; set; }

        DateTime? LastWriteTimeAfter { get; set; }
        DateTime? LastWriteTimeBefore { get; set; }
    }
}