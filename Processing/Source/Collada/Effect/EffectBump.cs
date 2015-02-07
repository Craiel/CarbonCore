namespace CarbonCore.Processing.Source.Collada.Effect
{
    using System;
    using System.Xml.Serialization;

    using CarbonCore.Processing.Source.Collada.Data;

    [Serializable]
    public class EffectBump
    {
        [XmlElement("texture")]
        public ColladaTexture Texture { get; set; }
    }
}
