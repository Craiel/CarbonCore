namespace CarbonCore.Processing.Resource
{
    using System.IO;

    using Google.ProtocolBuffers;

    public class ScriptResource : ProtocolResource
    {
        internal const int Version = 1;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Script { get; set; }

        public void Load(Protocol.Resource.Script scriptData)
        {
            if (scriptData.Version != Version)
            {
                throw new InvalidDataException("Script version is not correct: " + scriptData.Version);
            }

            this.Script = scriptData.ScriptData.ToStringUtf8();
        }

        public override void Load(Stream source)
        {
            this.Load(Protocol.Resource.Script.ParseFrom(source));
        }

        public Protocol.Resource.Script Save()
        {
            if (string.IsNullOrEmpty(this.Script))
            {
                throw new InvalidDataException("Script was empty on Save");
            }

            var builder = new Protocol.Resource.Script.Builder
            {
                Version = Version,
                ScriptData = ByteString.CopyFromUtf8(this.Script)
            };

            return builder.Build();
        }

        public override long Save(Stream target)
        {
            Protocol.Resource.Script entry = this.Save();
            entry.WriteTo(target);
            return entry.SerializedSize;
        }
    }
}
