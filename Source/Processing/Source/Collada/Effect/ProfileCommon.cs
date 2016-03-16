namespace CarbonCore.Processing.Source.Collada.Effect
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ProfileCommon
    {
        [XmlElement("newparam")]
        public EffectParameter[] Parameter { get; set; }

        [XmlElement("technique")]
        public EffectTechnique Technique { get; set; }

        [XmlElement("extra")]
        public EffectExtra Extra { get; set; }
    }
}
