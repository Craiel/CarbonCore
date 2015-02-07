namespace CarbonCore.Processing.Source.Xcd
{
    using System;
    using System.Xml.Serialization;

    using CarbonCore.Processing.Source.Xcd.Scene;

    [Serializable]
    public class XcdScene
    {
        [XmlElement(ElementName = "camera")]
        public XcdCamera[] Cameras { get; set; }

        [XmlElement(ElementName = "light")]
        public XcdLight[] Lights { get; set; }

        [XmlElement(ElementName = "element")]
        public XcdElement[] Elements { get; set; }
    }
}
