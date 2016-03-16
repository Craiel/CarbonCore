namespace CarbonCore.ContentServices.Contracts
{
    public interface ISmartDataEntry : IDataEntry
    {
        bool IsChanged { get; }
        
        void ResetChangedState();
    }
}
