namespace CarbonCore.Processing.Source.Collada.Geometry
{
    using System;
    using System.Xml.Serialization;

    using CarbonCore.Processing.Source.Collada.Data;
    using CarbonCore.Processing.Source.Generic.Data;

    [Serializable]
    public class ColladaPolygons
    {
        [XmlAttribute(AttributeName = "material")]
        public string Material { get; set; }

        [XmlAttribute(AttributeName = "count")]
        public int Count { get; set; }

        [XmlElement(ElementName = "input")]
        public ColladaInput[] Inputs { get; set; }

        [XmlElement(ElementName = "p")]
        public IntArrayType[] Polygons { get; set; }
    }
}
