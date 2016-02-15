namespace CarbonCore.Utils.Unity.Logic.Json
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    
    using CarbonCore.ContentServices.Logic.DataEntryLogic;
    using CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers;

    using UnityEngine;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class QuaternionSerializer : DataEntryElementSerializer
    {
        private static QuaternionSerializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static QuaternionSerializer Instance
        {
            get
            {
                return instance ?? (instance = new QuaternionSerializer());
            }
        }

        public void Serialize(Stream target, Quaternion source)
        {
            if (source == default(Quaternion))
            {
                target.WriteByte(0);
                return;
            }

            target.WriteByte(1);

            FloatSerializer.Instance.Serialize(target, source.x);
            FloatSerializer.Instance.Serialize(target, source.y);
            FloatSerializer.Instance.Serialize(target, source.z);
            FloatSerializer.Instance.Serialize(target, source.w);
        }

        public void SerializeNullable(Stream target, Quaternion? source)
        {
            if (source == null)
            {
                target.WriteByte(ContentServices.Constants.SerializationNull);
                return;
            }

            this.Serialize(target, source.Value);
        }

        public Quaternion Deserialize(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == byte.MaxValue)
            {
                throw new InvalidOperationException();
            }

            if (indicator == 0)
            {
                return default(Quaternion);
            }

            return this.DoDeserialize(source);
        }

        public Quaternion? DeserializeNullable(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == byte.MaxValue)
            {
                return null;
            }

            if (indicator == 0)
            {
                return default(Quaternion);
            }

            return this.DoDeserialize(source);
        }

        public override void SerializeImplicit(Stream target, object source)
        {
            this.SerializeNullable(target, (Quaternion)source);
        }

        public override object DeserializeImplicit(Stream source)
        {
            return this.DeserializeNullable(source);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private Quaternion DoDeserialize(Stream source)
        {
            float x = FloatSerializer.Instance.Deserialize(source);
            float y = FloatSerializer.Instance.Deserialize(source);
            float z = FloatSerializer.Instance.Deserialize(source);
            float w = FloatSerializer.Instance.Deserialize(source);

            return new Quaternion(x, y, z, w);
        }
    }
}
