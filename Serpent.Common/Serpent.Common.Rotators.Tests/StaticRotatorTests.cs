namespace Serpent.Common.Rotators.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StaticRotatorTests
    {
        [TestMethod]
        public void TestGetNext()
        {
            var items = new[] { 1, 3, 5, 7, 9 };

            var rotator = new StaticRotator<int>(items);

            Assert.AreEqual(5, rotator.Count);

            Assert.AreEqual(1, rotator.GetNext());
            Assert.AreEqual(3, rotator.GetNext());
            Assert.AreEqual(5, rotator.GetNext());
            Assert.AreEqual(7, rotator.GetNext());
            Assert.AreEqual(9, rotator.GetNext());
            Assert.AreEqual(1, rotator.GetNext());
            Assert.AreEqual(3, rotator.GetNext());
            Assert.AreEqual(5, rotator.GetNext());
            Assert.AreEqual(7, rotator.GetNext());
            Assert.AreEqual(9, rotator.GetNext());
        }
    }
}