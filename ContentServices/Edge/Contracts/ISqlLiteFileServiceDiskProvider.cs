namespace CarbonCore.ContentServices.Edge.Contracts
{
    using CarbonCore.ContentServices.Compat.Contracts;
    using CarbonCore.Utils.IO;

    public interface ISqlLiteFileServiceDiskProvider : IFileServiceProvider
    {
        CarbonDirectory Root { get; set; }
    }
}
