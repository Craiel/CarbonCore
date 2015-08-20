﻿namespace CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using CarbonCore.ContentServices.Logic.DataEntryLogic;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class DoubleSerializer : DataEntryElementSerializer
    {
        private static DoubleSerializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static DoubleSerializer Instance
        {
            get
            {
                return instance ?? (instance = new DoubleSerializer());
            }
        }

        public override int MinSize
        {
            get
            {
                return 9;
            }
        }

        public int Serialize(Stream target, Double? source)
        {
            if (source == null)
            {
                target.WriteByte(byte.MaxValue);
                return 1;
            }

            return this.Serialize(target, source.Value);
        }

        public int Serialize(Stream target, Double source)
        {
            if (Math.Abs(source - default(Double)) < double.Epsilon)
            {
                target.WriteByte(0);
                return 1;
            }

            target.WriteByte(1);

            byte[] data = BitConverter.GetBytes(source);

            target.Write(data, 0, 8);
            return this.MinSize;
        }

        public override int Serialize(Stream target, object source)
        {
            return this.Serialize(target, (Double)source);
        }

        public override object Deserialize(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == byte.MaxValue)
            {
                return null;
            }

            if (indicator == 0)
            {
                return default(double);
            }

            byte[] data = new byte[8];
            source.Read(data, 0, 8);

            return BitConverter.ToDouble(data, 0);
        }
    }
}
