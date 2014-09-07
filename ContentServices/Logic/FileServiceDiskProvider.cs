namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.IO;

    public class FileServiceDiskProvider : FileServiceProvider, IFileServiceDiskProvider
    {
        private const string IndexFile = "index.db";

        private readonly IDatabaseService databaseService;

        private CarbonDirectory root;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileServiceDiskProvider(IFactory factory)
        {
            this.databaseService = factory.Resolve<IDatabaseService>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CarbonDirectory Root
        {
            get
            {
                return this.root;
            }

            set
            {
                if (this.IsInitialized)
                {
                    throw new NotSupportedException("Can not change root after initialization");
                }

                if (this.root != value)
                {
                    this.root = value;
                }
            }
        }

        public override void Dispose()
        {
            this.databaseService.Dispose();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override bool DoInitialize()
        {
            if (!this.root.Exists)
            {
                this.root.Create();
            }

            this.Used = 0;
            this.Capacity = this.root.GetFreeSpace();

            // Initialize the index db
            CarbonFile index = this.root.ToFile(IndexFile);
            this.databaseService.Initialize(index);

            // Read all the file entries and verify


            return true;
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