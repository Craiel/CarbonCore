namespace CarbonCore.ContentServices.Compat.Contracts
{
    using CarbonCore.Utils.IO;

    public interface IFileServicePackProvider : IFileServiceProvider
    {
        string PackName { get; set; }

        CarbonDirectory Root { get; set; }
    }
}
