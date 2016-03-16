namespace CarbonCore.Unity.Utils.Logic.Json
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    
    using CarbonCore.ContentServices.Logic.DataEntryLogic;
    using CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers;

    using UnityEngine;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class Vector3Serializer : DataEntryElementSerializer
    {
        private static Vector3Serializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static Vector3Serializer Instance
        {
            get
            {
                return instance ?? (instance = new Vector3Serializer());
            }
        }

        public void Serialize(Stream target, Vector3 source)
        {
            if (source == default(Vector3))
            {
                target.WriteByte(0);
                return;
            }

            target.WriteByte(1);

            FloatSerializer.Instance.Serialize(target, source.x);
            FloatSerializer.Instance.Serialize(target, source.y);
            FloatSerializer.Instance.Serialize(target, source.z);
        }

        public void SerializeNullable(Stream target, Vector3? source)
        {
            if (source == null)
            {
                target.WriteByte(ContentServices.Constants.SerializationNull);
                return;
            }

            this.Serialize(target, source.Value);
        }

        public Vector3 Deserialize(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == byte.MaxValue)
            {
                throw new InvalidOperationException();
            }

            if (indicator == 0)
            {
                return default(Vector3);
            }

            return this.DoDeserialize(source);
        }

        public Vector3? DeserializeNullable(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == byte.MaxValue)
            {
                return null;
            }

            if (indicator == 0)
            {
                return default(Vector3);
            }

            return this.DoDeserialize(source);
        }

        public override void SerializeImplicit(Stream target, object source)
        {
            this.SerializeNullable(target, (Vector3)source);
        }

        public override object DeserializeImplicit(Stream source)
        {
            return this.DeserializeNullable(source);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private Vector3 DoDeserialize(Stream source)
        {
            float x = FloatSerializer.Instance.Deserialize(source);
            float y = FloatSerializer.Instance.Deserialize(source);
            float z = FloatSerializer.Instance.Deserialize(source);

            return new Vector3(x, y, z);
        }
    }
}
