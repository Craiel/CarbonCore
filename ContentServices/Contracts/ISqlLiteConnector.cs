namespace CarbonCore.ContentServices.Compat.Contracts
{
    using CarbonCore.Utils.IO;

    public interface ISqlLiteConnector : IDataConnector
    {
        void SetFile(CarbonFile newFile);
    }
}
