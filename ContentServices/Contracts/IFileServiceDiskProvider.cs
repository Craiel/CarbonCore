namespace CarbonCore.ContentServices.Contracts
{
    using CarbonCore.Utils.IO;

    public interface IFileServiceDiskProvider : IFileServiceProvider
    {
        CarbonDirectory Root { get; set; }

        IFileEntry CreateEntry(CarbonFile source);
    }
}
