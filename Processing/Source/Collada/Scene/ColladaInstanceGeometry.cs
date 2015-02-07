namespace CarbonCore.Processing.Source.Collada.Scene
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaInstanceGeometry
    {
        [XmlAttribute("url")]
        public string Url { get; set; }

        [XmlElement("bind_material")]
        public ColladaBindMaterial BindMaterial { get; set; }
    }
}
