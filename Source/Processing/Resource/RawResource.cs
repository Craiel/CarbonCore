namespace CarbonCore.Processing.Resource
{
    using System.IO;
    
    public class RawResource : ProtocolResource
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public byte[] Data { get; set; }

        public override void Load(Stream source)
        {
            //this.Data = Protocol.Resource.Raw.ParseFrom(source).Data.ToByteArray();
        }

        public override long Save(Stream target)
        {
            // TODO:
            return 0;

            /*var builder = new Protocol.Resource.Raw.Builder();
            builder.SetData(ByteString.CopyFrom(this.Data));
            Protocol.Resource.Raw entry = builder.Build();
            entry.WriteTo(target);
            return entry.SerializedSize;*/
        }
    }
}
