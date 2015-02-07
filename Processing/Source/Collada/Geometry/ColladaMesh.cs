namespace CarbonCore.Processing.Source.Collada.Geometry
{
    using System;
    using System.Xml.Serialization;

    using CarbonCore.Processing.Source.Collada.Data;

    [Serializable]
    public class ColladaMesh
    {
        [XmlElement(ElementName = "source")]
        public ColladaSource[] Sources { get; set; }

        [XmlElement(ElementName = "vertices")]
        public ColladaVertices Vertices { get; set; }

        [XmlElement(ElementName = "polylist")]
        public ColladaPolyList[] PolyLists { get; set; }

        [XmlElement(ElementName = "polygons")]
        public ColladaPolygons Polygons { get; set; }
    }
}
