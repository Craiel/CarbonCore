namespace CarbonCore.ContentServices.Sql.Logic
{
    using System;
    using System.Data;
    using System.Reflection;
    
    using CarbonCore.ContentServices.Sql.Logic.Attributes;
    using CarbonCore.Utils;

    public class DatabaseEntryElementDescriptor : AttributedPropertyInfo<DatabaseEntryElementAttribute>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseEntryElementDescriptor(Type host, DatabaseEntryElementAttribute attribute, PropertyInfo property)
            : base(host, attribute, property)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name
        {
            get
            {
                return this.Attribute.Name ?? this.PropertyName;
            }
        }

        public Type InternalType
        {
            get
            {
                return this.PropertyType;
            }
        }

        public SqlDbType DatabaseType
        {
            get
            {
                return DatabaseUtils.GetDatabaseType(this.PropertyType);
            }
        }
    }
}
