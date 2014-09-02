namespace CarbonCore.ContentServices.Contracts
{
    public interface IFileEntry : IContentEntry
    {
        byte[] Data { get; }

        void UpdateData(byte[] newData);
    }
}
