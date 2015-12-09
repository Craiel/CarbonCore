namespace CarbonCore.ContentServices.Edge.Contracts
{
    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.Utils.IO;

    public interface ISqlLiteFileServiceDiskProvider : IFileServiceProvider
    {
        CarbonDirectory Root { get; set; }
    }
}
