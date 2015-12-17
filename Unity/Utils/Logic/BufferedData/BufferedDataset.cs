namespace CarbonCore.Utils.Unity.Logic.BufferedData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    
    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Unity.Contracts.BufferedData;
    using CarbonCore.Utils.Unity.Logic;

    public class BufferedDataset : EngineComponent, IBufferedDataSetInternal
    {
        private static int nextId;

        private readonly IFactory factory;

        private readonly BufferedDataSetInstances instances;

        private readonly IDictionary<object, BufferedDataSetInstances> keyToInstanceLookup;
        private readonly IDictionary<IDataEntry, object> instanceToKeyLookup;

        private int refCount;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BufferedDataset(IFactory factory)
        {
            this.factory = factory;

            this.instances = new BufferedDataSetInstances();
            this.keyToInstanceLookup = new Dictionary<object, BufferedDataSetInstances>();
            this.instanceToKeyLookup = new Dictionary<IDataEntry, object>();

            this.Id = Interlocked.Increment(ref nextId);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Id { get; private set; }

        public void Execute(IBufferedDataCommand command)
        {
            command.Execute(this);
        }

        public void AddInstance(IDataEntry instance)
        {
            this.DoAddInstance(instance);
        }

        public void RemoveInstance(IDataEntry instance)
        {
            this.DoRemoveInstance(instance);
        }

        public T GetInstance<T>(object key = null)
            where T : IDataEntry
        {
            IList<IDataEntry> results = this.DoGetInstances(typeof(T));
            if (results == null || results.Count <= 0)
            {
                return default(T);
            }

            if (results.Count > 1)
            {
                throw new InvalidOperationException(string.Format("GetInstances<T>() returned {0} entries, expected only one", results.Count));
            }

            return (T)results[0];
        }

        // Note: This is a GC heavy function, use with caution
        public IList<T> GetInstances<T>(object key = null)
            where T : IDataEntry
        {
            IList<IDataEntry> result = this.DoGetInstances(typeof(T), key);
            if (result == null)
            {
                return null;
            }

            return result.Cast<T>().ToList();
        }

        public void SetInstanceKey<T>(IDataEntry instance, T key)
        {
            this.DoSetInstanceKey(instance, key);
        }

        public void Reset()
        {
            this.instances.Clear();
            this.keyToInstanceLookup.Clear();
            this.instanceToKeyLookup.Clear();

            this.refCount = 0;
        }

        public int RefCount()
        {
            return this.refCount;
        }

        public void RefCountIncrease()
        {
            Interlocked.Increment(ref this.refCount);
        }

        public void RefCountDecrease()
        {
            Interlocked.Decrement(ref this.refCount);
        }

        IBufferedDataSetInternal IBufferedDataSetInternal.Clone()
        {
            var clone = this.factory.Resolve<IBufferedDataSetInternal>();
            clone.Initialize();

            // Clone over all the contents of the buffer
            foreach (Type type in this.instances.Keys)
            {
                IList<IDataEntry> entryList = this.instances[type];
                foreach (IDataEntry entry in entryList)
                {
                    IDataEntry clonedEntry = entry.Clone();
                    clone.AddInstance(clonedEntry);

                    object instanceKey;
                    if (this.instanceToKeyLookup.TryGetValue(entry, out instanceKey))
                    {
                        clone.SetInstanceKey(clonedEntry, instanceKey);
                    }
                }
            }

            return clone;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoAddInstance(IDataEntry instance)
        {
            Diagnostic.Assert(instance != null, "Instance can not be null!");

            Type type = instance.GetType();
            IList<IDataEntry> data;
            if (!this.instances.TryGetValue(type, out data))
            {
                data = new List<IDataEntry>();
                this.instances.Add(type, data);
            }

            Diagnostic.Assert(!data.Contains(instance), "Instance was already added!");
            data.Add(instance);
        }

        private void DoRemoveInstance(IDataEntry instance)
        {
            Diagnostic.Assert(instance != null, "Instance can not be null!");

            Type type = instance.GetType();
            if (!this.instances.ContainsKey(type))
            {
                Diagnostic.Warning("Instance type was not registered");
                return;
            }

            this.instances[type].Remove(instance);

            // Check if this instance was associated with a key
            object instanceKey;
            if (this.instanceToKeyLookup.TryGetValue(instance, out instanceKey))
            {
                // Unregister the key and the association
                this.instanceToKeyLookup.Remove(instance);
                this.keyToInstanceLookup[instanceKey][type].Remove(instance);
            }
        }

        private IList<IDataEntry> DoGetInstances(Type type, object key = null)
        {
            // Check if we are looking instances associated with a specific key
            BufferedDataSetInstances source;
            if (key != null)
            {
                if (!this.keyToInstanceLookup.TryGetValue(key, out source))
                {
                    return null;
                }
            }
            else
            {
                source = this.instances;
            }

            // Now we try to find the instances of the given type
            IList<IDataEntry> result;
            if (source.TryGetValue(type, out result))
            {
                return result;
            }

            return null;
        }

        private void DoSetInstanceKey(IDataEntry instance, object key)
        {
            if (!this.keyToInstanceLookup.ContainsKey(key))
            {
                this.keyToInstanceLookup.Add(key, new BufferedDataSetInstances());
            }

            // Add the key
            Type type = instance.GetType();
            if (!this.keyToInstanceLookup[key].ContainsKey(type))
            {
                this.keyToInstanceLookup[key].Add(type, new List<IDataEntry>());
            }

            this.keyToInstanceLookup[key][type].Add(instance);

            // Add the reverse lookup for delete
            if (this.instanceToKeyLookup.ContainsKey(instance))
            {
                this.instanceToKeyLookup[instance] = key;
            }
            else
            {
                this.instanceToKeyLookup.Add(instance, key);
            }
        }

        // -------------------------------------------------------------------
        // Internal helper class
        // -------------------------------------------------------------------
        internal class BufferedDataSetInstances : Dictionary<Type, IList<IDataEntry>>
        {
        }
    }
}
