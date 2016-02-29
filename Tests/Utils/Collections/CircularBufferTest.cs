namespace CarbonCore.Tests.Utils.Collections
{
    using CarbonCore.Utils.Collections;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class CicularBufferTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public void CicularBufferConstructorTest()
        {
            var buffer = new CircularBuffer<int>(1);
            Assert.AreEqual(buffer.HaveValue(0), false);
            Assert.Throws<ArgumentOutOfRangeException>(delegate() { buffer.Get(0); });
        }

        [Test]
        public void CicularBufferAddTest()
        {
            var buffer = new CircularBuffer<int>(2);
            buffer.Add(0);
            buffer.Add(1);
            Assert.AreEqual(buffer.Get(0), 0);
            Assert.AreEqual(buffer.Get(1), 1);
            buffer.Add(2);
            Assert.Throws<ArgumentOutOfRangeException>(delegate() { buffer.Get(0); });
            Assert.AreEqual(buffer.Get(1), 1);
            Assert.AreEqual(buffer.Get(2), 2);
        }

        [Test]
        public void CicularBufferSetTest()
        {
            var buffer = new CircularBuffer<int>(2);
            buffer.Set(0, 0);
            buffer.Set(1, 1);
            Assert.AreEqual(buffer.Get(0), 0);
            Assert.AreEqual(buffer.Get(1), 1);

            buffer.Set(2, 2);
            Assert.AreEqual(buffer.HaveValue(0), false);
            Assert.AreEqual(buffer.Get(1), 1);
            Assert.AreEqual(buffer.Get(2), 2);

            buffer.Set(100, 100);
            Assert.AreEqual(buffer.HaveValue(99), false);
            Assert.AreEqual(buffer.Get(100), 100);

            buffer.Add(101);
            Assert.AreEqual(buffer.Get(100), 100);
            Assert.AreEqual(buffer.Get(101), 101);
        }

        [Test]
        public void CicularBufferSetTest2()
        {
            var buffer = new CircularBuffer<int>(3);
            buffer.Set(0, 0);
            buffer.Set(1, 1);
            Assert.AreEqual(buffer.Get(0), 0);
            Assert.AreEqual(buffer.Get(1), 1);

            buffer.Set(3, 3);
            Assert.AreEqual(buffer.Get(3), 3);
            Assert.AreEqual(buffer.HaveValue(2), false);
            Assert.AreEqual(buffer.Get(1), 1);
        }
    }
}
