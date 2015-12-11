namespace CarbonCore.Utils.Unity.Logic.BehaviorTree
{
    using System.Collections.Generic;

    public class BehaviorTreeContext
    {
        private readonly IDictionary<object, object> variables;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BehaviorTreeContext()
        {
            this.variables = new Dictionary<object, object>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Set<T>(T value)
        {
            string key = typeof(T).FullName;
            this.DoSet(key, value);
        }

        public void Set<T>(int key, T value)
        {
            this.DoSet(key, value);
        }

        public void Set<T>(string key, T value)
        {
            this.DoSet(key, value);
        }

        public T Get<T>()
        {
            string key = typeof(T).FullName;
            return this.DoGet<T>(key);
        }

        public T Get<T>(int key)
        {
            return this.DoGet<T>(key);
        }

        public T Get<T>(string key)
        {
            return this.DoGet<T>(key);
        }

        public void Clear()
        {
            this.variables.Clear();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoSet<T>(object key, T value)
        {
            if (!this.variables.ContainsKey(key))
            {
                this.variables.Add(key, value);
                return;
            }

            this.variables[key] = value;
        }

        private T DoGet<T>(object key)
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
