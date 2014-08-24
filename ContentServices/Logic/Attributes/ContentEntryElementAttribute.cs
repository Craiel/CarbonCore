namespace CarbonCore.ContentServices.Logic.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class ContentEntryElementAttribute : Attribute
    {
        public bool IgnoreClone { get; set; }
        public bool IgnoreEquality { get; set; }
    }
}
