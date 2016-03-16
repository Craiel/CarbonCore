namespace CarbonCore.ContentServices.Edge.Contracts
{
    using CarbonCore.ContentServices.Sql.Contracts;
    using CarbonCore.Utils.IO;

    public interface ISqlLiteFileServicePackProvider : IFileServiceProvider
    {
        CarbonDirectory Root { get; set; }
    }
}
