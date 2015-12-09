namespace CarbonCore.ContentServices.Compat.Contracts
{
    using System.IO;

    public interface ISyncContent
    {
        bool IsChanged { get; }

        int Save(Stream stream);

        void Load(Stream stream);

        void ResetChangeState();
    }
}
