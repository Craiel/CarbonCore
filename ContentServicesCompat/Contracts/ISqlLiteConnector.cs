namespace CarbonCore.ContentServices.Compat.Contracts
{
    using CarbonCore.Utils.Compat.IO;

    public interface ISqlLiteConnector : IDataConnector
    {
        void SetFile(CarbonFile newFile);
    }
}
