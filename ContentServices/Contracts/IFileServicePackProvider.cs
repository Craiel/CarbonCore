namespace CarbonCore.ContentServices.Contracts
{
    using CarbonCore.Utils.IO;

    public interface IFileServicePackProvider : IFileServiceProvider
    {
        string PackPrefix { get; set; }

        CarbonDirectory Root { get; set; }
    }
}
