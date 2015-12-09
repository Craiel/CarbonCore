namespace CarbonCore.Processing.Logic
{
    using System;
    using CarbonCore.Processing.Contracts;
    using CarbonCore.Processing.Processors;
    using CarbonCore.Processing.Resource;
    using CarbonCore.Processing.Resource.Model;
    using CarbonCore.Processing.Resource.Stage;
    using CarbonCore.Processing.Source.Collada;
    using CarbonCore.Processing.Source.Xcd;
    using CarbonCore.Utils.IO;

    public delegate string ReferenceResolveDelegate(string reference);

    public class ResourceProcessor : IResourceProcessor
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CarbonDirectory TextureToolsPath
        {
            get
            {
                return TextureProcessor.TextureToolsPath;
            }

            set
            {
                TextureProcessor.TextureToolsPath = value;
            }
        }

        public RawResource ProcessRaw(CarbonDirectory path)
        {
            return Utils.Edge.Diagnostics.Internal.NotImplemented<RawResource>();
        }

        public RawResource ProcessRaw(CarbonFile file)
        {
            if (!CarbonFile.FileExists(file))
            {
                throw new ArgumentException("Given path is invalid");
            }

            using (var stream = file.OpenRead())
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);
                return new RawResource { Data = data };
            }
        }

        public RawResource ProcessTexture(CarbonFile file, TextureProcessingOptions options)
        {
            return TextureProcessor.Process(file, options);
        }

        public RawResource ProcessFont(CarbonFile file, FontProcessingOptions options)
        {
            return FontProcessor.Process(file, options);
        }

        public ModelResourceGroup ProcessModel(ColladaInfo info, string element, CarbonDirectory texturePath)
        {
            if (info == null)
            {
                throw new ArgumentException();
            }

            return ColladaProcessor.Process(info, element, texturePath);
        }

        public StageResource ProcessStage(CarbonFile file, XcdProcessingOptions options)
        {
            return XcdProcessor.Process(file, options);
        }

        public ScriptResource ProcessScript(CarbonFile file, ScriptProcessingOptions options)
        {
            return ScriptProcessor.Process(file, options);
        }

        public UserInterfaceResource ProcessUserInterface(CarbonFile file, UserInterfaceProcessingOptions options)
        {
            return UserInterfaceProcessor.Process(file, options);
        }
    }
}
