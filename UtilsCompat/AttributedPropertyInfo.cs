namespace CarbonCore.Utils.Compat
{
    using System;
    using System.Reflection;

    public class AttributedPropertyInfo<T>
        where T : Attribute
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public AttributedPropertyInfo(Type host, T attribute, PropertyInfo property)
        {
            this.Host = host;
            this.Attribute = attribute;
            this.Property = property;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Host { get; private set; }

        public T Attribute { get; private set; }

        public PropertyInfo Property { get; private set; }
    }
}
