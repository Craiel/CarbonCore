namespace CarbonCore.Processing.Source.Generic.Data
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class BoolArrayType
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
                this.Data = DataConversion.ConvertBool(value);
            }
        }

        [XmlIgnore]
        public bool[] Data { get; private set; }
    }
}
