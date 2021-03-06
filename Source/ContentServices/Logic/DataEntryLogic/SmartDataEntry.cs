﻿namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils;

    using Newtonsoft.Json;

    using NLog;

    [JsonObject(MemberSerialization.OptOut)]
    public abstract class SmartDataEntry : ISmartDataEntry
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private int? changeHashCode;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [JsonIgnore]
        public bool IsChanged
        {
            get
            {
                return !this.changeHashCode.HasValue ||
                        this.GetHashCode() != this.changeHashCode;
            }
        }

        public virtual IDataEntry Clone()
        {
            // Clone by serialization, gives more consistent results
            byte[] serialized = DataEntrySerialization.CompactSave(this);
            return DataEntrySerialization.CompactLoad(this.GetType(), serialized);
        }
        
        public virtual bool CopyFrom(IDataEntry source)
        {
            Debug.Assert(source != null);

            if (source.GetType() != this.GetType())
            {
                Logger.Error("Attempt to copy from different type, expected {0} but was {1}", this.GetType(), source.GetType());
                return false;
            }

            // This will clone the properties into ourselves, currently this does not consider any exclusion rules
            DataEntryDescriptor descriptor = DataEntryDescriptors.GetDescriptor(this.GetType());
            foreach (AttributedPropertyInfo<DataElementAttribute> info in descriptor.CloneableProperties)
            {
                info.SetValue(this, info.GetValue(source));
            }

            return false;
        }
        
        public override int GetHashCode()
        {
            DataEntryDescriptor descriptor = DataEntryDescriptors.GetDescriptor(this.GetType());
            if (descriptor.UseDefaultEquality)
            {
                // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
                return base.GetHashCode();
            }

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

            DataEntryDescriptor descriptor = DataEntryDescriptors.GetDescriptor(localType);
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

        public virtual void ResetChangedState()
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
#if UNITY_5
        protected abstract int DoGetHashCode();
#else
        protected virtual int DoGetHashCode()
        {
            Type localType = this.GetType();
            DataEntryDescriptor descriptor = DataEntryDescriptors.GetDescriptor(localType);

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
#endif
    }
}
