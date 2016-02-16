namespace CarbonCore.ContentServices.Sql.Contracts
{
    using CarbonCore.Utils.IO;

    public interface IFileServicePackProvider : IFileServiceProvider
    {
        string PackName { get; set; }

        CarbonDirectory Root { get; set; }
    }
}
