namespace CarbonCore.ContentServices.Sql.Contracts
{
    using CarbonCore.Utils.IO;

    public interface IFileServiceDiskProvider : IFileServiceProvider
    {
        CarbonDirectory Root { get; set; }
    }
}
