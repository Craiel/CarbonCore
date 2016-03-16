namespace CarbonCore.Processing.Source.Collada.Effect
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class EffectLambert
    {
        [XmlElement("emission")]
        public EffectEmission Emission { get; set; }

        [XmlElement("ambient")]
        public EffectAmbient Ambient { get; set; }

        [XmlElement("diffuse")]
        public EffectDiffuse Diffuse { get; set; }

        [XmlElement("index_of_refraction")]
        public EffectIndexOfRefraction IndexOfRefraction { get; set; }
    }
}
