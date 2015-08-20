﻿namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Compat.Logic;
    using CarbonCore.ContentServices.Contracts;

    public class FileServiceMemoryProvider : FileServiceProvider, IFileServiceMemoryProvider
    {
        private readonly IDictionary<FileEntryKey, FileEntry> fileEntries;
        private readonly IDictionary<FileEntryKey, byte[]> files;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileServiceMemoryProvider()
        {
            this.fileEntries = new Dictionary<FileEntryKey, FileEntry>();
            this.files = new Dictionary<FileEntryKey, byte[]>();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoInitialize()
        {
        }

        protected override void DoLoad(FileEntryKey key, out byte[] data)
        {
            if (!this.files.ContainsKey(key))
            {
                throw new ArgumentException("File does not exist: " + key);
            }

            data = this.files[key];
        }

        protected override void DoSave(FileEntryKey key, byte[] data)
        {
            if (!this.files.ContainsKey(key))
            {
                this.files.Add(key, null);
            }

            this.files[key] = data;

            if (!this.fileEntries.ContainsKey(key))
            {
                var entry = new FileEntry { Hash = key.Hash.Value, Size = data.Length };
                this.fileEntries.Add(key, entry);
            }
        }

        protected override void DoDelete(FileEntryKey key)
        {
            if (!this.files.ContainsKey(key))
            {
                throw new ArgumentException("File does not exist: " + key);
            }

            this.fileEntries[key].IsDeleted = true;
        }

        protected override int DoCleanup()
        {
            IList<FileEntryKey> deleteList = new List<FileEntryKey>();
            foreach (FileEntryKey key in this.fileEntries.Keys)
            {
                if (this.fileEntries[key].IsDeleted)
                {
                    deleteList.Add(key);
                }
            }

            foreach (FileEntryKey key in deleteList)
            {
                this.files.Remove(key);
                this.fileEntries.Remove(key);
            }

            return deleteList.Count;
        }

        protected override FileEntry LoadEntry(FileEntryKey key)
        {
            System.Diagnostics.Trace.Assert(this.files.ContainsKey(key));

            return this.fileEntries[key];
        }

        protected override void SaveEntry(FileEntryKey key, FileEntry entry)
        {
            System.Diagnostics.Trace.Assert(this.files.ContainsKey(key));

            this.fileEntries[key] = entry;
        }
        
        protected override IList<FileEntryKey> DoGetFiles(bool includeDeleted)
        {
            IList<FileEntryKey> results = new List<FileEntryKey>();
            foreach (FileEntryKey key in this.fileEntries.Keys)
            {
                if (this.fileEntries[key].IsDeleted && !includeDeleted)
                {
                    continue;
                }

                results.Add(key);
            }

            return results;
        }
    }
}
