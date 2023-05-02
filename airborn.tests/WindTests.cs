using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class WindTests
    {


        [TestMethod]
        public void TestAngularDifference_WindLeftOfRunway()
        {
            int magneticVariation = 0;

            Wind wind = Wind.FromMagnetic(310, magneticVariation);
            Runway runway = Runway.FromMagnetic(10, magneticVariation);

            Assert.AreEqual(-60, CalculationUtilities.SmallestAngularDifference(10, 310));

        }

        [TestMethod]
        public void TestAngularDifference_WindRightOfRunway()
        {
            int magneticVariation = 0;

            Wind wind = Wind.FromMagnetic(010, magneticVariation);
            Runway runway = Runway.FromMagnetic(310, magneticVariation);

            decimal expected = -60;

            Assert.AreEqual(expected,
                CalculationUtilities.SmallestAngularDifference(
                    wind.Direction.DirectionMagnetic,
                    runway.RunwayHeading.DirectionMagnetic)
                );
        }

        [TestMethod]
        public void TestAngularDifference_NoDifference()
        {
            int runwayHeading = 0;
            int windDirection = 0;

            decimal expected = 0;

            Assert.AreEqual(expected, CalculationUtilities.SmallestAngularDifference(runwayHeading, windDirection));

        }

        [TestMethod]
        public void TestAngularDifference_180DegreeDifference()
        {
            int runwayHeading = 180;
            int windDirection = 0;

            Assert.AreEqual(-180, CalculationUtilities.SmallestAngularDifference(runwayHeading, windDirection));
        }

        [TestMethod]
        public void TestAngularDifference_WindGreaterThan180Degrees()
        {
            int runwayHeading = 160;
            int windDirection = 10;

            Assert.AreEqual(-150, CalculationUtilities.SmallestAngularDifference(runwayHeading, windDirection));
        }

        [TestMethod]
        public void TestAngularDifference_NearNorthButWest()
        {
            int runwayHeading = 350;
            int windDirection = 10;

            Assert.AreEqual(20, CalculationUtilities.SmallestAngularDifference(runwayHeading, windDirection));
        }

        [TestMethod]
        public void TestAngularDifference_NearNorthButEast()
        {
            int runwayHeading = 10;
            int windDirection = 350;

            decimal difference = CalculationUtilities.SmallestAngularDifference(runwayHeading, windDirection);
            decimal expected = -20;

            Assert.AreEqual(expected, difference);
        }

    }

}