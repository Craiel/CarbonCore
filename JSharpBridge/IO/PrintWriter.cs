namespace CarbonCore.JSharpBridge.IO
{
    using System.IO;
    using System.Text;

    public class PrintWriter : TextWriter
    {
        private readonly Encoding encoding = null;

        public PrintWriter(TextWriter var4)
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoding
        {
            get
            {
                return this.encoding;
            }
        }

        public void Println(string p0 = null)
        {
            throw new System.NotImplementedException();
        }

        public void Print(string func77441A)
        {
            throw new System.NotImplementedException();
        }
    }
}
