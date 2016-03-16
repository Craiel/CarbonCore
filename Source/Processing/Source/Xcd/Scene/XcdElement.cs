namespace CarbonCore.Processing.Source.Xcd.Scene
{
    using System;
    using System.Xml.Serialization;

    using CarbonCore.Processing.Source.Generic.Data;

    [Serializable]
    public class XcdElement
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("link")]
        public string Reference { get; set; }

        [XmlElement("translation")]
        public FloatArrayType Translation { get; set; }

        [XmlElement("rotation")]
        public FloatArrayType Rotation { get; set; }

        [XmlElement("scale")]
        public FloatArrayType Scale { get; set; }

        [XmlElement(ElementName = "boundingbox")]
        public XcdBoundingBox BoundingBox { get; set; }

        [XmlElement(ElementName = "layers")]
        public XcdLayerInfo LayerInfo { get; set; }

        [XmlElement(ElementName = "customproperties")]
        public XcdCustomProperties CustomProperties { get; set; }

        [XmlElement(ElementName = "element")]
        public XcdElement[] Elements { get; set; }
    }
}
