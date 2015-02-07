namespace CarbonCore.Processing.Source.Collada.Data
{
    using System.Xml.Serialization;

    public class ColladaInput
    {
        [XmlAttribute("semantic")]
        public string Semantic { get; set; }

        [XmlAttribute("source")]
        public string Source { get; set; }

        [XmlAttribute("offset")]
        public int Offset { get; set; }

        [XmlAttribute("set")]
        public int Set { get; set; }
    }
}
