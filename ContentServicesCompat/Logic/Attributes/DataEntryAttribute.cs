namespace CarbonCore.ContentServices.Compat.Logic.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class DataEntryAttribute : Attribute
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool UseDefaultEquality { get; set; }

        public bool OptOutAttributes { get; set; }
    }
}
