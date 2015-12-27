namespace CarbonCore.Processing.Resource
{
    using System.IO;

    using CarbonCore.Processing.Data;

    public class CompiledShaderResource : ProtocolResource
    {
        internal const int Version = 1;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public byte[] Data { get; set; }
        public byte[] Md5 { get; set; }

        public void Load(CompiledShader shaderData)
        {
            if (shaderData.Version != Version)
            {
                throw new InvalidDataException("Shader version is not correct: " + shaderData.Version);
            }

            this.Data = shaderData.Data;
            this.Md5 = shaderData.MD5;
        }

        public override void Load(Stream source)
        {
            //this.Load(CompiledShader.ParseFrom(source));
        }

        public CompiledShader Save()
        {
            // TODO:
            return null;

            /*if (this.Data == null || this.Md5 == null)
            {
                throw new InvalidDataException("Shader data was empty or invalid on Save");
            }

            var builder = new CompiledShader.Builder
            {
                Version = Version,
                Data = ByteString.CopyFrom(this.Data),
                MD5 = ByteString.CopyFrom(this.Md5),
            };

            return builder.Build();*/
        }

        public override long Save(Stream target)
        {
            // TODO:
            return 0;

            /*CompiledShader entry = this.Save();
            entry.WriteTo(target);
            return entry.SerializedSize;*/
        }
    }
}
