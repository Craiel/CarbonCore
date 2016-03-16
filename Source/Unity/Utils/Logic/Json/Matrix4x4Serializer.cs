namespace CarbonCore.Unity.Utils.Logic.Json
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    
    using CarbonCore.ContentServices.Logic.DataEntryLogic;
    using CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers;

    using UnityEngine;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class Matrix4X4Serializer : DataEntryElementSerializer
    {
        private static Matrix4X4Serializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static Matrix4X4Serializer Instance
        {
            get
            {
                return instance ?? (instance = new Matrix4X4Serializer());
            }
        }

        public void Serialize(Stream target, Matrix4x4 source)
        {
            if (source == default(Matrix4x4))
            {
                target.WriteByte(0);
                return;
            }

            target.WriteByte(1);
            for (var i = 0; i < 16; i++)
            {
                FloatSerializer.Instance.Serialize(target, source[i]);
            }
        }

        public void SerializeNullable(Stream target, Matrix4x4? source)
        {
            if (source == null)
            {
                target.WriteByte(ContentServices.Constants.SerializationNull);
                return;
            }

            this.Serialize(target, source.Value);
        }

        public Matrix4x4 Deserialize(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == byte.MaxValue)
            {
                throw new InvalidOperationException();
            }

            if (indicator == 0)
            {
                return default(Matrix4x4);
            }

            return this.DoDeserialize(source);
        }

        public Matrix4x4? DeserializeNullable(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == byte.MaxValue)
            {
                return null;
            }

            if (indicator == 0)
            {
                return default(Matrix4x4);
            }

            return this.DoDeserialize(source);
        }

        public override void SerializeImplicit(Stream target, object source)
        {
            this.SerializeNullable(target, (Matrix4x4)source);
        }

        public override object DeserializeImplicit(Stream source)
        {
            return this.DeserializeNullable(source);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private Matrix4x4 DoDeserialize(Stream source)
        {
            var matrix = new Matrix4x4();
            for (var i = 0; i < 16; i++)
            {
                matrix[i] = FloatSerializer.Instance.Deserialize(source);
            }

            return matrix;
        }
    }
}
