namespace CarbonCore.Utils
{
    using System;
    using System.IO;

    using CarbonCore.Utils.Contracts;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.IO;

    public class TextFile : ITextFile
    {
        private readonly object fileLock;

        private CarbonFile file;
        private TextWriter writer;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TextFile()
        {
            this.fileLock = new object();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CarbonFile File
        {
            get
            {
                return this.file;
            }

            set
            {
                if (this.file == null || !this.file.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    this.Close();
                    this.file = value;
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Write(string value)
        {
            lock (this.fileLock)
            {
                this.OpenIfNeeded();
                if (this.writer != null)
                {
                    this.writer.Write(value);
                    this.writer.Flush();
                }
            }
        }

        public void WriteLine(string line)
        {
            lock (this.fileLock)
            {
                this.OpenIfNeeded();
                if (this.writer != null)
                {
                    this.writer.WriteLine(line);
                    this.writer.Flush();
                }
            }
        }

        public void Clear()
        {
            lock (this.fileLock)
            {
                this.Close();
                this.file.DeleteIfExists();
            }
        }

        public void Close()
        {
            lock (this.fileLock)
            {
                if (this.writer != null)
                {
                    this.writer.Flush();
                    this.writer.Close();
                    this.writer.Dispose();
                }

                this.writer = null;
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            lock (this.fileLock)
            {
                this.Close();
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OpenIfNeeded()
        {
            if (this.writer != null)
            {
                return;
            }

            try
            {
                CarbonDirectory directory = this.file.GetDirectory();
                if (!directory.IsNull)
                {
                    directory.Create();
                }

                FileStream stream = this.file.OpenWrite(FileMode.Append);
                this.writer = new StreamWriter(stream);
            }
            catch (Exception e)
            {
                Diagnostic.Exception(e);
                Diagnostic.Error("Failed to open Log File: {0}", this.file);
            }
        }
    }
}
