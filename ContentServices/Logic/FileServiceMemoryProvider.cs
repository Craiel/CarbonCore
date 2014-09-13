namespace CarbonCore.ContentServices.Logic
{
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Contracts;

    public class FileServiceMemoryProvider : FileServiceProvider, IFileServiceMemoryProvider
    {
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override bool DoInitialize()
        {
            return true;
        }

        protected override bool DoLoad(IFileEntry key, out byte[] data)
        {
            throw new System.NotImplementedException();
        }

        protected override bool DoSave(IFileEntry key, byte[] data)
        {
            throw new System.NotImplementedException();
        }

        protected override IList<IFileEntry> DoGetFiles()
        {
            return null;
        }
    }
}
