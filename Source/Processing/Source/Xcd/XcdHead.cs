namespace CarbonCore.Processing.Source.Xcd
{
    using System;
    using System.Xml.Serialization;

    using CarbonCore.Processing.Source.Xcd.Scene;

    [Serializable]
    public class XcdHead
    {
        [XmlElement(ElementName = "meta")]
        public XcdMeta[] Metadata { get; set; }
    }
}
