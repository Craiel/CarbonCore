namespace CarbonCore.ContentServices.Contracts
{
    using CarbonCore.Utils.IO;

    public interface IFileServicePackProvider : IFileServiceProvider
    {
        CarbonDirectory Root { get; set; }
    }
}
