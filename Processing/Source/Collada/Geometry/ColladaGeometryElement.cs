namespace CarbonCore.Processing.Source.Collada.Geometry
{
    using System;
    using System.Xml.Serialization;

    using CarbonCore.Processing.Source.Collada.Data;
    using CarbonCore.Processing.Source.Generic.Data;

    [Serializable]
    public abstract class ColladaGeometryElement
    {
        [XmlAttribute("material")]
        public string Material { get; set; }

        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlElement(ElementName = "input")]
        public ColladaInput[] Inputs { get; set; }

        // Todo: Rename this with something more reasonable
        [XmlElement(ElementName = "p")]
        public IntArrayType P { get; set; }
    }
}
