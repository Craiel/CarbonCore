namespace CarbonCore.Processing.Source.Xcd.Scene
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class XcdMeta
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("content")]
        public string Content { get; set; }
    }
}
