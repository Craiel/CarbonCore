namespace CarbonCore.Processing.Source.Xcd.Scene
{
    using System;
    using System.Xml.Serialization;

    using CarbonCore.Processing.Source.Generic.Data;

    [Serializable]
    public class XcdCamera
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("position")]
        public FloatArrayType Position { get; set; }

        [XmlElement("rotation")]
        public FloatArrayType Rotation { get; set; }

        [XmlAttribute("fov")]
        public float FieldOfView { get; set; }

        [XmlElement(ElementName = "Layers")]
        public XcdLayerInfo LayerInfo { get; set; }

        [XmlElement(ElementName = "CustomProperties")]
        public XcdCustomProperties CustomProperties { get; set; }
    }
}
