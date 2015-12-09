namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Data;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.IO;

    public class FileServicePackProvider : FileServiceProvider, IFileServicePackProvider
    {
        private const int PaddingValueMultiplier = 512;
        private const int PaddingValueMin = 512;

        private const string PackFilePattern = "{0}.df";
        private const string IndexFile = "{0}.db";

        private readonly IDatabaseService databaseService;

        private readonly IDictionary<FileEntryKey, FileEntry> files;

        private readonly IDictionary<FileEntryKey, FileEntryPackInfo> packInfo;

        private string packName = "Data";

        private long endOfFile;

        private CarbonDirectory root;

        private FileStream packStream;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileServicePackProvider(IFactory factory)
            : this(factory.Resolve<IJsonDatabaseService>(), new DefaultCompressionProvider())
        {
        }

        public FileServicePackProvider(IDatabaseService customDatabase, ICompressionProvider customProvider)
            : base(customProvider)
        {
            this.databaseService = customDatabase;
            this.files = new Dictionary<FileEntryKey, FileEntry>();
            this.packInfo = new Dictionary<FileEntryKey, FileEntryPackInfo>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string PackName
        {
            get
            {
                return this.packName;
            }

            set
            {
                if (this.IsInitialized)
                {
                    throw new InvalidOperationException("Can not change the pack prefix after initialization");
                }

                this.packName = value;
            }
        }

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
            CarbonFile pack = this.root.ToFile(string.Format(PackFilePattern, this.packName));
            this.packStream = pack.OpenWrite(FileMode.Create);
            System.Diagnostics.Trace.Assert(this.packStream != null);

            // Initialize the index db
            CarbonFile index = this.root.ToFile(string.Format(IndexFile, this.packName));
            this.databaseService.Initialize(index);

            // Read all the file entries and verify
            IList<FileEntry> entries = this.databaseService.Load<FileEntry>();
            foreach (FileEntry entry in entries)
            {
                System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(entry.Hash));

                this.files.Add(new FileEntryKey(entry.Hash), entry);
            }

            // Read all the pack info
            IList<FileEntryPackInfo> packInfoList = this.databaseService.Load<FileEntryPackInfo>();
            foreach (FileEntryPackInfo info in packInfoList)
            {
                System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(info.Hash));
                if (info.Offset > this.endOfFile)
                {
                    this.endOfFile = info.Offset + info.Size + info.Padding;
                }

                this.packInfo.Add(new FileEntryKey(info.Hash), info);
            }
        }

        protected override void DoLoad(FileEntryKey key, out byte[] data)
        {
            if (!this.packInfo.ContainsKey(key))
            {
                throw new FileNotFoundException("Could not load file data: " + key);
            }

            FileEntryPackInfo info = this.packInfo[key];
            data = new byte[info.Size];
            lock (this.packStream)
            {
                this.packStream.Seek(info.Offset, SeekOrigin.Begin);
                this.packStream.Read(data, 0, (int)info.Size);
            }
        }

        protected override void DoSave(FileEntryKey key, byte[] data)
        {
            FileEntryPackInfo info = null;
            if (this.packInfo.ContainsKey(key))
            {
                info = this.packInfo[key];

                // Check if the new data still fits into the region
                if (info.Size + info.Padding < data.Length)
                {
                    // The data won't fit, move the data to the end of the file, the old chunk becomes orphaned
                    info.Offset = this.endOfFile;
                    info.Padding = this.GetPaddingValue(data.Length);
                }
                else
                {
                    // It fits so set the data to write and adjust the padding
                    long oldSize = info.Size + info.Padding;
                    info.Size = data.Length;
                    info.Padding = oldSize - info.Size;
                }
            }

            // We have to lock the info creation as well to make sure end of file is incremented linear
            lock (this.packStream)
            {
                if (info == null)
                {
                    info = new FileEntryPackInfo
                    {
                        Hash = key.Hash,
                        Padding = this.GetPaddingValue(data.Length),
                        Offset = this.endOfFile,
                        Size = data.Length
                    };

                    this.endOfFile += data.Length;
                }

                var paddingBuffer = new byte[info.Padding];
                this.packStream.Seek(info.Offset, SeekOrigin.Begin);
                this.packStream.Write(data, 0, data.Length);
                this.packStream.Write(paddingBuffer, 0, paddingBuffer.Length);
                this.packStream.Flush();
            }

            // Save the info into the db and into our storage 
            this.databaseService.Save(ref info);
            this.packInfo[key] = info;

            // If we don't have an entry for this hash create one and save it
            if (!this.files.ContainsKey(key))
            {
                var entry = new FileEntry { Hash = key.Hash, Size = data.Length };
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

            this.databaseService.Delete<FileEntryPackInfo>(this.files[key].Id);
            this.databaseService.Delete<FileEntry>(this.files[key].Id);
            this.files.Remove(key);
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
                    object key = file.GetDescriptor().PrimaryKey.GetValue(file);
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

        private int GetPaddingValue(long size)
        {
            var filePadding = (int)((((int)(size / PaddingValueMultiplier) + 1) * PaddingValueMultiplier) - size);
            var partialSize = (long)(size * 0.01);
            if (partialSize < PaddingValueMin)
            {
                return filePadding + PaddingValueMin;
            }

            var multiplier = (int)(partialSize / PaddingValueMultiplier);
            int extraPadding = multiplier * PaddingValueMultiplier;
            return filePadding + extraPadding;
        }
    }
}
