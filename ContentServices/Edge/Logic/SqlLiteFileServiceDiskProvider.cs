namespace CarbonCore.ContentServices.Edge.Logic
{
    using CarbonCore.ContentServices.Edge.Compat.Logic;
    using CarbonCore.ContentServices.Edge.Contracts;
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
