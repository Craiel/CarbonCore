namespace CarbonCore.ContentServices.Contracts
{
    using System.IO;

    public interface IContentEntry
    {
        IContentEntry Clone();

        bool Load(Stream source);
        bool Save(Stream target);
    }
}
