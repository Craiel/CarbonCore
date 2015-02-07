namespace CarbonCore.Processing.Source.Xcd.Scene
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class XcdCustomProperties
    {
        [XmlElement(ElementName = "property")]
        public XcdProperty[] Properties { get; set; }
    }
}
