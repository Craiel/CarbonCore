namespace CarbonCore.ContentServices.Compat.Contracts
{
    using CarbonCore.Utils.Compat.IO;

    public interface IFileServiceDiskProvider : IFileServiceProvider
    {
        CarbonDirectory Root { get; set; }
    }
}
