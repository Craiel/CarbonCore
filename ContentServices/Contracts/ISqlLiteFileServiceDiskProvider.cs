namespace CarbonCore.ContentServices.Contracts
{
    using CarbonCore.ContentServices.Compat.Contracts;
    using CarbonCore.Utils.Compat.IO;

    public interface ISqlLiteFileServiceDiskProvider : IFileServiceProvider
    {
        CarbonDirectory Root { get; set; }
    }
}
