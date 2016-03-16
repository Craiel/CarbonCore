namespace CarbonCore.Processing.Source.Collada.Data
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaSurface
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlElement(ElementName = "init_from")]
        public ColladaInitFrom InitFrom { get; set; }
    }
}
