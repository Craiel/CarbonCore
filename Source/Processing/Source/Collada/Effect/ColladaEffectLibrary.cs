namespace CarbonCore.Processing.Source.Collada.Effect
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaEffectLibrary
    {
        [XmlElement(ElementName = "effect")]
        public ColladaEffect[] Effects { get; set; }
    }
}
