namespace CarbonCore.ContentServices.Contracts
{
    using System.IO;

    public interface ISyncEntry
    {
        bool IsChanged { get; }
        
        void Save(Stream target, bool ignoreChangeState = false);

        void Load(Stream source);

        void ResetChangeState(bool state = false);
    }
}
