namespace CarbonCore.ContentServices.Contracts
{
    using CarbonCore.ContentServices.Logic;
    using CarbonCore.Utils.IO;

    public interface IFileServiceDiskProvider : IFileServiceProvider
    {
        CarbonDirectory Root { get; set; }
    }
}
