namespace CarbonCore.ContentServices.Contracts
{
    using System;

    public interface IFileInfo : IContentEntry
    {
        string Hash { get; set; }

        int Version { get; }

        long Size { get; }

        DateTime CreateDate { get; }
        DateTime ModifyDate { get; }

        void Update(int newVersion, DateTime createDate, DateTime modifyDate, long size);
    }
}
