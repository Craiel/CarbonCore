namespace CarbonCore.CodeGeneration
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
    public class Consts
    {
        public static string ItemPropertyType = "ItemType";

        public static string ItemTypeEmbeddedResource = "EmbeddedResource";
        public static string ItemTypeResource = "Resource";
    }
}
