namespace CarbonCore.Processing.Source.Generic.Data
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class FloatArrayType
    {
        [XmlText]
        public string RawData
        {
            get
            {
                return string.Empty;
            }

            set
            {
                this.Data = DataConversion.ConvertFloat(value);
            }
        }

        [XmlIgnore]
        public float[] Data { get; private set; }
    }
}
