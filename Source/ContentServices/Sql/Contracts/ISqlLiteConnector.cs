namespace CarbonCore.ContentServices.Sql.Contracts
{
    using CarbonCore.Utils.IO;

    public interface ISqlLiteConnector : IDataConnector
    {
        void SetFile(CarbonFile newFile);
    }
}