namespace CarbonCore.ContentServices.Compat.Contracts
{
    using System.IO;

    public interface ISyncEntry
    {
        bool GetEntryChanged();

        void Save(Stream target, bool ignoreChangeState = false);

        void Load(Stream source);

        void ResetChangeState(bool state = false);
    }
}
