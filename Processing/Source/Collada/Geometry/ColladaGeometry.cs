namespace CarbonCore.Processing.Source.Collada.Geometry
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaGeometry
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "mesh")]
        public ColladaMesh Mesh { get; set; }
    }
}
