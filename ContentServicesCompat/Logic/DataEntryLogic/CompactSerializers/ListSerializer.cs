namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic.CompactSerializers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class ListSerializer : DataEntryElementSerializer
    {
        public static readonly Type BaseListType = typeof(List<>);
        
        private readonly DataEntryElementSerializer innerSerializer;

        private readonly MethodInfo addMethod;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ListSerializer(Type innerType, DataEntryElementSerializer inner)
        {
            this.innerSerializer = inner;

            this.Type = BaseListType.MakeGenericType(innerType);

            this.addMethod = this.Type.GetMethod("Add");
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }

        public override void Serialize(Stream target, object value)
        {
            if (value == null)
            {
                target.WriteByte(Constants.SerializationNull);
                return;
            }

            var typed = (ICollection)value;
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
                this.innerSerializer.Serialize(target, entry);
            }
        }

        public override object Deserialize(Stream source)
        {
            var indicator = source.ReadByte();
            if (indicator == Constants.SerializationNull)
            {
                return null;
            }

            object instance = Activator.CreateInstance(this.Type);
            if (indicator == 0)
            {
                return instance;
            }

            byte[] length = new byte[2];
            source.Read(length, 0, length.Length);

            short count = BitConverter.ToInt16(length, 0);
            for (var i = 0; i < count; i++)
            {
                object entry = this.innerSerializer.Deserialize(source);
                this.addMethod.Invoke(instance, new[] { entry });
            }

            return instance;
        }
    }
}
