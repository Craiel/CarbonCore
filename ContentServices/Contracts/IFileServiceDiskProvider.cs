namespace CarbonCore.ContentServices.Compat.Contracts
{
    using CarbonCore.Utils.IO;

    public interface IFileServiceDiskProvider : IFileServiceProvider
    {
        CarbonDirectory Root { get; set; }
    }
}
