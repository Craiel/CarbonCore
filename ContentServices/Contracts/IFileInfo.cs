namespace CarbonCore.ContentServices.Contracts
{
    using System;

    public interface IFileInfo
    {
        string Hash { get; set; }

        int Version { get; }

        DateTime CreateDate { get; }
        DateTime ModifyDate { get; }

        void Update(int newVersion);
    }
}
