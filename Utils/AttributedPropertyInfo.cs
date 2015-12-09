namespace CarbonCore.Utils
{
    using System;
    using System.Reflection;

    public delegate object Foo(object other);

    public class AttributedPropertyInfo<T>
        where T : Attribute
    {
        private readonly MethodInfo getter;
        private readonly MethodInfo setter;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public AttributedPropertyInfo(Type host, T attribute, PropertyInfo property)
        {
            this.Host = host;
            this.Attribute = attribute;

            this.PropertyName = property.Name;
            this.PropertyType = property.PropertyType;

            if (property.CanRead)
            {
                this.getter = property.GetGetMethod();
            }

            if (property.CanWrite)
            {
                this.setter = property.GetSetMethod();
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Host { get; private set; }

        public T Attribute { get; private set; }

        public string PropertyName { get; private set; }

        public Type PropertyType { get; private set; }

        public object GetValue(object target)
        {
            return this.getter.Invoke(target, null);
        }

        public void SetValue(object target, object value)
        {
            if (this.setter == null)
            {
                throw new InvalidOperationException();
            }

            this.setter.Invoke(target, new[] { value });
        }
    }
}
