namespace CarbonCore.Processing.Source.Generic.Data
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class IntArrayType
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
                this.Data = DataConversion.ConvertInt(value);
            }
        }

        [XmlIgnore]
        public int[] Data { get; private set; }
    }
}
