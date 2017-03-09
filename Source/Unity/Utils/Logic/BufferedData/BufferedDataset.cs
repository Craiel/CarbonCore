namespace CarbonCore.Unity.Utils.Logic.BufferedData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    using CarbonCore.Unity.Utils.Contracts.BufferedData;
    using CarbonCore.Utils.Contracts.IoC;

    using NLog;

    public class BufferedDataset : EngineComponent, IBufferedDataSetInternal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static int nextId;

        private readonly IFactory factory;

        private readonly BufferedDataSetInstances instances;

        private readonly IDictionary<object, BufferedDataSetInstances> keyToInstanceLookup;
        private readonly IDictionary<IBufferedDataEntry, object> instanceToKeyLookup;

        private int refCount;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BufferedDataset(IFactory factory)
        {
            this.factory = factory;

            this.instances = new BufferedDataSetInstances();
            this.keyToInstanceLookup = new Dictionary<object, BufferedDataSetInstances>();
            this.instanceToKeyLookup = new Dictionary<IBufferedDataEntry, object>();

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

        public void AddInstance(IBufferedDataEntry instance)
        {
            this.DoAddInstance(instance);
        }

        public void RemoveInstance(IBufferedDataEntry instance)
        {
            this.DoRemoveInstance(instance);
        }

        public T GetInstance<T>(object key = null)
            where T : IBufferedDataEntry
        {
            IList<IBufferedDataEntry> results = this.DoGetInstances(typeof(T));
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
            where T : IBufferedDataEntry
        {
            IList<IBufferedDataEntry> result = this.DoGetInstances(typeof(T), key);
            if (result == null)
            {
                return null;
            }

            return result.Cast<T>().ToList();
        }

        public void SetInstanceKey<T>(IBufferedDataEntry instance, T key)
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
                IList<IBufferedDataEntry> entryList = this.instances[type];
                foreach (IBufferedDataEntry entry in entryList)
                {
                    IBufferedDataEntry clonedEntry = (IBufferedDataEntry)entry.Clone();
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
        private void DoAddInstance(IBufferedDataEntry instance)
        {
            Debug.Assert(instance != null, "Instance can not be null!");

            Type type = instance.GetType();
            IList<IBufferedDataEntry> data;
            if (!this.instances.TryGetValue(type, out data))
            {
                data = new List<IBufferedDataEntry>();
                this.instances.Add(type, data);
            }

            Debug.Assert(!data.Contains(instance), "Instance was already added!");
            data.Add(instance);
        }

        private void DoRemoveInstance(IBufferedDataEntry instance)
        {
            Debug.Assert(instance != null, "Instance can not be null!");

            Type type = instance.GetType();
            if (!this.instances.ContainsKey(type))
            {
                Logger.Warn("Instance type was not registered");
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

        private IList<IBufferedDataEntry> DoGetInstances(Type type, object key = null)
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
            IList<IBufferedDataEntry> result;
            if (source.TryGetValue(type, out result))
            {
                return result;
            }

            return null;
        }

        private void DoSetInstanceKey(IBufferedDataEntry instance, object key)
        {
            if (!this.keyToInstanceLookup.ContainsKey(key))
            {
                this.keyToInstanceLookup.Add(key, new BufferedDataSetInstances());
            }

            // Add the key
            Type type = instance.GetType();
            if (!this.keyToInstanceLookup[key].ContainsKey(type))
            {
                this.keyToInstanceLookup[key].Add(type, new List<IBufferedDataEntry>());
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
        internal class BufferedDataSetInstances : Dictionary<Type, IList<IBufferedDataEntry>>
        {
        }
    }
}
