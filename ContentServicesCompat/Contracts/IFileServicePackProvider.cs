namespace CarbonCore.ContentServices.Compat.Contracts
{
    using CarbonCore.Utils.Compat.IO;

    public interface IFileServicePackProvider : IFileServiceProvider
    {
        string PackName { get; set; }

        CarbonDirectory Root { get; set; }
    }
}
