namespace CarbonCore.ContentServices.Contracts
{
    public interface IDataEntry
    {
        bool IsChanged { get; }

        IDataEntry Clone();
        
        void ResetChangedState();
    }
}
