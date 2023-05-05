using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class WindTests
    {


        [TestMethod]
        public void Test_AngularDifference_WindLeftOfRunway_IsCorrect()
        {
            int magneticVariation = 0;

            Wind wind = Wind.FromMagnetic(310, magneticVariation);
            Runway runway = Runway.FromMagnetic(new Airport(), 10, magneticVariation);

            double expected = -60;

            Assert.AreEqual(
                expected,
                CalculationUtilities.SmallestAngularDifference(10, 310),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

        }

        [TestMethod]
        public void Test_AngularDifference_WindRightOfRunway_IsCorrect()
        {
            int magneticVariation = 0;

            Wind wind = Wind.FromMagnetic(010, magneticVariation);
            Runway runway = Runway.FromMagnetic(new Airport(), 310, magneticVariation);

            double expected = -60;

            Assert.AreEqual(expected,
                CalculationUtilities.SmallestAngularDifference(
                    wind.Direction.DirectionMagnetic,
                    runway.RunwayHeading.DirectionMagnetic),
                    UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_AngularDifference_NoDifference_ReturnsNoDifference()
        {
            int runwayHeading = 0;
            int windDirection = 0;

            double expected = 0;

            Assert.AreEqual(
                expected,
                CalculationUtilities.SmallestAngularDifference(runwayHeading, windDirection),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_AngularDifference_180DegreeDifference_ReturnsMinus180()
        {
            int runwayHeading = 180;
            int windDirection = 0;

            double expected = -180;

            Assert.AreEqual(
                expected,
                CalculationUtilities.SmallestAngularDifference(runwayHeading, windDirection),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_AngularDifference_WindGreaterThan180Degrees_IsCorrect()
        {
            int runwayHeading = 160;
            int windDirection = 10;

            double expected = -150;

            Assert.AreEqual(
                expected,
                CalculationUtilities.SmallestAngularDifference(runwayHeading, windDirection),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_AngularDifference_NearNorthButWest_IsCorrect()
        {
            int runwayHeading = 350;
            int windDirection = 10;

            double expected = 20;

            Assert.AreEqual(
                expected,
                CalculationUtilities.SmallestAngularDifference(runwayHeading, windDirection)
                );
        }

        [TestMethod]
        public void Test_AngularDifference_NearNorthButEast_IsCorrect()
        {
            int runwayHeading = 10;
            int windDirection = 350;

            double difference = CalculationUtilities.SmallestAngularDifference(runwayHeading, windDirection);
            double expected = -20;

            Assert.AreEqual(
                expected,
                difference,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison);
        }

    }

}