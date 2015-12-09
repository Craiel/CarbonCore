namespace CarbonCore.Tests.Utils
{
    using CarbonCore.Utils.MathUtils;

    using NUnit.Framework;

    [TestFixture]
    public class MathTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public void RectLTests()
        {
            var testRect = new RectL();
            var testLine = new LineL(-5, -5, 10, 2);
            var testRectTwo = new RectL(2, 2, 20, 5);
            var testCircle = new Circle(-15, 5, 10);

            testRect = testRect.Encompass(testLine.GetBounds());
            Assert.AreEqual(new Vector2L(-5, -5), testRect.LeftTop);
            Assert.AreEqual(new Vector2L(10, 2), testRect.RightBottom);

            testRect = testRect.Encompass(testRectTwo);
            Assert.AreEqual(new Vector2L(-5, -5), testRect.LeftTop);
            Assert.AreEqual(new Vector2L(22, 7), testRect.RightBottom);

            testRect = testRect.Encompass(testCircle.GetBounds());
            Assert.AreEqual(new Vector2L(-25, -5), testRect.LeftTop);
            Assert.AreEqual(new Vector2L(22, 15), testRect.RightBottom);
        }
    }
}
