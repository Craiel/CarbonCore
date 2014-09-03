namespace CarbonCore.ContentServices.Logic
{
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Contracts;

    public class FileServicePackProvider : FileServiceProvider
    {
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override bool DoInitialize()
        {
            throw new System.NotImplementedException();
        }

        protected override bool DoLoad(IFileInfo key, out byte[] data)
        {
            throw new System.NotImplementedException();
        }

        protected override bool DoSave(IFileInfo key, byte[] data)
        {
            throw new System.NotImplementedException();
        }

        protected override IList<IFileInfo> DoGetFiles()
        {
            throw new System.NotImplementedException();
        }
    }
}
