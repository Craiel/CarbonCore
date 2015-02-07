namespace CarbonCore.Processing.Source.Collada.Scene
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaBindVertexInput
    {
        [XmlAttribute("semantic")]
        public string Semantic { get; set; }

        [XmlAttribute("input_semantic")]
        public string InputSemantic { get; set; }

        [XmlAttribute("input_set")]
        public string InputSet { get; set; }
    }
}
