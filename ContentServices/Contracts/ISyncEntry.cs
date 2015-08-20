namespace CarbonCore.ContentServices.Contracts
{
    using System.IO;

    public interface ISyncEntry
    {
        bool GetEntryChanged();

        void Save(Stream target);

        void Load(Stream source);

        void ResetChangeState(bool state = false);
    }
}
