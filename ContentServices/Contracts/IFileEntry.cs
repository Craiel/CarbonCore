namespace CarbonCore.ContentServices.Contracts
{
    using System;

    public interface IFileEntry : IContentEntry, IDisposable
    {
        byte[] Data { get; }

        void UpdateData(byte[] newData);
    }
}
