namespace CarbonCore.Processing.Source.Xcd.Scene
{
    using System;
    using System.Xml.Serialization;

    using CarbonCore.Processing.Source.Generic.Data;

    [Serializable]
    public class XcdBoundingBox
    {
        [XmlElement("point")]
        public FloatArrayType[] Points { get; set; }
    }
}
