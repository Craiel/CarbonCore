namespace CarbonCore.ContentServices.Contracts
{
    using System;

    public interface IFileEntry : IDatabaseEntry
    {
        string Hash { get; set; }

        int Version { get; set; }

        bool IsDeleted { get; set; }

        long Size { get; set; }

        DateTime CreateDate { get; set; }
        DateTime ModifyDate { get; set; }
    }
}
