namespace CarbonCore.Unity.Utils.Logic.Json
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    
    using CarbonCore.ContentServices.Logic.DataEntryLogic;
    using CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers;
    using CarbonCore.Unity.Utils.Data;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class ResourceKeySerializer : DataEntryElementSerializer
    {
        private static readonly Assembly UnityAssembly = Assembly.GetAssembly(typeof(UnityEngine.GameObject));

        private static ResourceKeySerializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static ResourceKeySerializer Instance
        {
            get
            {
                return instance ?? (instance = new ResourceKeySerializer());
            }
        }

        public void Serialize(Stream target, ResourceKey source)
        {
            if (source == default(ResourceKey))
            {
                target.WriteByte(0);
                return;
            }

            target.WriteByte(1);

            BundleKeySerializer.Instance.SerializeNullable(target, source.Bundle);
            StringSerializer.Instance.Serialize(target, source.Path);
            StringSerializer.Instance.Serialize(target, source.Type.FullName);
        }

        public void SerializeNullable(Stream target, ResourceKey? source)
        {
            if (source == null)
            {
                target.WriteByte(ContentServices.Constants.SerializationNull);
                return;
            }

            this.Serialize(target, source.Value);
        }

        public ResourceKey Deserialize(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == byte.MaxValue)
            {
                throw new InvalidOperationException();
            }

            if (indicator == 0)
            {
                return default(ResourceKey);
            }

            return this.DoDeserialize(source);
        }

        public ResourceKey? DeserializeNullable(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == byte.MaxValue)
            {
                return null;
            }

            if (indicator == 0)
            {
                return default(ResourceKey);
            }

            return this.DoDeserialize(source);
        }

        public override void SerializeImplicit(Stream target, object source)
        {
            this.SerializeNullable(target, (ResourceKey)source);
        }

        public override object DeserializeImplicit(Stream source)
        {
            return this.DeserializeNullable(source);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private ResourceKey DoDeserialize(Stream source)
        {
            BundleKey? bundle = BundleKeySerializer.Instance.DeserializeNullable(source);
            string path = StringSerializer.Instance.Deserialize(source);
            string typeString = StringSerializer.Instance.Deserialize(source);
            Type type = Type.GetType(typeString);
            if (type == null)
            {
                type = UnityAssembly.GetType(typeString);
            }

            if (bundle != null)
            {
                return new ResourceKey(bundle.Value, path, type);
            }

            return new ResourceKey(path, type);
        }
    }
}
