namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Reflection;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.Attributes;

    public class ContentEntry : IContentEntry
    {
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

        public override int GetHashCode()
        {
            throw new NotImplementedException();
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
    }
}
