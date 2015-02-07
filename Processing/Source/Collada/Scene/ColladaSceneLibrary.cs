namespace CarbonCore.Processing.Source.Collada.Scene
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaSceneLibrary
    {
        [XmlElement("visual_scene")]
        public ColladaVisualScene VisualScene { get; set; }
    }
}
