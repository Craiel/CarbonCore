namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.IO;

    public class FileServicePackProvider : FileServiceProvider, IFileServicePackProvider
    {
        private const string PackFile = "data.df";
        private const string IndexFile = "index.db";

        private readonly IDatabaseService databaseService;

        private readonly IDictionary<FileEntryKey, FileEntry> files;

        private readonly IDictionary<FileEntryKey, FileEntryPackInfo> packInfo;

        private CarbonDirectory root;

        private FileStream packStream;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileServicePackProvider(IFactory factory)
        {
            this.databaseService = factory.Resolve<IDatabaseService>();
            this.files = new Dictionary<FileEntryKey, FileEntry>();
            this.packInfo = new Dictionary<FileEntryKey, FileEntryPackInfo>();
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

                this.root = value;
            }
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

            if (this.packStream != null)
            {
                this.packStream.Dispose();
                this.packStream = null;
            }

            this.databaseService.Dispose();
            base.Dispose(true);
        }

        protected override void DoInitialize()
        {
            if (!this.root.Exists)
            {
                this.root.Create();
            }

            this.Used = 0;
            this.Capacity = this.root.GetFreeSpace();

            // Initialize the pack file
            CarbonFile pack = this.root.ToFile(PackFile);
            this.packStream = pack.OpenWrite(FileMode.Create);
            System.Diagnostics.Trace.Assert(this.packStream != null);

            // Initialize the index db
            CarbonFile index = this.root.ToFile(IndexFile);
            this.databaseService.Initialize(index);

            // Read all the file entries and verify
            IList<FileEntry> entries = this.databaseService.Load<FileEntry>();
            foreach (FileEntry entry in entries)
            {
                System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(entry.Hash));

                this.files.Add(new FileEntryKey(entry.Hash), entry);
            }

            // Read all the pack info
            IList<FileEntryPackInfo> packInfos = this.databaseService.Load<FileEntryPackInfo>();
            foreach (FileEntryPackInfo info in packInfos)
            {
                System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(info.Hash));

                this.packInfo.Add(new FileEntryKey(info.Hash), info);
            }
        }

        protected override void DoLoad(FileEntryKey key, out byte[] data)
        {
            throw new NotImplementedException();
            /*CarbonFile file = this.root.ToFile(key.Hash);
            if (!file.Exists)
            {
                throw new FileNotFoundException("Could not load file data", file.GetPath());
            }

            using (var stream = file.OpenRead())
            {
                data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
            }*/
        }

        protected override void DoSave(FileEntryKey key, byte[] data)
        {
            throw new NotImplementedException();
            /*CarbonFile file = this.root.ToFile(key.Hash);
            using (var stream = file.OpenWrite())
            {
                stream.Write(data, 0, data.Length);
            }*/

            // If we don't have an entry for this hash create one and save it
            if (!this.files.ContainsKey(key))
            {
                var entry = new FileEntry { Hash = key.Hash.Value, Size = data.Length };
                this.databaseService.Save(ref entry, true);
                lock (this.files)
                {
                    this.files.Add(key, entry);
                }
            }
        }

        protected override void DoDelete(FileEntryKey key)
        {
            if (!this.files.ContainsKey(key))
            {
                throw new InvalidOperationException(string.Format("Can not delete {0}, was not in the provider", key));
            }

            throw new NotImplementedException();
            //CarbonFile file = this.root.ToFile(key.Hash);
            //System.Diagnostics.Trace.Assert(file.Exists, "Entry to delete is not in the provider!");

            // First mark the file as deleted in the table
            var entry = this.files[key];
            entry.IsDeleted = true;
            this.databaseService.Save(ref entry, true);

            // Now delete the file itself if all went well
            //file.Delete();
        }

        protected override int DoCleanup()
        {
            // Wait to have pending writes flushed before cleanup
            this.databaseService.WaitForAsyncActions();

            IList<object> deleteList = new List<object>();
            IList<FileEntry> activeFiles = this.databaseService.Load<FileEntry>();
            foreach (FileEntry file in activeFiles)
            {
                if (file.IsDeleted)
                {
                    object key = file.GetDescriptor().PrimaryKey.Property.GetValue(file);
                    deleteList.Add(key);
                    this.databaseService.Delete<FileEntry>(key, true);
                }
            }

            return deleteList.Count;
        }

        protected override FileEntry LoadEntry(FileEntryKey key)
        {
            // Wait to have pending writes flushed before loading
            this.databaseService.WaitForAsyncActions();

            System.Diagnostics.Trace.Assert(this.files.ContainsKey(key));

            return this.files[key];
        }

        protected override void SaveEntry(FileEntryKey key, FileEntry entry)
        {
            System.Diagnostics.Trace.Assert(this.files.ContainsKey(key));

            if (this.files.ContainsKey(key))
            {
                this.files[key] = entry;
            }
            else
            {
                this.files.Add(key, entry);
            }
        }
        
        protected override IList<FileEntryKey> DoGetFiles(bool includeDeleted)
        {
            IList<FileEntryKey> results = new List<FileEntryKey>();
            foreach (FileEntryKey key in this.files.Keys)
            {
                if (this.files[key].IsDeleted && !includeDeleted)
                {
                    continue;
                }

                results.Add(key);
            }

            return results;
        }
    }
}
