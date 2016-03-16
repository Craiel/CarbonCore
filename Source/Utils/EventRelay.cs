namespace CarbonCore.Utils
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Contracts;

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
                IList<object> list;
                if (this.subscribers.TryGetValue(type, out list))
                {
                    list.Add(action);
                    return;
                }

                this.subscribers.Add(type, new List<object> { action });
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
                IList<object> list;
                if (!this.subscribers.TryGetValue(type, out list))
                {
                    return;
                }

                if (!list.Contains(action))
                {
                    return;
                }

                list.Remove(action);
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
                IList<object> list;
                if (!this.subscribers.TryGetValue(type, out list))
                {
                    return;
                }

                foreach (Action<T> action in list)
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
