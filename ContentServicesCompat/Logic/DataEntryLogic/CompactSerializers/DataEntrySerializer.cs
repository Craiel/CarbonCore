namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic.CompactSerializers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using CarbonCore.ContentServices.Compat.Contracts;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class DataEntrySerializer : DataEntryElementSerializer
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DataEntrySerializer(Type type)
        {
            this.Type = type;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }

        public void Serialize<T>(Stream target, T value)
            where T : IDataEntry
        {
            if (value == null)
            {
                target.WriteByte(Constants.SerializationNull);
                return;
            }

            target.WriteByte(1);
            byte[] data = DataEntrySerialization.CompactSave(value);

            byte[] length = BitConverter.GetBytes(data.Length);
            target.Write(length, 0, 4);
            target.Write(data, 0, data.Length);
        }

        public T Deserialize<T>(Stream source)
            where T : class, IDataEntry
        {
            byte[] data = this.DeserializeBytes(source);
            if (data == null)
            {
                return null;
            }

            T entry = DataEntrySerialization.CompactLoad<T>(data);
            return entry;
        }

        public override void SerializeImplicit(Stream target, object value)
        {
            this.Serialize(target, value as IDataEntry);
        }

        public override object DeserializeImplicit(Stream source)
        {
            byte[] data = this.DeserializeBytes(source);
            if (data == null)
            {
                return null;
            }

            IDataEntry entry = DataEntrySerialization.CompactLoad(this.Type, data);
            return entry;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private byte[] DeserializeBytes(Stream source)
        {
            var indicator = source.ReadByte();
            if (indicator == Constants.SerializationNull)
            {
                return null;
            }

            byte[] length = new byte[4];
            source.Read(length, 0, 4);

            byte[] data = new byte[BitConverter.ToInt32(length, 0)];
            source.Read(data, 0, data.Length);
            return data;
        }
    }
}
