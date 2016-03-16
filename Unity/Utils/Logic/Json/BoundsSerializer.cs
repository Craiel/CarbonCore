namespace CarbonCore.Unity.Utils.Logic.Json
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    
    using CarbonCore.ContentServices.Logic.DataEntryLogic;

    using UnityEngine;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class BoundsSerializer : DataEntryElementSerializer
    {
        private static BoundsSerializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static BoundsSerializer Instance
        {
            get
            {
                return instance ?? (instance = new BoundsSerializer());
            }
        }

        public void Serialize(Stream target, Bounds source)
        {
            if (source == default(Bounds))
            {
                target.WriteByte(0);
                return;
            }

            target.WriteByte(1);

            Vector3Serializer.Instance.Serialize(target, source.center);
            Vector3Serializer.Instance.Serialize(target, source.size);
        }

        public void SerializeNullable(Stream target, Bounds? source)
        {
            if (source == null)
            {
                target.WriteByte(ContentServices.Constants.SerializationNull);
                return;
            }

            this.Serialize(target, source.Value);
        }

        public Bounds Deserialize(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == byte.MaxValue)
            {
                throw new InvalidOperationException();
            }

            if (indicator == 0)
            {
                return default(Bounds);
            }

            return this.DoDeserialize(source);
        }

        public Bounds? DeserializeNullable(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == byte.MaxValue)
            {
                return null;
            }

            if (indicator == 0)
            {
                return default(Bounds);
            }

            return this.DoDeserialize(source);
        }

        public override void SerializeImplicit(Stream target, object source)
        {
            this.SerializeNullable(target, (Bounds)source);
        }

        public override object DeserializeImplicit(Stream source)
        {
            return this.DeserializeNullable(source);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private Bounds DoDeserialize(Stream source)
        {
            Vector3 center = Vector3Serializer.Instance.Deserialize(source);
            Vector3 size = Vector3Serializer.Instance.Deserialize(source);

            return new Bounds(center, size);
        }
    }
}
