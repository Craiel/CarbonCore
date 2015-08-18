namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils.Compat;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public abstract class DataEntry : IDataEntry
    {
        private int changeHashCode;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected DataEntry()
        {
            this.ResetChangedState();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsChanged
        {
            get
            {
                return this.GetHashCode() != this.changeHashCode;
            }
        }

        public virtual IDataEntry Clone()
        {
            Type localType = this.GetType();
            var clone = (IDataEntry)Activator.CreateInstance(localType, null);
            DataEntryDescriptor descriptor = DataEntryDescriptor.GetDescriptor(localType);
            foreach (AttributedPropertyInfo<DataElementAttribute> info in descriptor.CloneableProperties)
            {
                object localValue = info.GetValue(this);
                info.SetValue(clone, localValue);
            }

            return clone;
        }

        public virtual bool Save(Stream target)
        {
            System.Diagnostics.Trace.TraceError("Stream Save is not implemented for {0}", this.GetType());
            return false;
        }

        public virtual bool Load(Stream source)
        {
            System.Diagnostics.Trace.TraceError("Stream Load is not implemented for {0}", this.GetType());
            return false;
        }

        public virtual bool CopyFrom(IDataEntry source)
        {
            System.Diagnostics.Trace.Assert(source != null);

            if (source.GetType() != this.GetType())
            {
                System.Diagnostics.Trace.TraceError("Attempt to copy from different type, expected {0} but was {1}", this.GetType(), source.GetType());
                return false;
            }

            // This will clone the properties into ourselves, currently this does not consider any exclusion rules
            DataEntryDescriptor descriptor = DataEntryDescriptor.GetDescriptor(this.GetType());
            foreach (AttributedPropertyInfo<DataElementAttribute> info in descriptor.CloneableProperties)
            {
                info.SetValue(this, info.GetValue(source));
            }

            return false;
        }
        
        public override int GetHashCode()
        {
            return this.DoGetHashCode();
        }

        public override bool Equals(object obj)
        {
            Type localType = this.GetType();
            var other = obj as IDataEntry;
            if (other == null || other.GetType() != localType)
            {
                return false;
            }

            DataEntryDescriptor descriptor = DataEntryDescriptor.GetDescriptor(localType);
            if (descriptor.UseDefaultEquality)
            {
                // ReSharper disable once BaseObjectEqualsIsObjectEquals
                return base.Equals(obj);
            }

            foreach (AttributedPropertyInfo<DataElementAttribute> info in descriptor.EqualityProperties)
            {
                object localValue = info.GetValue(this);
                object otherValue = info.GetValue(other);
                if (localValue == null && otherValue == null)
                {
                    continue;
                }

                if (localValue == null || otherValue == null)
                {
                    return false;
                }

                if (localValue.Equals(otherValue))
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        public void ResetChangedState()
        {
            this.changeHashCode = this.GetHashCode();
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            this.ResetChangedState();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual int DoGetHashCode()
        {
            Type localType = this.GetType();
            DataEntryDescriptor descriptor = DataEntryDescriptor.GetDescriptor(localType);

            if (descriptor.UseDefaultEquality)
            {
                // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
                return base.GetHashCode();
            }

            object[] values = new object[descriptor.HashableProperties.Count];
            for (var i = 0; i < descriptor.HashableProperties.Count; i++)
            {
                values[i] = descriptor.HashableProperties[i].GetValue(this);
            }

            return HashUtils.CombineObjectHashes(values);
        }
    }
}
