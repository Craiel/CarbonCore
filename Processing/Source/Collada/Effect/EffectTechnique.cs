namespace CarbonCore.Processing.Source.Collada.Effect
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class EffectTechnique
    {
        [XmlAttribute("profile")]
        public string Profile { get; set; }

        [XmlAttribute("sid")]
        public string SID { get; set; }

        [XmlElement("phong")]
        public EffectPhong Phong { get; set; }

        [XmlElement("blinn")]
        public EffectBlinn Blinn { get; set; }

        [XmlElement("lambert")]
        public EffectLambert Lambert { get; set; }

        [XmlElement("bump")]
        public EffectBump Bump { get; set; }

        [XmlElement("double_sided")]
        public EffectDoubleSided DoubleSided { get; set; }

        [XmlElement("extra")]
        public EffectExtra Extra { get; set; }
    }
}
