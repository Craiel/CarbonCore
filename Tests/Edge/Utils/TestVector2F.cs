namespace CarbonCore.Tests.Utils
{
    using System;

    using CarbonCore.Utils.MathUtils;

    using NUnit.Framework;

    [TestFixture]
    public class TestVector2F
    {
        [Test]
        public void Add()
        {
            var v1 = new Vector2F(1, 2);
            var v2 = new Vector2F(4, 3);
            Vector2F res = v1 + v2;
            Assert.AreEqual(5, res.X);
            Assert.AreEqual(5, res.Y);
        }

        [Test]
        public void AddAssign()
        {
            var v1 = new Vector2F(1, 2);
            v1 += new Vector2F(4, 3);
            Assert.AreEqual(5, v1.X);
            Assert.AreEqual(5, v1.Y);
        }

        [Test]
        public void Subtract()
        {
            var v1 = new Vector2F(5, 5);
            var v2 = new Vector2F(4, 3);
            Vector2F res = v1 - v2;
            Assert.AreEqual(1, res.X);
            Assert.AreEqual(2, res.Y);
        }

        [Test]
        public void SubtractAssign()
        {
            var v1 = new Vector2F(5, 5);
            v1 -= new Vector2F(4, 3);
            Assert.AreEqual(1, v1.X);
            Assert.AreEqual(2, v1.Y);
        }

        [Test]
        public void Multiply()
        {
            var v1 = new Vector2F(1, 2);
            const float Scale = 10.0f;
            Vector2F res = v1 * Scale;
            Assert.AreEqual(10, res.X);
            Assert.AreEqual(20, res.Y);
        }

        [Test]
        public void MultiplyAssign()
        {
            var v1 = new Vector2F(1, 2);
            v1 *= 10.0f;
            Assert.AreEqual(10, v1.X);
            Assert.AreEqual(20, v1.Y);
        }

        [Test]
        public void Divide()
        {
            var v1 = new Vector2F(10, 20);
            const float Scale = 10.0f;
            Vector2F res = v1 / Scale;
            Assert.AreEqual(1, res.X);
            Assert.AreEqual(2, res.Y);
        }

        [Test]
        public void DivideAssign()
        {
            var v1 = new Vector2F(10, 20);
            v1 /= 10.0f;
            Assert.AreEqual(1, v1.X);
            Assert.AreEqual(2, v1.Y);
        }

        [Test]
        public void LengthQ()
        {
            var v1 = new Vector2F(2, 2);
            Assert.AreEqual(8, v1.LengthQ);
        }

        [Test]
        public void Length()
        {
            var v1 = new Vector2F(2, 2);
            Assert.AreEqual(2.8284, v1.Length, 0.0001);
        }

        [Test]
        public void Normalize()
        {
            var v1 = new Vector2F(2, 2);
            Assert.AreEqual(0.7071, v1.Normalized.X, 0.0001);
            Assert.AreEqual(0.7071, v1.Normalized.Y, 0.0001);
        }

        [Test]
        public void Angle()
        {
            var v1 = new Vector2F(1, 0);
            Assert.AreEqual(0.0, v1.Angle, 0.0001);

            v1 = new Vector2F(0, 1);
            Assert.AreEqual(Math.PI / 2, v1.Angle, 0.0001);

            v1 = new Vector2F(-1, 0);
            Assert.AreEqual(Math.PI, v1.Angle, 0.0001);

            v1 = new Vector2F(0, -1);
            Assert.AreEqual(-Math.PI / 2, v1.Angle, 0.0001);

            v1 = new Vector2F(1, 1);
            Assert.AreEqual(Math.PI / 4, v1.Angle, 0.0001);
        }

        [Test]
        public void AngleTo()
        {
            var v1 = new Vector2F(1, 0);
            var v2 = new Vector2F(0, 1);
            Assert.AreEqual(Math.PI - Math.PI / 4, v1.AngleTo(v2), 0.0001);

            v1 = new Vector2F(1, 1);
            v2 = new Vector2F(-1, 1);
            Assert.AreEqual(Math.PI, v1.AngleTo(v2), 0.0001);
        }

        [Test]
        public void DistanceTo()
        {
            var v1 = new Vector2F(1, 0);
            var v2 = new Vector2F(0, 1);
            Assert.AreEqual(1.4142, v1.DistanceTo(v2), 0.0001);

            v1 = new Vector2F(1, 1);
            v2 = new Vector2F(-1, 1);
            Assert.AreEqual(2, v1.DistanceTo(v2));
        }

        [Test]
        public void Dot()
        {
            var v1 = new Vector2F(1, 0);
            var v2 = new Vector2F(0, 1);
            Assert.AreEqual(0.0, v1.Dot(v2), 0.0001);

            v1 = new Vector2F(1, 1);
            v2 = new Vector2F(1, 0);
            Assert.AreEqual(Math.Cos(Math.PI / 4), v1.Normalized.Dot(v2.Normalized), 0.0001);
        }
    }
}
