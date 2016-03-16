namespace CarbonCore.Processing.Source.Collada.General
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaImageLibrary
    {
        [XmlElement(ElementName = "image")]
        public ColladaImage[] Images { get; set; }
    }
}
