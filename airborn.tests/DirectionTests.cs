using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class DirectionTests
    {
        [TestMethod]
        public void Test_TrueToMagnetic_MagneticVariation_Minus20_StartingAt350_Is330()
        {
            double directionDegrees = 350;
            double magneticVariation = -20;

            double expected = 330;

            Direction direction = Direction.FromTrue(
                directionDegrees,
                magneticVariation
                );

            Assert.AreEqual(
                expected,
                direction.DirectionMagnetic,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

        }

        [TestMethod]
        public void Test_TrueToMagnetic_MagneticVariation_Minus20_StartingAtZero_Is340()
        {
            double directionDegrees = 0;
            double magneticVariation = -20;

            double expected = 340;

            Direction direction = Direction.FromTrue(directionDegrees, magneticVariation);

            Assert.AreEqual(
                expected,
                direction.DirectionMagnetic,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
            );

        }

        [TestMethod]
        public void Test_TrueToMagnetic_MagneticVariation_Minus20_StartingAt180_Is160()
        {
            double directionDegrees = 180;
            double magneticVariation = -20;

            double expected = 160;

            Direction direction = Direction.FromTrue(directionDegrees, magneticVariation);

            Assert.AreEqual(
                expected,
                direction.DirectionMagnetic,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
            );

        }

        [TestMethod]
        public void Test_TruetoMagnetic_MagneticVariation_Zero_NoChange_Is180()
        {
            double directionDegrees = 180;
            double magneticVariation = 0;

            double expected = 180;

            Direction direction = Direction.FromTrue(directionDegrees, magneticVariation);

            Assert.AreEqual(
                expected,
                direction.DirectionMagnetic,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
            );
        }

        [TestMethod]
        public void Test_TrueToMagnetic_MagneticVariation_Plus20_StartingAt350_Is10()
        {
            double directionDegrees = 350;
            double magneticVariation = 20;

            double expected = 10;

            Direction direction = Direction.FromTrue(directionDegrees, magneticVariation);

            Assert.AreEqual(
                expected,
                direction.DirectionMagnetic,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
            );

        }

        [TestMethod]
        public void Test_TrueToMagnetic_MagneticVariation_Plus20_StartingAt10_Is30()
        {
            double directionDegrees = 10;
            double magneticVariation = 20;

            double expected = 30;

            Direction direction = Direction.FromTrue(directionDegrees, magneticVariation);

            Assert.AreEqual(
                expected,
                direction.DirectionMagnetic,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
            );
        }

        [TestMethod]
        public void Test_MagneticToTrue_MagneticVariation_Minus20_StartingAt350_Is10()
        {
            double directionDegrees = 350;
            double magneticVariation = -20;

            double expected = 10;

            Direction direction = Direction.FromMagnetic(directionDegrees, magneticVariation);

            Assert.AreEqual(
                expected,
                direction.DirectionMagnetic,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
            );
        }

        [TestMethod]
        public void Test_MagneticToTrue_MagneticVariation_Minus20_StartingAt10_Is30()
        {
            double directionDegrees = 10;
            double magneticVariation = -20;

            double expected = 30;

            Direction direction = Direction.FromMagnetic(directionDegrees, magneticVariation);

            Assert.AreEqual(
                expected,
                direction.DirectionMagnetic,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
            );

        }

        [TestMethod]
        public void Test_MagneticToTrue_MagneticVariation_Minus20_StartingAtZero_Is20()
        {
            double directionDegrees = 0;
            double magneticVariation = -20;

            double expected = 20;

            Direction direction = Direction.FromMagnetic(directionDegrees, magneticVariation);

            Assert.AreEqual(
                expected,
                direction.DirectionMagnetic,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
            );
        }

        [TestMethod]
        public void Test_MagneticToTrue_MagneticVariation_Minus20_StartingAt180_Is200()
        {
            double directionDegrees = 180;
            double magneticVariation = -20;

            double expected = 200;

            Direction direction = Direction.FromMagnetic(directionDegrees, magneticVariation);

            Assert.AreEqual(
                expected,
                direction.DirectionMagnetic,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
            );

        }

        [TestMethod]
        public void Test_MagneticToTrue_MagneticVariation_Zero_NoChange_Is180()
        {
            double directionDegrees = 180;
            double magneticVariation = 0;

            double expected = 180;

            Direction direction = Direction.FromMagnetic(directionDegrees, magneticVariation);

            Assert.AreEqual(
                expected,
                direction.DirectionMagnetic,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
            );
        }

        [TestMethod]
        public void Test_MagneticToTrue_MagneticVariation_Plus20_StartingAt350_Is10()
        {
            double directionDegrees = 350;
            double magneticVariation = 20;

            double expected = 10;

            Direction direction = Direction.FromMagnetic(directionDegrees, magneticVariation);

            Assert.AreEqual(
                expected,
                direction.DirectionMagnetic,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
            );
        }

        [TestMethod]
        public void Test_MagneticToTrue_MagneticVariation_Plus20_StartingAt10_Is350()
        {
            double directionDegrees = 10;
            double magneticVariation = 20;

            double expected = 350;

            Direction direction = Direction.FromMagnetic(directionDegrees, magneticVariation);

            Assert.AreEqual(
                expected,
                direction.DirectionMagnetic,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
            );
        }

    }
}
