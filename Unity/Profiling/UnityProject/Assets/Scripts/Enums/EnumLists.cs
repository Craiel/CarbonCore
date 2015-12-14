namespace Assets.Scripts.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
    public static class EnumLists
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static IList<Controls> Controls = Enum.GetValues(typeof(Controls)).Cast<Controls>().ToList();
    }
}
