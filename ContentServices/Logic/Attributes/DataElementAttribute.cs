namespace CarbonCore.ContentServices.Compat.Logic.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class DataElementAttribute : Attribute
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IgnoreClone { get; set; }

        public bool IgnoreSerialization { get; set; }

        public bool IgnoreEquality { get; set; }

        public bool IgnoreGetHashCode { get; set; }
    }
}
