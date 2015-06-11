namespace CarbonCore.Utils.Compat.Contracts
{
    using System.Collections.Generic;

    public interface IExtendedDictionary<T, TN> : IDictionary<T, TN>
    {
        bool EnableReverseLookup { get; set; }

        IEnumerator<KeyValuePair<TN, T>> GetReverseEnumerator();

        void Add(KeyValuePair<TN, T> item);

        bool Contains(KeyValuePair<TN, T> item);

        void CopyTo(KeyValuePair<TN, T>[] array, int arrayIndex);

        bool Remove(KeyValuePair<TN, T> item);

        bool ContainsValue(TN value);

        bool TryGetKey(TN value, out T key);
    }
}
