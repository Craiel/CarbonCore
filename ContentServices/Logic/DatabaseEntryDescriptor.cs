namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils;
    using CarbonCore.Utils.Diagnostics;

    public class DatabaseEntryDescriptor
    {
        public static readonly IDictionary<Type, DatabaseEntryDescriptor> Descriptors = new Dictionary<Type, DatabaseEntryDescriptor>();

        private readonly IDictionary<string, DatabaseEntryElementDescriptor> elementNameLookup;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseEntryDescriptor(Type targetType)
        {
            Diagnostic.Assert(typeof(IDatabaseEntry).IsAssignableFrom(targetType));

            this.Type = targetType;

            this.Elements = new List<DatabaseEntryElementDescriptor>();
            this.JoinedElements = new List<DatabaseEntryJoinedElementDescriptor>();

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

        public IList<DatabaseEntryJoinedElementDescriptor> JoinedElements { get; private set; }

        public static DatabaseEntryDescriptor GetDescriptor<T>()
        {
            return GetDescriptor(typeof(T));
        }

        public static DatabaseEntryDescriptor GetDescriptor(Type type)
        {
            lock (Descriptors)
            {
                DatabaseEntryDescriptor descriptor;
                if (Descriptors.TryGetValue(type, out descriptor))
                {
                    return descriptor;
                }

                // Create a new descriptor, this type is not known yet
                descriptor = new DatabaseEntryDescriptor(type);
                Descriptors.Add(type, descriptor);
                return descriptor;
            }
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

        public object GetPrimaryKey<T>(T entry)
            where T : IDatabaseEntry
        {
            return this.PrimaryKey.GetValue(entry);
        }

        public void SetPrimaryKey<T>(T entry, object value)
            where T : IDatabaseEntry
        {
            this.PrimaryKey.SetValue(entry, value);
        }

        public DatabaseEntryElementDescriptor GetElementByName(string name)
        {
            DatabaseEntryElementDescriptor descriptor;
            if (this.elementNameLookup.TryGetValue(name, out descriptor))
            {
                return descriptor;
            }

            return null;
        }

        public IList<string> GetElementNames()
        {
            IList<string> results = new List<string>();
            foreach (DatabaseEntryElementDescriptor element in this.Elements)
            {
                results.Add(element.Name);
            }

            return results;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Analyze()
        {
            this.PrimaryKey = null;
            this.Elements.Clear();

            IEnumerable<Attribute> typeAttributes = this.Type.GetCustomAttributes(true).Cast<Attribute>();
            foreach (Attribute attribute in typeAttributes)
            {
                var typed = attribute as DatabaseEntryAttribute;
                if (typed != null)
                {
                    this.EntryAttribute = typed;
                    break;
                }
            }

            Diagnostic.Assert(this.EntryAttribute != null, "The Main attribute is not defined");
            this.TableName = this.EntryAttribute.Table;

            PropertyInfo[] properties = this.Type.GetProperties();
            foreach (PropertyInfo info in properties)
            {
                object[] attributes = info.GetCustomAttributes(true);
                foreach (object attribute in attributes)
                {
                    Type attributeType = attribute.GetType();
                    
                    if (attributeType == typeof(DatabaseEntryElementAttribute))
                    {
                        var typed = (DatabaseEntryElementAttribute)attribute;
                        var element = new DatabaseEntryElementDescriptor(this.Type, typed, info);
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

                        this.Elements.Add(element);
                    }
                    else if (attributeType == typeof(DatabaseEntryJoinedElementAttribute))
                    {
                        var element = new DatabaseEntryJoinedElementDescriptor(this.Type, (DatabaseEntryJoinedElementAttribute)attribute, info);
                        this.JoinedElements.Add(element);
                    }
                }
            }

            if (this.PrimaryKey == null)
            {
                Diagnostic.Warning("DatabaseEntry has no Primary key defined: {0}", this.Type);
            }
        }
    }
}
