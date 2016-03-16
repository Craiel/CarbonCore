namespace CarbonCore.Processing.Source.Collada.Effect
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class EffectDoubleSided
    {
        [XmlText]
        public string DoubleSided { get; set; }
    }
}
