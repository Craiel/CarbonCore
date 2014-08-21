namespace CarbonCore.ContentServices.Logic
{
    using System.Reflection;

    public class ContentReflectionProperty
    {
        public ContentReflectionProperty(ContentEntryElementAttribute attribute, PropertyInfo info)
        {
            this.Name = attribute.Name ?? info.Name;
            this.Info = info;
        }

        public string Name { get; private set; }
        public PropertyInfo Info { get; private set; }
        public PrimaryKeyMode PrimaryKey { get; set; }
    }
}
