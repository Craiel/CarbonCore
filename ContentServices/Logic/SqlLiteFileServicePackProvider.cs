namespace CarbonCore.ContentServices.Logic
{
    using CarbonCore.ContentServices.Compat.Logic;
    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.Utils.Compat.Contracts.IoC;

    public class SqlLiteFileServicePackProvider : FileServicePackProvider, ISqlLiteFileServicePackProvider
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SqlLiteFileServicePackProvider(IFactory factory)
            : base(factory.Resolve<ISqlLiteDatabaseService>(), new ZipStreamCompressionProvider())
        {
        }
    }
}
