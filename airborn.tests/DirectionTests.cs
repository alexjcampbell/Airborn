using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class DirectionTests
    {
        [TestMethod]
        public void TestTrueToMagnetic()
        {
            Direction direction = Direction.FromTrue(350, -20);
            Assert.AreEqual(330, direction.DirectionMagnetic);

            direction = Direction.FromTrue(350, 20);
            Assert.AreEqual(010, direction.DirectionMagnetic);

            // a heading of 360 is the same as 0, but this should return 0
            direction = Direction.FromTrue(350, 10);
            Assert.AreEqual(0, direction.DirectionMagnetic);
            
        }

        [TestMethod]
        public void TestMagneticToTrue()
        {
            Direction direction = Direction.FromMagnetic(350, -20);
            Assert.AreEqual(010, direction.DirectionTrue);

            direction = Direction.FromMagnetic(010, 20);
            Assert.AreEqual(350, direction.DirectionTrue);

            direction = Direction.FromMagnetic(340, 10);
            Assert.AreEqual(330, direction.DirectionTrue);

            direction = Direction.FromMagnetic(0, 10);
            Assert.AreEqual(350, direction.DirectionTrue);

            direction = Direction.FromMagnetic(360, 10);
            Assert.AreEqual(350, direction.DirectionTrue);

            direction = Direction.FromMagnetic(360, -10);
            Assert.AreEqual(10, direction.DirectionTrue);            
        }

    }
}
