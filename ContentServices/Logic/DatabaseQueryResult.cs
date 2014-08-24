namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.IO;

    using CarbonCore.ContentServices.Contracts;
    
    public class DatabaseQueryResult
    {
        private readonly IDatabaseService provider;

        private readonly DbCommand command;

        private IList<object[]> results;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseQueryResult(IDatabaseService provider, DbCommand command)
        {
            this.provider = provider;
            this.command = command;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Count
        {
            get
            {
                this.EvaluateCommand();
                return this.results.Count;
            }
        }

        public IList<object> ToList()
        {
            this.EvaluateCommand();
            return new List<object>(this.results);
        }

        public object UniqueResult()
        {
            this.EvaluateCommand();
            if (this.results.Count != 1)
            {
                throw new InvalidDataException("Expected unique result but got " + this.results.Count);
            }

            return this.results[0];
        }
        
        private void EvaluateCommand()
        {
            if (this.results != null)
            {
                return;
            }

            this.results = new List<object[]>();
            using (DbDataReader reader = this.command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var buffer = new object[reader.FieldCount];
                    if (reader.GetValues(buffer) != reader.FieldCount)
                    {
                        throw new InvalidOperationException("GetValues returned wrong number of values, expected " + reader.FieldCount);
                    }

                    this.results.Add(buffer);
                }
            }
        }

        /*private IList ProcessResults(Type targetType)
        {
            IList<ContentReflectionProperty> properties = ContentReflection.GetPropertyInfos(targetType);
            IList processed = new List<object>();

            for (int i = 0; i < this.results.Count; i++)
            {
                if (this.results[i].Length != properties.Count)
                {
                    System.Diagnostics.Trace.TraceError(
                        "Result does not match property count for {0} (got {1} but expected {2}",
                        null,
                        targetType,
                        this.results[i].Length,
                        properties.Count);
                    continue;
                }

                object entry = Activator.CreateInstance(targetType);
                for (int p = 0; p < properties.Count; p++)
                {
                    object translatedValue = this.TanslateValue(properties[p].Info.PropertyType, this.results[i][p]);
                    properties[p].Info.SetValue(entry, translatedValue);
                }

                // Lock the change state if we are dealing with content objects
                // Todo: Might want to turn this behavior off for the client
                if (entry as ICarbonContent != null)
                {
                    ((ICarbonContent)entry).LockChangeState();
                }

                processed.Add(entry);
            }

            return processed;
        }

        private object TanslateValue(Type type, object source)
        {
            Type targetType = type;
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                targetType = targetType.GetGenericArguments()[0];
            }

            if (targetType == typeof(ContentLink))
            {
                var id = (int?)typeof(int?).ConvertValue(source);
                if (id == null)
                {
                    return null;
                }

                return this.contentManager.ResolveLink(id.Value);
            }

            return type.ConvertValue(source);
        }*/
    }
}
