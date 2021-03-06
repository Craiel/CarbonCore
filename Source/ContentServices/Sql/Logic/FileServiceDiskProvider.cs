﻿namespace CarbonCore.ContentServices.Sql.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Data;
    using CarbonCore.ContentServices.Logic;
    using CarbonCore.ContentServices.Sql.Contracts;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.IO;

    public class FileServiceDiskProvider : FileServiceProvider, IFileServiceDiskProvider
    {
        private const string IndexFile = "index.db";

        private readonly IDatabaseService databaseService;

        private readonly IDictionary<FileEntryKey, FileEntry> files;

        private CarbonDirectory root;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileServiceDiskProvider(IFactory factory)
            : this(factory.Resolve<IJsonDatabaseService>(), new DefaultCompressionProvider())
        {
        }

        public FileServiceDiskProvider(IDatabaseService customDatabase, ICompressionProvider customProvider)
            : base(customProvider)
        {
            this.databaseService = customDatabase;
            this.files = new Dictionary<FileEntryKey, FileEntry>();
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

            // Initialize the index db
            CarbonFile index = this.root.ToFile(IndexFile);
            this.databaseService.Initialize(index);

            // Read all the file entries and verify
            IList<FileEntry> entries = this.databaseService.Load<FileEntry>();
            foreach (FileEntry entry in entries)
            {
                Debug.Assert(!string.IsNullOrEmpty(entry.Hash));

                this.files.Add(new FileEntryKey(entry.Hash), entry);
            }
        }

        protected override void DoLoad(FileEntryKey key, out byte[] data)
        {
            CarbonFile file = this.root.ToFile(key.Hash);
            if (!file.Exists)
            {
                throw new FileNotFoundException("Could not load file data", file.GetPath());
            }

            using (var stream = file.OpenRead())
            {
                data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
            }
        }

        protected override void DoSave(FileEntryKey key, byte[] data)
        {
            CarbonFile file = this.root.ToFile(key.Hash);
            using (var stream = file.OpenWrite())
            {
                stream.Write(data, 0, data.Length);
            }

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
            FileEntry entry;
            if (!this.files.TryGetValue(key, out entry))
            {
                throw new InvalidOperationException(string.Format("Can not delete {0}, was not in the provider", key));
            }

            CarbonFile file = this.root.ToFile(key.Hash);
            Debug.Assert(file.Exists, "Entry to delete is not in the provider!");

            // First mark the file as deleted in the table
            entry.IsDeleted = true;
            this.databaseService.Save(ref entry, true);

            // Now delete the file itself if all went well
            file.Delete();
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

            Debug.Assert(this.files.ContainsKey(key));

            return this.files[key];
        }

        protected override void SaveEntry(FileEntryKey key, FileEntry entry)
        {
            Debug.Assert(this.files.ContainsKey(key));

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