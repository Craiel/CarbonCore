namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils;

    public class DatabaseEntryDescriptor
    {
        public static readonly IDictionary<Type, DatabaseEntryDescriptor> Descriptors = new Dictionary<Type, DatabaseEntryDescriptor>();

        private readonly IDictionary<string, DatabaseEntryElementDescriptor> elementNameLookup;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseEntryDescriptor(Type targetType)
        {
            System.Diagnostics.Trace.Assert(typeof(IDatabaseEntry).IsAssignableFrom(targetType));

            this.Type = targetType;

            this.Elements = new List<DatabaseEntryElementDescriptor>();

            this.elementNameLookup = new Dictionary<string, DatabaseEntryElementDescriptor>();

            this.Analyze();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }

        public string TableName { get; private set; }
        
        public DatabaseEntryAttribute EntryAttribute { get; private set; }

        public DatabaseEntryElementDescriptor PrimaryKey { get; private set; }

        public IList<DatabaseEntryElementDescriptor> Elements { get; private set; }

        public static DatabaseEntryDescriptor GetDescriptor<T>()
        {
            return GetDescriptor(typeof(T));
        }

        public static DatabaseEntryDescriptor GetDescriptor(Type type)
        {
            if (!Descriptors.ContainsKey(type))
            {
                Descriptors.Add(type, new DatabaseEntryDescriptor(type));
            }

            return Descriptors[type];
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as DatabaseEntryDescriptor;
            if (other == null)
            {
                return false;
            }

            return other.Type == this.Type;
        }

        public DatabaseEntryElementDescriptor GetElementByName(string name)
        {
            if (this.elementNameLookup.ContainsKey(name))
            {
                return this.elementNameLookup[name];
            }

            return null;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Analyze()
        {
            this.PrimaryKey = null;
            this.Elements.Clear();

            IEnumerable<Attribute> typeAttributes = this.Type.GetCustomAttributes();
            foreach (Attribute attribute in typeAttributes)
            {
                var typed = attribute as DatabaseEntryAttribute;
                if (typed != null)
                {
                    this.EntryAttribute = typed;
                    break;
                }
            }

            System.Diagnostics.Trace.Assert(this.EntryAttribute != null, "The Main attribute is not defined");
            this.TableName = this.EntryAttribute.Table;

            PropertyInfo[] properties = this.Type.GetProperties();
            foreach (PropertyInfo info in properties)
            {
                object[] attributes = info.GetCustomAttributes(true);
                foreach (object attribute in attributes)
                {
                    Type attributeType = attribute.GetType();
                    DatabaseEntryElementDescriptor element = null;

                    if (attributeType == typeof(DatabaseEntryElementAttribute))
                    {
                        var typed = (DatabaseEntryElementAttribute)attribute;

                        element = new DatabaseEntryElementDescriptor(this.Type, (DatabaseEntryElementAttribute)attribute, info);
                        this.elementNameLookup.Add(element.Name, element);

                        if (typed.PrimaryKeyMode != PrimaryKeyMode.None)
                        {
                            if (this.PrimaryKey != null)
                            {
                                throw new InvalidDataException(
                                    "Primary key was already defined, multiple is not supported yet!");
                            }

                            if (!info.PropertyType.IsNullable())
                            {
                                throw new InvalidDataException(string.Format("Primary key type needs to be nullable: {0} on {1}", info.Name, this.Type));
                            }
                            this.PrimaryKey = element;
                        }
                    }
                        
                    if (element != null)
                    {
                        this.Elements.Add(element);
                    }
                }
            }

            if (this.PrimaryKey == null)
            {
                System.Diagnostics.Trace.TraceWarning("DatabaseEntry has no Primary key defined: {0}", this.Type);
            }
        }
    }
}
