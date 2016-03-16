namespace CarbonCore.Processing.Contracts
{
    using CarbonCore.Processing.Processors;
    using CarbonCore.Processing.Resource;
    using CarbonCore.Processing.Resource.Model;
    using CarbonCore.Processing.Resource.Stage;
    using CarbonCore.Processing.Source.Collada;
    using CarbonCore.Processing.Source.Xcd;
    using CarbonCore.Utils.IO;

    public interface IResourceProcessor
    {
        CarbonDirectory TextureToolsPath { get; set; }

        RawResource ProcessRaw(CarbonDirectory path);

        RawResource ProcessRaw(CarbonFile file);
        RawResource ProcessTexture(CarbonFile file, TextureProcessingOptions options);
        RawResource ProcessFont(CarbonFile file, FontProcessingOptions options);
        StageResource ProcessStage(CarbonFile file, XcdProcessingOptions options);
        ModelResourceGroup ProcessModel(ColladaInfo info, string element, CarbonDirectory texturePath);
        ScriptResource ProcessScript(CarbonFile file, ScriptProcessingOptions options);
        UserInterfaceResource ProcessUserInterface(CarbonFile file, UserInterfaceProcessingOptions options);
    }
}
