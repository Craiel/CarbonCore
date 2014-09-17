namespace CarbonCore.ContentServices.Logic
{
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Contracts;

    public class FileServicePackProvider : FileServiceProvider, IFileServicePackProvider
    {
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override bool DoInitialize()
        {
            return true;
        }

        protected override bool DoLoad(string hash, out byte[] data)
        {
            throw new System.NotImplementedException();
        }

        protected override bool DoSave(string hash, byte[] data)
        {
            throw new System.NotImplementedException();
        }

        protected override bool DoDelete(string hash)
        {
            throw new System.NotImplementedException();
        }

        protected override int DoCleanup()
        {
            throw new System.NotImplementedException();
        }

        protected override IList<FileEntry> DoGetFiles(bool includeDeleted)
        {
            return null;
        }
    }
}
