namespace CarbonCore.Unity.Utils.Logic.BehaviorTree
{
    using System;
    using System.Collections.Generic;

    public class BehaviorTreeContext
    {
        private readonly IDictionary<Type, object> objects;
        private readonly IDictionary<object, object> variables;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BehaviorTreeContext()
        {
            this.objects = new Dictionary<Type, object>();
            this.variables = new Dictionary<object, object>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public virtual void Set<T>(T value)
        {
            Type type = typeof(T);
            if (this.objects.ContainsKey(type))
            {
                this.objects[type] = value;
                return;
            }

            this.objects.Add(type, value);
        }

        public virtual void SetVariable<T>(int key, T value)
        {
            this.DoSetVariable(key, value);
        }

        public virtual void SetVariable<T>(string key, T value)
        {
            this.DoSetVariable(key, value);
        }

        public virtual T Get<T>()
        {
            Type type = typeof(T);
            object result;
            if (this.objects.TryGetValue(type, out result))
            {
                return (T)result;
            }

            return default(T);
        }

        public virtual T GetVariable<T>(int key)
        {
            return this.DoGetVariable<T>(key);
        }

        public virtual T GetVariable<T>(string key)
        {
            return this.DoGetVariable<T>(key);
        }

        public virtual void Clear()
        {
            this.objects.Clear();
            this.variables.Clear();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoSetVariable<T>(object key, T value)
        {
            if (!this.variables.ContainsKey(key))
            {
                this.variables.Add(key, value);
                return;
            }

            this.variables[key] = value;
        }

        private T DoGetVariable<T>(object key)
        {
            object result;
            if (this.variables.TryGetValue(key, out result))
            {
                return (T)result;
            }

            return default(T);
        }
    }
}
