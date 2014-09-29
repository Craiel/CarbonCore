namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.IO;
    using System.Reflection;

    using CarbonCore.ContentServices.Contracts;

    public abstract class ContentEntry : IContentEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IContentEntry Clone()
        {
            Type localType = this.GetType();
            var clone = (IContentEntry)Activator.CreateInstance(localType, null);
            ContentEntryDescriptor descriptor = ContentEntryDescriptor.GetDescriptor(localType);
            foreach (PropertyInfo propertyInfo in descriptor.ClonableProperties)
            {
                object localValue = propertyInfo.GetValue(this);
                propertyInfo.SetValue(clone, localValue);
            }

            return clone;
        }

        public virtual bool CopyFrom(IContentEntry source)
        {
            System.Diagnostics.Trace.Assert(source != null);

            if (source.GetType() != this.GetType())
            {
                System.Diagnostics.Trace.TraceError("Attempt to copy from different type, expected {0} but was {1}", this.GetType(), source.GetType());
                return false;
            }

            // This will clone the properties into ourselves, currently this does not consider any exclusion rules
            PropertyInfo[] propertyInfos = source.GetType().GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                info.SetValue(this, info.GetValue(source));
            }

            return false;
        }

        public virtual bool Load(Stream source)
        {
            System.Diagnostics.Trace.TraceError("Content entry of type {0} is not implementing Load!", this.GetType());

            return false;
        }

        public virtual bool Save(Stream target)
        {
            System.Diagnostics.Trace.TraceError("Content entry of type {0} is not implementing Save!", this.GetType());

            return false;
        }

        public override int GetHashCode()
        {
            return this.DoGetHashCode();
        }

        public override bool Equals(object obj)
        {
            Type localType = this.GetType();
            var other = obj as IContentEntry;
            if (other == null || other.GetType() != localType)
            {
                return false;
            }

            ContentEntryDescriptor descriptor = ContentEntryDescriptor.GetDescriptor(localType);
            foreach (PropertyInfo propertyInfo in descriptor.EqualityProperties)
            {
                object localValue = propertyInfo.GetValue(this);
                object otherValue = propertyInfo.GetValue(other);
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

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected abstract int DoGetHashCode();
    }
}
