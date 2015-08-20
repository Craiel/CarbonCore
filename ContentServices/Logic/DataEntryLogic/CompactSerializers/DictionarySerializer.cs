namespace CarbonCore.ContentServices.Logic.DataEntryLogic.CompactSerializers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class DictionarySerializer : DataEntryElementSerializer
    {
        public static readonly Type BaseDictionaryType = typeof(Dictionary<,>);
        public static readonly Type BaseKeyValuePairType = typeof(KeyValuePair<,>);
        
        private readonly DataEntryElementSerializer keySerializer;
        private readonly DataEntryElementSerializer valueSerializer;

        private readonly PropertyInfo keyProperty;
        private readonly PropertyInfo valueProperty;

        private readonly MethodInfo addMethod;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DictionarySerializer(Type keyType, Type valueType, DataEntryElementSerializer key, DataEntryElementSerializer value)
        {
            this.keySerializer = key;
            this.valueSerializer = value;

            this.Type = BaseDictionaryType.MakeGenericType(keyType, valueType);
            this.KeyValueType = BaseKeyValuePairType.MakeGenericType(keyType, valueType);

            this.addMethod = this.Type.GetMethod("Add");

            this.keyProperty = this.KeyValueType.GetProperty("Key");
            this.valueProperty = this.KeyValueType.GetProperty("Value");
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }

        public Type KeyValueType { get; private set; }

        public override void Serialize(Stream target, object value)
        {
            if (value == null)
            {
                target.WriteByte(0);
                return;
            }

            var typed = (IDictionary)value;
            if (typed.Count <= 0)
            {
                target.WriteByte(0);
                return;
            }
            
            target.WriteByte(1);

            // Count of elements
            byte[] length = BitConverter.GetBytes((Int16)typed.Count);
            target.Write(length, 0, 2);

            foreach (object entry in (IEnumerable)value)
            {
                this.keySerializer.Serialize(target, this.keyProperty.GetValue(entry));
                this.valueSerializer.Serialize(target, this.valueProperty.GetValue(entry));
            }
        }

        public override object Deserialize(Stream source)
        {
            var indicator = source.ReadByte();
            if (indicator == byte.MaxValue)
            {
                return null;
            }

            object instance = Activator.CreateInstance(this.Type);

            byte[] length = new byte[2];
            source.Read(length, 0, length.Length);

            short count = BitConverter.ToInt16(length, 0);
            for (var i = 0; i < count; i++)
            {
                object key = this.keySerializer.Deserialize(source);
                object value = this.valueSerializer.Deserialize(source);
                this.addMethod.Invoke(instance, new[] { key, value });
            }

            return instance;
        }
    }
}
