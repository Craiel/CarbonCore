namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.IO;

    public class FileServiceDiskProvider : FileServiceProvider, IFileServiceDiskProvider
    {
        private const string IndexFile = "index.db";

        private readonly IDatabaseService databaseService;

        private readonly IList<IFileEntry> files; 

        private CarbonDirectory root;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileServiceDiskProvider(IFactory factory)
        {
            this.databaseService = factory.Resolve<IDatabaseService>();
            this.files = new List<IFileEntry>();
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

        public IFileEntry CreateEntry(CarbonFile source)
        {
            DateTime createTime = DateTime.Now;
            DateTime modifyTime = DateTime.Now;
            long size = 0;
            if (source.Exists)
            {
                createTime = source.CreateTime;
                modifyTime = source.LastWriteTime;
                size = source.Size;
            }

            var entry = new FileEntry
                            {
                                CreateDate = createTime,
                                ModifyDate = modifyTime,
                                Size = size,
                                Version = 1
                            };

            return entry;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this.databaseService.Dispose();
            base.Dispose(true);
        }

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
            IList<FileEntry> entries = this.databaseService.Load<FileEntry>();
            foreach (FileEntry entry in entries)
            {
                this.files.Add(entry);
            }

            return true;
        }

        protected override bool DoLoad(IFileEntry key, out byte[] data)
        {
            throw new System.NotImplementedException();
        }

        protected override bool DoSave(IFileEntry key, byte[] data)
        {
            CarbonFile file = this.root.ToFile(key.Hash);
            using (var stream = file.OpenWrite())
            {
                stream.Write(data, 0, data.Length);
            }

            var local = (FileEntry)key;
            this.databaseService.Save(ref local);
            this.files.Add(key);

            return true;
        }

        protected override IList<IFileEntry> DoGetFiles()
        {
            return new List<IFileEntry>(this.files);
        }
    }
}