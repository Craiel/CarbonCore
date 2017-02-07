namespace CarbonCore.Tests.Edge.Utils
{
    using CarbonCore.Utils.MathUtils;
    using Microsoft.Xna.Framework;
    using NUnit.Framework;

    [TestFixture]
    public class TestMath
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public void RectLTests()
        {
            var testRect = default(Rectangle);

            var testLine = new Line(-5, -5, 10, 2);
            var testRectTwo = new Rectangle(2, 2, 20, 5);

            testRect = Rectangle.Union(testRect, testLine.GetBounds());
            Assert.AreEqual(new Point(-5, -5), new Point(testRect.Left, testRect.Top));
            Assert.AreEqual(new Point(10, 2), new Point(testRect.Right, testRect.Bottom));

            testRect = Rectangle.Union(testRect, testRectTwo);
            Assert.AreEqual(new Point(-5, -5), new Point(testRect.Left, testRect.Top));
            Assert.AreEqual(new Point(22, 7), new Point(testRect.Right, testRect.Bottom));
        }
    }
}
