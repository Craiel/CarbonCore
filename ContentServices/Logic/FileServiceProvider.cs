namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;

    using CarbonCore.ContentServices.Contracts;

    public abstract class FileServiceProvider : IFileServiceProvider
    {
        private readonly List<CompressionLevel> supportedCompressionLevels = new List<CompressionLevel>();

        private CompressionLevel compressionLevel = CompressionLevel.NoCompression;

        protected FileServiceProvider()
        {
            this.RegisterCompressionLevel(CompressionLevel.NoCompression);
            this.RegisterCompressionLevel(CompressionLevel.Fastest);
            this.RegisterCompressionLevel(CompressionLevel.Optimal);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CompressionLevel CompressionLevel
        {
            get
            {
                return this.compressionLevel;
            }

            set
            {
                if (this.IsInitialized)
                {
                    throw new InvalidOperationException("Can not change compression use after initialization");
                }

                if (!this.supportedCompressionLevels.Contains(value))
                {
                    throw new InvalidOperationException(string.Format("Provider {0} does not support compression {1}", this.GetType(), value));
                }

                this.compressionLevel = value;
            }
        }

        public bool SupportsCompression { get; protected set; }

        public bool IsInitialized { get; protected set; }

        public long Capacity { get; protected set; }

        public long Used { get; protected set; }

        public long BytesRead { get; private set; }

        public long BytesWritten { get; private set; }

        public long BytesReadActual { get; private set; }

        public long BytesWrittenActual { get; private set; }

        public long EntriesDeleted { get; private set; }

        public IReadOnlyCollection<CompressionLevel> SupportedCompressionLevels
        {
            get
            {
                return this.supportedCompressionLevels.AsReadOnly();
            }
        }

        public void Load(FileEntryKey key, out byte[] data)
        {
            System.Diagnostics.Trace.Assert(this.IsInitialized);
            if (this.compressionLevel != CompressionLevel.NoCompression)
            {
                byte[] compressed;
                this.DoLoad(key, out compressed);
                this.ProcessCompression(compressed, out data, CompressionMode.Decompress, this.compressionLevel);
                this.BytesRead += data.Length;
                this.BytesReadActual += compressed.Length;
            }
            else
            {
                this.DoLoad(key, out data);
                this.BytesRead += data.Length;
                this.BytesReadActual += data.Length;
            }
        }

        public void Save(FileEntryKey key, byte[] data)
        {
            System.Diagnostics.Trace.Assert(this.IsInitialized);
            if (this.compressionLevel != CompressionLevel.NoCompression)
            {
                byte[] compressed;
                this.ProcessCompression(data, out compressed, CompressionMode.Compress, this.compressionLevel);
                this.DoSave(key, compressed);
                this.BytesWritten += data.Length;
                this.BytesWrittenActual += compressed.Length;
            }
            else
            {
                this.DoSave(key, data);
                this.BytesWritten += data.Length;
                this.BytesWrittenActual += data.Length;
            }
        }

        public void Delete(FileEntryKey key)
        {
            System.Diagnostics.Trace.Assert(this.IsInitialized);
            this.DoDelete(key);
            this.EntriesDeleted++;
        }

        public int Cleanup()
        {
            return this.DoCleanup();
        }

        public void Initialize()
        {
            System.Diagnostics.Trace.Assert(!this.IsInitialized);
            this.DoInitialize();
            this.IsInitialized = true;
        }

        public IList<FileEntryKey> GetFiles(bool includeDeleted = false)
        {
            System.Diagnostics.Trace.Assert(this.IsInitialized);

            return this.DoGetFiles(includeDeleted);
        }

        public void SetVersion(FileEntryKey key, int version)
        {
            FileEntry entry = this.LoadEntry(key);
            entry.Version = version;
            this.SaveEntry(key, entry);
        }

        public int GetVersion(FileEntryKey key)
        {
            FileEntry entry = this.LoadEntry(key);
            return entry.Version;
        }

        public void SetCreateDate(FileEntryKey key, DateTime date)
        {
            FileEntry entry = this.LoadEntry(key);
            entry.CreateDate = date;
            this.SaveEntry(key, entry);
        }

        public DateTime GetCreateDate(FileEntryKey key)
        {
            FileEntry entry = this.LoadEntry(key);
            return entry.CreateDate;
        }

        public void SetModifiedDate(FileEntryKey key, DateTime date)
        {
            FileEntry entry = this.LoadEntry(key);
            entry.ModifyDate = date;
            this.SaveEntry(key, entry);
        }

        public DateTime GetModifiedDate(FileEntryKey key)
        {
            FileEntry entry = this.LoadEntry(key);
            return entry.ModifyDate;
        }

        public void GetMetadata(FileEntryKey key, int metadataKey, out int? value, out string stringValue)
        {
            throw new NotImplementedException();
        }

        public void SetMetadata(FileEntryKey key, int metadataKey, int? value = null, string stringValue = null)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Protected
        // ------------------------------------------------------------------- 
        protected virtual void Dispose(bool disposing)
        {
        }

        protected abstract void DoInitialize();

        protected abstract void DoLoad(FileEntryKey key, out byte[] data);

        protected abstract void DoSave(FileEntryKey key, byte[] data);

        protected abstract void DoDelete(FileEntryKey key);

        protected abstract int DoCleanup();

        protected abstract FileEntry LoadEntry(FileEntryKey key);
        protected abstract void SaveEntry(FileEntryKey key, FileEntry entry);

        protected abstract IList<FileEntryKey> DoGetFiles(bool includeDeleted);

        protected void RegisterCompressionLevel(CompressionLevel level)
        {
            this.supportedCompressionLevels.Add(level);
        }
        
        protected void ProcessCompression(byte[] source, out byte[] target, CompressionMode mode, CompressionLevel level)
        {
            using (var stream = new MemoryStream(source))
            {
                using (var targetStream = new MemoryStream())
                {
                    if (mode == CompressionMode.Compress)
                    {
                        using (var zipStream = new GZipStream(targetStream, level, true))
                        {
                            stream.WriteTo(zipStream);
                        }
                    }
                    else
                    {
                        using (var zipStream = new GZipStream(stream, mode, true))
                        {
                            int count;
                            var buffer = new byte[2048];
                            do
                            {
                                count = zipStream.Read(buffer, 0, 2048);
                                if (count > 0)
                                {
                                    targetStream.Write(buffer, 0, count);
                                }
                            }
                            while (count > 0);
                        }
                    }

                    // Write the modified data to the target buffer
                    target = new byte[targetStream.Length];
                    targetStream.Seek(0, SeekOrigin.Begin);
                    targetStream.Read(target, 0, (int)targetStream.Length);
                }
            }
        }
    }
}
