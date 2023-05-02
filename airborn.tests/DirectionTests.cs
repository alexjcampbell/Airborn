using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class DirectionTests
    {
        [TestMethod]
        public void TestTrueToMagnetic_MagneticVariation_Minus20_StartingAt350_Is330()
        {
            int directionDegrees = 350;
            int magneticVariation = -20;

            int expected = 330;

            Direction direction = Direction.FromTrue(directionDegrees, magneticVariation);
            Assert.AreEqual(expected, direction.DirectionMagnetic);

        }

        [TestMethod]
        public void TestTrueToMagnetic_MagneticVariation_Minus20_StartingAt10_Is350()
        {
            int directionDegrees = 10;
            int magneticVariation = -20;

            int expected = 350;

            Direction direction = Direction.FromTrue(directionDegrees, magneticVariation);
            Assert.AreEqual(expected, direction.DirectionMagnetic);

        }

        [TestMethod]
        public void TestTrueToMagnetic_MagneticVariation_Minus20_StartingAtZero_Is340()
        {
            int directionDegrees = 0;
            int magneticVariation = -20;

            int expected = 340;

            Direction direction = Direction.FromTrue(directionDegrees, magneticVariation);
            Assert.AreEqual(expected, direction.DirectionMagnetic);

        }

        [TestMethod]
        public void TestTrueToMagnetic_MagneticVariation_Minus20_StartingAt180_Is160()
        {
            int directionDegrees = 180;
            int magneticVariation = -20;

            int expected = 160;

            Direction direction = Direction.FromTrue(directionDegrees, magneticVariation);
            Assert.AreEqual(expected, direction.DirectionMagnetic);

        }

        [TestMethod]
        public void TestTruetoMagnetic_MagneticVariation_Zero_NoChange_Is180()
        {
            int directionDegrees = 180;
            int magneticVariation = 0;

            int expected = 180;

            Direction direction = Direction.FromTrue(directionDegrees, magneticVariation);
            Assert.AreEqual(expected, direction.DirectionMagnetic);
        }

        [TestMethod]
        public void TestTrueToMagnetic_MagneticVariation_Plus20_StartingAt350_Is010()
        {
            int directionDegrees = 350;
            int magneticVariation = 20;

            int expected = 010;

            Direction direction = Direction.FromTrue(directionDegrees, magneticVariation);
            Assert.AreEqual(expected, direction.DirectionMagnetic);

        }

        [TestMethod]
        public void TestTrueToMagnetic_MagneticVariation_Plus20_StartingAt10_Is30()
        {
            int directionDegrees = 10;
            int magneticVariation = 20;

            int expected = 30;

            Direction direction = Direction.FromTrue(directionDegrees, magneticVariation);
            Assert.AreEqual(expected, direction.DirectionMagnetic);
        }

        [TestMethod]
        public void TestMagneticToTrue_MagneticVariation_Minus20_StartingAt350_Is10()
        {
            int directionDegrees = 350;
            int magneticVariation = -20;

            int expected = 010;

            Direction direction = Direction.FromMagnetic(directionDegrees, magneticVariation);
            Assert.AreEqual(expected, direction.DirectionTrue);
        }

        [TestMethod]
        public void TestMagneticToTrue_MagneticVariation_Minus20_StartingAt10_Is30()
        {
            int directionDegrees = 10;
            int magneticVariation = -20;

            int expected = 030;

            Direction direction = Direction.FromMagnetic(directionDegrees, magneticVariation);
            Assert.AreEqual(expected, direction.DirectionTrue);

        }

        [TestMethod]
        public void TestMagneticToTrue_MagneticVariation_Minus20_StartingAtZero_Is20()
        {
            int directionDegrees = 0;
            int magneticVariation = -20;

            int expected = 020;

            Direction direction = Direction.FromMagnetic(directionDegrees, magneticVariation);
            Assert.AreEqual(expected, direction.DirectionTrue);
        }

        [TestMethod]
        public void TestMagneticToTrue_MagneticVariation_Minus20_StartingAt180_Is200()
        {
            int directionDegrees = 180;
            int magneticVariation = -20;

            int expected = 200;

            Direction direction = Direction.FromMagnetic(directionDegrees, magneticVariation);
            Assert.AreEqual(expected, direction.DirectionTrue);

        }

        [TestMethod]
        public void TestMagneticToTrue_MagneticVariation_Zero_NoChange_Is180()
        {
            int directionDegrees = 180;
            int magneticVariation = 0;

            int expected = 180;

            Direction direction = Direction.FromMagnetic(directionDegrees, magneticVariation);
            Assert.AreEqual(expected, direction.DirectionTrue);
        }

        [TestMethod]
        public void TestMagneticToTrue_MagneticVariation_Plus20_StartingAt350_Is330()
        {
            int directionDegrees = 350;
            int magneticVariation = 20;

            int expected = 330;

            Direction direction = Direction.FromMagnetic(directionDegrees, magneticVariation);
            Assert.AreEqual(expected, direction.DirectionTrue);
        }

        [TestMethod]
        public void TestMagneticToTrue_MagneticVariation_Plus20_StartingAt10_Is350()
        {
            int directionDegrees = 10;
            int magneticVariation = 20;

            int expected = 350;

            Direction direction = Direction.FromMagnetic(directionDegrees, magneticVariation);
            Assert.AreEqual(expected, direction.DirectionTrue);
        }

    }
}
