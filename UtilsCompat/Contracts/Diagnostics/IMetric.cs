namespace CarbonCore.Utils.Compat.Contracts.Diagnostics
{
    public interface IMetric
    {
        int Id { get; }

        long Count { get; }
        
        void Reset();

        void Add(IMetric other);
    }
}
