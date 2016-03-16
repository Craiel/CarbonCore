namespace CarbonCore.Utils.IO
{
    using System.IO;
    using System.Text;

    public class NoDisposeStreamWriter : StreamWriter
    {
        // ------------------------------------------------------------------- 
        // Constructor
        // ------------------------------------------------------------------- 
        public NoDisposeStreamWriter(Stream stream)
            : base(stream)
        {
        }

        public NoDisposeStreamWriter(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
        }

        public NoDisposeStreamWriter(Stream stream, Encoding encoding, int bufferSize)
            : base(stream, encoding, bufferSize)
        {
        }

        public NoDisposeStreamWriter(string path)
            : base(path)
        {
        }

        public NoDisposeStreamWriter(string path, bool append)
            : base(path, append)
        {
        }

        public NoDisposeStreamWriter(string path, bool append, Encoding encoding)
            : base(path, append, encoding)
        {
        }

        public NoDisposeStreamWriter(string path, bool append, Encoding encoding, int bufferSize)
            : base(path, append, encoding, bufferSize)
        {
        }

        // ------------------------------------------------------------------- 
        // Protected 
        // ------------------------------------------------------------------- 
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Flush();
            }
            
            base.Dispose(false);
        }
    }
}
