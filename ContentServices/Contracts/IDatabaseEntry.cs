namespace CarbonCore.ContentServices.Contracts
{
    using CarbonCore.ContentServices.Logic;

    public interface IDatabaseEntry : IDataEntry
    {
        DatabaseEntryDescriptor GetDescriptor();
    }
}
