namespace CarbonCore.Processing.Source.Xcd
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot(ElementName = "xcd", IsNullable = false)]
    public class Xcd
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(Xcd));

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlElement(ElementName = "head")]
        public XcdHead Head { get; set; }

        [XmlElement(ElementName = "scene")]
        public XcdScene Scene { get; set; }

        public static Xcd Load(byte[] data)
        {
            using (var dataStream = new MemoryStream(data))
            {
                return Serializer.Deserialize(dataStream) as Xcd;
            }
        }

        public static Xcd Load(Stream source)
        {
            return Serializer.Deserialize(source) as Xcd;
        }
    }
}
