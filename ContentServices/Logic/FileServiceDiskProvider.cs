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

        private readonly IDictionary<string, IFileEntry> files;

        private CarbonDirectory root;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileServiceDiskProvider(IFactory factory)
        {
            this.databaseService = factory.Resolve<IDatabaseService>();
            this.files = new Dictionary<string, IFileEntry>();
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
                System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(entry.Hash));

                this.files.Add(entry.Hash, entry);
            }

            return true;
        }

        protected override bool DoLoad(string hash, out byte[] data)
        {
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(hash));

            throw new System.NotImplementedException();
        }

        protected override bool DoSave(string hash, byte[] data)
        {
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(hash));

            CarbonFile file = this.root.ToFile(hash);
            using (var stream = file.OpenWrite())
            {
                stream.Write(data, 0, data.Length);
            }

            var local = (FileEntry)key;
            this.databaseService.Save(ref local);
            this.files.Add(key);

            return true;
        }

        protected override bool DoDelete(string hash)
        {
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(hash));
            System.Diagnostics.Trace.Assert(this.files.ContainsKey(hash));

            CarbonFile file = this.root.ToFile(hash);
            System.Diagnostics.Trace.Assert(file.Exists, "Entry to delete is not in the provider!");

            IFileEntry entry = this.files[hash];
            entry.IsDeleted = true;
            if (this.databaseService.Save(ref entry))
            {
                this.files.Remove(hash);
                return true;
            }

            return false;
        }

        protected override int DoCleanup()
        {
            IList<object> deleteList = new List<object>();
            IList<FileEntry> activeFiles = this.databaseService.Load<FileEntry>();
            foreach (FileEntry file in activeFiles)
            {
                if (file.IsDeleted)
                {
                    deleteList.Add(file.GetDescriptor().PrimaryKey.Property.GetValue(file));
                }
            }

            if (this.databaseService.Delete<FileEntry>(deleteList))
            {
                return deleteList.Count;
            }

            return -1;
        }

        protected override IList<IFileEntry> DoGetFiles()
        {
            return new List<IFileEntry>(this.files.Values);
        }
    }
}