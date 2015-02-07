namespace CarbonCore.Processing.Source.Xcd.Scene
{
    using System;
    using System.Xml.Serialization;

    using CarbonCore.Processing.Source.Generic.Data;

    [Serializable]
    public class XcdLight
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("radius")]
        public float Radius { get; set; }

        [XmlAttribute("intensity")]
        public float Intensity { get; set; }

        [XmlAttribute("ambientintensity")]
        public float AmbientIntensity { get; set; }

        [XmlAttribute("spotsize")]
        public float SpotSize { get; set; }

        [XmlAttribute("angle")]
        public float Angle { get; set; }

        [XmlElement("location")]
        public FloatArrayType Location { get; set; }

        [XmlElement("direction")]
        public FloatArrayType Direction { get; set; }

        [XmlElement("color")]
        public FloatArrayType Color { get; set; }

        [XmlElement(ElementName = "layers")]
        public XcdLayerInfo LayerInfo { get; set; }

        [XmlElement(ElementName = "customproperties")]
        public XcdCustomProperties CustomProperties { get; set; }
    }
}
