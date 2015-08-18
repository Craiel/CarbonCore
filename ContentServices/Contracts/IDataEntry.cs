namespace CarbonCore.ContentServices.Contracts
{
    using System.IO;

    public interface IDataEntry
    {
        bool IsChanged { get; }

        IDataEntry Clone();

        int NativeSave(Stream target);

        void NativeLoad(Stream source);

        void ResetChangedState();
    }
}
