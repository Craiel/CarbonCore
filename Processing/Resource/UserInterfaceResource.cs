namespace CarbonCore.Processing.Resource
{
    using System.IO;

    using CarbonCore.Protocol.Resource;
    
    public class UserInterfaceResource : ProtocolResource
    {
        internal const int Version = 1;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Csaml CsamlData { get; set; }
        public ScriptResource Script { get; set; }

        public override void Load(Stream source)
        {
            UserInterface entry = UserInterface.ParseFrom(source);

            if (entry.Version != Version)
            {
                throw new InvalidDataException("UserInterface version is not correct: " + entry.Version);
            }

            this.CsamlData = entry.Csaml;

            this.Script = new ScriptResource();
            this.Script.Load(entry.Script);
        }

        public override long Save(Stream target)
        {
            if (this.CsamlData == null)
            {
                throw new InvalidDataException("CsamlData was empty on Save");
            }

            var builder = new UserInterface.Builder
                              {
                                  Version = Version,
                                  Csaml = this.CsamlData,
                                  Script = this.Script.Save()
                              };

            UserInterface entry = builder.Build();
            entry.WriteTo(target);
            return entry.SerializedSize;
        }
    }
}
