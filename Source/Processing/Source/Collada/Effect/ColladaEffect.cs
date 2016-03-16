namespace CarbonCore.Processing.Source.Collada.Effect
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaEffect
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("profile_COMMON")]
        public ProfileCommon ProfileCommon { get; set; }
    }
}
