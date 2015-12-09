namespace CarbonCore.ContentServices.Compat.Contracts
{
    using CarbonCore.ContentServices.Compat.Logic;

    public interface IDatabaseEntry : IDataEntry
    {
        DatabaseEntryDescriptor GetDescriptor();
    }
}
