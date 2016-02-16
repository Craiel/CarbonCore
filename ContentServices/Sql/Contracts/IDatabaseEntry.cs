namespace CarbonCore.ContentServices.Sql.Contracts
{
    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Sql.Logic;

    public interface IDatabaseEntry : IDataEntry
    {
        DatabaseEntryDescriptor GetDescriptor();
    }
}
