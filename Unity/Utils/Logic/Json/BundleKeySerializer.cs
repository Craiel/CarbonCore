namespace CarbonCore.Utils.Unity.Logic.Json
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    
    using CarbonCore.ContentServices.Logic.DataEntryLogic;
    using CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers;
    using CarbonCore.Utils.Unity.Data;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class BundleKeySerializer : DataEntryElementSerializer
    {
        private static BundleKeySerializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static BundleKeySerializer Instance
        {
            get
            {
                return instance ?? (instance = new BundleKeySerializer());
            }
        }

        public void Serialize(Stream target, BundleKey source)
        {
            if (source == default(BundleKey))
            {
                target.WriteByte(0);
                return;
            }

            target.WriteByte(1);

            Int32Serializer.Instance.Serialize(target, source.BundleVersion);
            StringSerializer.Instance.Serialize(target, source.Bundle);
        }

        public void SerializeNullable(Stream target, BundleKey? source)
        {
            if (source == null)
            {
                target.WriteByte(ContentServices.Constants.SerializationNull);
                return;
            }

            this.Serialize(target, source.Value);
        }

        public BundleKey Deserialize(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == byte.MaxValue)
            {
                throw new InvalidOperationException();
            }

            if (indicator == 0)
            {
                return default(BundleKey);
            }

            return this.DoDeserialize(source);
        }

        public BundleKey? DeserializeNullable(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == byte.MaxValue)
            {
                return null;
            }

            if (indicator == 0)
            {
                return default(BundleKey);
            }

            return this.DoDeserialize(source);
        }

        public override void SerializeImplicit(Stream target, object source)
        {
            this.SerializeNullable(target, (BundleKey)source);
        }

        public override object DeserializeImplicit(Stream source)
        {
            return this.DeserializeNullable(source);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private BundleKey DoDeserialize(Stream source)
        {
            int bundleVersion = Int32Serializer.Instance.Deserialize(source);
            string bundle = StringSerializer.Instance.Deserialize(source);

            return new BundleKey(bundleVersion, bundle);
        }
    }
}
