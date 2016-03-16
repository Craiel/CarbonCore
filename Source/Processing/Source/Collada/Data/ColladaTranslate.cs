namespace CarbonCore.Processing.Source.Collada.Data
{
    using System;
    using System.Xml.Serialization;

    using CarbonCore.Processing.Source.Generic.Data;

    [Serializable]
    public class ColladaTranslate : FloatArrayType
    {
        [XmlAttribute("sid")]
        public string Sid { get; set; }
    }
}
