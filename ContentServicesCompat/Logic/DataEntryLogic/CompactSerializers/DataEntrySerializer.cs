﻿namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic.CompactSerializers
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
        
        public override void Serialize(Stream target, object value)
        {
            if (value == null)
            {
                target.WriteByte(0);
                return;
            }

            target.WriteByte(1);
            byte[] data = DataEntrySerialization.CompactSave((IDataEntry)value);

            byte[] length = BitConverter.GetBytes(data.Length);
            target.Write(length, 0, 4);
            target.Write(data, 0, data.Length);
        }

        public override object Deserialize(Stream source)
        {
            var indicator = source.ReadByte();
            if (indicator == Constants.SerializationNull || indicator == 0)
            {
                return null;
            }

            byte[] length = new byte[4];
            source.Read(length, 0, 4);

            byte[] data = new byte[BitConverter.ToInt32(length, 0)];
            source.Read(data, 0, data.Length);

            IDataEntry entry = DataEntrySerialization.CompactLoad(this.Type, data);
            return entry;
        }
    }
}
