namespace CarbonCore.Utils.IoC
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependsOnModuleAttribute : Attribute
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DependsOnModuleAttribute(Type type)
        {
            this.Type = type;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }
    }
}
