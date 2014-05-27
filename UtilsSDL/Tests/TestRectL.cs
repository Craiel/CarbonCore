namespace CarbonCore.UtilsSDL.Tests
{
    using CarbonCore.UtilsSDL.Math;

    using NUnit.Framework;

    [TestFixture]
    public class CameraTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public void SizingTest()
        {
            var testRect = new RectL();
            var testLine = new LineL(-5, -5, 10, 2);
            var testRectTwo = new RectL(2, 2, 20, 5);
            var testCircle = new Circle(0, 5, 10);

            testRect.Encompass(testLine.GetBounds());
            Assert.AreEqual(new Vector2L(-5, -5), testRect.Position);
            Assert.AreEqual(new Vector2L(15, 7), testRect.Size);

            testRect = new RectL();
            testRect.Encompass(testRectTwo);
            Assert.AreEqual(new Vector2L(0, 0), testRect.Position);
            Assert.AreEqual(new Vector2L(22, 7), testRect.Size);

            testRect.Encompass(testCircle.GetBounds());
            Assert.AreEqual(new Vector2L(-10, -5), testRect.Position);
            Assert.AreEqual(new Vector2L(32, 20), testRect.Size);
        }
    }
}
