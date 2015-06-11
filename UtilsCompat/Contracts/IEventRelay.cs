namespace CarbonCore.Utils.Compat.Contracts
{
    using System;

    public interface IEventRelay
    {
        void Subscribe<T>(Action<T> action);
        void Subscribe<T>(Type type, Action<T> action);

        void Unsubscribe<T>(Action<T> action);
        void Unsubscribe<T>(Type type, Action<T> action);

        void Relay<T>(T e);
        void Relay<T>(Type type, T e);
    }
}
