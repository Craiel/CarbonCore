namespace CarbonCore.ContentServices.Contracts
{
    using System.IO;

    public interface IDataEntry
    {
        bool IsChanged { get; }

        IDataEntry Clone();

        bool Save(Stream target);
        bool Load(Stream source);
        
        void ResetChangedState();
    }
}
