namespace CarbonCore.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using CarbonCore.ContentServices.Compat.Contracts;
    using CarbonCore.ContentServices.Compat.Logic.DataEntryLogic;

    using NUnit.Framework;

    public static class TestUtils
    {
        private static readonly Type BaseCascadeType = typeof(SyncCascade<>);

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void AssertStreamPos(Stream stream, long expectedCount, ref long lastPosition)
        {
            Assert.AreEqual(expectedCount, stream.Position - lastPosition);
            lastPosition = stream.Position;
        }

        public static void AssertListEquals<T>(IList<T> first, IList<T> second)
        {
            Assert.AreEqual(first.Count, second.Count);
            for (var i = 0; i < first.Count; i++)
            {
                AssertInstanceEquals(first[i], second[i]);
            }
        }

        public static void AssertDictionaryEquals<T, TN>(IDictionary<T, TN> first, IDictionary<T, TN> second)
        {
            Assert.AreEqual(first.Count, second.Count);

            IList<T> firstKeys = new List<T>(first.Keys);
            IList<T> secondKeys = new List<T>(second.Keys);
            AssertListEquals(firstKeys, secondKeys);

            IList<TN> firstValues = new List<TN>(first.Values);
            IList<TN> secondValues = new List<TN>(second.Values);
            AssertListEquals(firstValues, secondValues);
        }

        public static void AssertInstanceEquals(object first, object second)
        {
            if (first == null)
            {
                Assert.Null(second);
                return;
            }

            Assert.NotNull(second);
            Assert.AreEqual(first.GetType(), second.GetType());

            Type type = first.GetType();
            if (type == typeof(byte[]))
            {
                Assert.IsTrue(((byte[])first).SequenceEqual((byte[])second));
                return;
            }

            if (type == typeof(SyncObject<byte[]>))
            {
                Assert.IsTrue(((SyncObject<byte[]>)first).Value.SequenceEqual(((SyncObject<byte[]>)second).Value));
                return;
            }

            if (typeof(ISyncEntry).IsAssignableFrom(type))
            {
                IList<PropertyInfo> properties = type.GetProperties();
                foreach (PropertyInfo info in properties)
                {
                    var firstValue = info.GetValue(first);
                    var secondValue = info.GetValue(second);
                    AssertInstanceEquals(firstValue, secondValue);
                }

                return;
            }

            if (type.IsGenericType)
            {
                var args = type.GetGenericArguments();
                if (args.Length == 1 && typeof(ISyncEntry).IsAssignableFrom(args[0]))
                {
                    var cascadeType = BaseCascadeType.MakeGenericType(args[0]);
                    if (type == cascadeType)
                    {
                        PropertyInfo property = type.GetProperty("Value");
                        AssertInstanceEquals(property.GetValue(first), property.GetValue(second));

                        return;
                    }
                }
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return;
            }

            Assert.AreEqual(first, second);
        }
    }
}
