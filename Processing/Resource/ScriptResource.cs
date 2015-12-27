namespace CarbonCore.Processing.Resource
{
    using System.IO;

    using CarbonCore.Processing.Data;
    
    public class ScriptResource : ProtocolResource
    {
        internal const int Version = 1;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Script { get; set; }

        public void Load(Script scriptData)
        {
            // TODO: 
            
            /*if (scriptData.Version != Version)
            {
                throw new InvalidDataException("Script version is not correct: " + scriptData.Version);
            }

            this.Script = scriptData.ScriptData.ToStringUtf8();*/
        }

        public override void Load(Stream source)
        {
            //this.Load(Protocol.Resource.Script.ParseFrom(source));
        }

        public Script Save()
        {
            // TODO:
            return null;

            /*if (string.IsNullOrEmpty(this.Script))
            {
                throw new InvalidDataException("Script was empty on Save");
            }

            var builder = new Protocol.Resource.Script.Builder
            {
                Version = Version,
                ScriptData = ByteString.CopyFromUtf8(this.Script)
            };

            return builder.Build();*/
        }

        public override long Save(Stream target)
        {
            // TODO:
            return 0;

            /*Script entry = this.Save();
            entry.WriteTo(target);
            return entry.SerializedSize;*/
        }
    }
}
