namespace CarbonCore.JSharpBridge.IO
{
    using System;
    using System.IO;
    using System.Text;

    public class FileWriter : TextWriter
    {
        private readonly Encoding encoding = null;

        public FileWriter(BridgedFile file, bool something = false)
        {
            throw new NotImplementedException();
        }

        public override Encoding Encoding
        {
            get
            {
                return this.encoding;
            }
        }
    }
}
