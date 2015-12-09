namespace CarbonCore.ContentServices.Compat.Contracts
{
    public interface IDataEntry
    {
        bool IsChanged { get; }

        IDataEntry Clone();
        
        void ResetChangedState();
    }
}
