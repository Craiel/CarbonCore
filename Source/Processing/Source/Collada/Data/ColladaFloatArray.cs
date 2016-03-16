namespace CarbonCore.Processing.Source.Collada.Data
{
    using System;
    using System.Xml.Serialization;

    using CarbonCore.Processing.Source.Generic.Data;

    [Serializable]
    public class ColladaFloatArray : FloatArrayType
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("count")]
        public int Count { get; set; }
    }
}