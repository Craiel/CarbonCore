namespace CarbonCore.ContentServices.Edge.Contracts
{
    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.Utils.IO;

    public interface ISqlLiteFileServicePackProvider : IFileServiceProvider
    {
        CarbonDirectory Root { get; set; }
    }
}
