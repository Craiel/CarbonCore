namespace CarbonCore.Processing.Source.Collada.Effect
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaMaterialLibrary
    {
        [XmlElement("material")]
        public ColladaMaterial[] Materials { get; set; }
    }
}
