namespace CarbonCore.Processing.Source.Collada.Geometry
{
    using System;
    using System.Xml.Serialization;

    using CarbonCore.Processing.Source.Collada.Data;

    [Serializable]
    public class ColladaVertices
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "input")]
        public ColladaInput Input { get; set; }
    }
}
