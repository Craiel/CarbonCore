﻿namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Data;
    using System.Reflection;

    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils;
    using CarbonCore.Utils.Database;

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