namespace CarbonCore.Processing.Source.Collada.Effect
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaMaterial
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("instance_effect")]
        public InstanceEffect Effect { get; set; }
    }
}
