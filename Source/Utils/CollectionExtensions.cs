namespace CarbonCore.Utils
{
    using System.Collections.Generic;

    public static class CollectionExtensions
    {
        public static void AddRange<T>(this IList<T> target, IEnumerable<T> source)
        {
            foreach (T entry in source)
            {
                target.Add(entry);
            }
        }

        public static void RemoveRange<T>(this IList<T> target, IEnumerable<T> source)
        {
            foreach (T entry in source)
            {
                target.Remove(entry);
            }
        }

        public static void AddRange<T>(this HashSet<T> target, IEnumerable<T> source)
        {
            foreach (T entry in source)
            {
                target.Add(entry);
            }
        }

        public static void RemoveRange<T>(this HashSet<T> target, IEnumerable<T> source)
        {
            foreach (T entry in source)
            {
                target.Remove(entry);
            }
        }

        public static void EnqueueRange<T>(this Queue<T> target, IEnumerable<T> source)
        {
            foreach (T entry in source)
            {
                target.Enqueue(entry);
            }
        }

        public static void PushRange<T>(this Stack<T> target, IEnumerable<T> source)
        {
            foreach (T entry in source)
            {
                target.Push(entry);
            }
        }

        public static void AddRange<T, TN>(this IDictionary<T, TN> target, IDictionary<T, TN> source)
        {
            foreach (KeyValuePair<T, TN> entry in source)
            {
                target.Add(entry.Key, entry.Value);
            }
        }
    }
}
