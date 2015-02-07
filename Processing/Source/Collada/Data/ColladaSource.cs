namespace CarbonCore.Processing.Source.Collada.Data
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaSource
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "float_array")]
        public ColladaFloatArray FloatArray { get; set; }
    }
}
