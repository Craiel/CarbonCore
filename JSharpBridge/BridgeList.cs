namespace CarbonCore.JSharpBridge
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using CarbonCore.JSharpBridge.Collections;
    using CarbonCore.JSharpBridge.Core;

    public static class BridgeList
    {
        public static int Count(this IList list)
        {
            return list.Count;
        }

        public static object Get(this IList list, int index)
        {
            return list[index];
        }

        public static object Get(this IDictionary<object, object> dictionary, object key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return null;
        }

        public static bool IsEmpty(this IList list)
        {
            return list.Count <= 0;
        }

        public static bool IsEmpty(this ISet<object> set)
        {
            return set.Count <= 0;
        }

        public static bool IsEmpty(this Dictionary<object, object> dictionary)
        {
            return dictionary.Count <= 0;
        }

        public static int Size(this IList list)
        {
            return list.Count;
        }

        public static int Size(this ISet<object> set)
        {
            return set.Count;
        }

        public static void Set(this IList list, int index, object entry)
        {
            if (list.Count <= index)
            {
                list.Add(entry);
            }
            else
            {
                list.Insert(index, entry);
            }
        }

        public static void Put(this Dictionary<object, object> dictionary, object key, object value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        public static void AddAll(this Iterable list, IList other)
        {
            throw new NotImplementedException();
        }

        public static Iterator Iterator(this IList list)
        {
            return new ListIterator(list, list.GetEnumerator());
        }

        public static Iterator Iterator(this ISet<object> set)
        {
            return new SetIterator<object>(set, set.GetEnumerator());
        }

        public static Iterator Iterator(this ICollection<object> set)
        {
            return new Iterator(set.GetEnumerator());
        }

        public static Iterator Iterator(this Dictionary<object, object>.KeyCollection collection)
        {
            return new Iterator(collection.GetEnumerator());
        }

        public static Iterator Iterator(this Dictionary<object, object>.ValueCollection collection)
        {
            return new Iterator(collection.GetEnumerator());
        }

        public static void Sort(JavaCollection javaCollection, JavaComparator comparer = null)
        {
            throw new NotImplementedException();
        }

        public static void Sort(object[] array, JavaComparator comparer = null)
        {
            throw new NotImplementedException();
        }

        public static void Fill<T>(T[] array, int count)
        {
            throw new NotImplementedException();
        }

        public static JavaCollection AsList(object[] entries)
        {
            throw new NotImplementedException();
        }

        public static void Shuffle(JavaCollection other)
        {
            throw new NotImplementedException();
        }

        public static JavaCollection EmptyList()
        {
            throw new NotImplementedException();
        }

        public static JavaCollection SynchronizedList(JavaCollection javaCollection)
        {
            throw new NotImplementedException();
        }

        public static JavaHashSet EmptySet()
        {
            throw new NotImplementedException();
        }

        public static JavaHashSet NewHashSet()
        {
            throw new NotImplementedException();
        }

        public static JavaTreeSet NewTreeSet(JavaCollection values)
        {
            throw new NotImplementedException();
        }

        public static object[] ToArray(object[] split, Type type)
        {
            throw new NotImplementedException();
        }

        public static JavaHashSet NewLinkedHashSet()
        {
            throw new NotImplementedException();
        }

        public static object Transform(JavaCollection par1List, Function transformFunction)
        {
            throw new NotImplementedException();
        }

        public static void Reverse(JavaCollection var12)
        {
            throw new NotImplementedException();
        }

        public static JavaCollection UnmodifiableList(JavaCollection listOfLanServers)
        {
            throw new NotImplementedException();
        }

        public static void Sort(object[] javaCollection)
        {
            throw new NotImplementedException();
        }

        public static void AddAll<T>(JavaHashSet var1, IList<T> getAvailableDisplayModes)
        {
            throw new NotImplementedException();
        }

        public static void Fill<T>(T[] par1ArrayOfBiomeGenBase, int count, int i, T biomeToUse)
        {
            throw new NotImplementedException();
        }

        public static int GetHashCode<T>(T[] data)
        {
            throw new NotImplementedException();
        }
    }
}
