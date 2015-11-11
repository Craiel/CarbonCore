namespace CarbonCore.ContentServices.Contracts
{
    using CarbonCore.ContentServices.Compat.Contracts;
    using CarbonCore.Utils.Compat.IO;

    public interface ISqlLiteFileServicePackProvider : IFileServiceProvider
    {
        CarbonDirectory Root { get; set; }
    }
}
