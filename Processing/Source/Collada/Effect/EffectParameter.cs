namespace CarbonCore.Processing.Source.Collada.Effect
{
    using System;
    using System.Xml.Serialization;

    using CarbonCore.Processing.Source.Collada.Data;

    [Serializable]
    public class EffectParameter
    {
        [XmlAttribute(AttributeName = "sid")]
        public string Sid { get; set; }

        [XmlElement(ElementName = "sampler2D")]
        public ColladaSampler2D Sampler2D { get; set; }

        [XmlElement(ElementName = "surface")]
        public ColladaSurface Surface { get; set; }
    }
}
