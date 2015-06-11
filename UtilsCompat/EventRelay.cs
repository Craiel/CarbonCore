namespace CarbonCore.Utils.Compat
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Compat.Contracts;

    public class EventRelay : IEventRelay
    {
        private readonly IDictionary<Type, IList<object>> subscribers;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EventRelay()
        {
            this.subscribers = new Dictionary<Type, IList<object>>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Subscribe<T>(Action<T> action)
        {
            this.Subscribe(typeof(T), action);
        }

        public void Subscribe<T>(Type type, Action<T> action)
        {
            lock (this.subscribers)
            {
                if (!this.subscribers.ContainsKey(type))
                {
                    this.subscribers.Add(type, new List<object>());
                }

                this.subscribers[type].Add(action);
            }
        }

        public void Unsubscribe<T>(Action<T> action)
        {
            this.Unsubscribe(typeof(T), action);
        }

        public void Unsubscribe<T>(Type type, Action<T> action)
        {
            lock (this.subscribers)
            {
                if (!this.subscribers.ContainsKey(type))
                {
                    return;
                }

                if (!this.subscribers[type].Contains(action))
                {
                    return;
                }

                this.subscribers[type].Remove(action);
            }
        }

        public void Relay<T>(T e)
        {
            this.Relay(typeof(T), e);
        }

        public void Relay<T>(Type type, T e)
        {
            IList<Action<T>> actionList = new List<Action<T>>();
            lock (this.subscribers)
            {
                if (!this.subscribers.ContainsKey(type))
                {
                    return;
                }

                foreach (Action<T> action in this.subscribers[type])
                {
                    actionList.Add(action);
                }
            }

            foreach (Action<T> action in actionList)
            {
                action(e);
            }
        }
    }
}
