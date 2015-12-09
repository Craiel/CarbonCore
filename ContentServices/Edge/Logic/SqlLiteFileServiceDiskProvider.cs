namespace CarbonCore.ContentServices.Edge.Logic
{
    using CarbonCore.ContentServices.Edge.Contracts;
    using CarbonCore.ContentServices.Logic;
    using CarbonCore.Utils.Contracts.IoC;

    public class SqlLiteFileServiceDiskProvider : FileServiceDiskProvider, ISqlLiteFileServiceDiskProvider
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SqlLiteFileServiceDiskProvider(IFactory factory)
            : base(factory.Resolve<ISqlLiteDatabaseService>(), new ZipStreamCompressionProvider())
        {
        }
    }
}
