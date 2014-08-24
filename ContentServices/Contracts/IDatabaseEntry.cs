namespace CarbonCore.ContentServices.Contracts
{
    using CarbonCore.ContentServices.Logic;

    public interface IDatabaseEntry : IContentEntry
    {
        DatabaseEntryDescriptor GetDescriptor();
    }
}
