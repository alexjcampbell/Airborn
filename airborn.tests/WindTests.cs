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
            Wind wind = Wind.FromMagnetic(310, 10);
            Runway runway = Runway.FromMagnetic(010);

            Assert.AreEqual(-60, CalculationUtilities.SmallestAngularDifference(10, 310));

        }

        [TestMethod]
        public void TestAngularDifference_WindRightOfRunway()
        {
            Assert.AreEqual(20, CalculationUtilities.SmallestAngularDifference(350, 10));
        }

        [TestMethod]
        public void TestAngularDifference_NoDifference()
        {
            int runwayHeading = 0;
            int windDirection = 0;

            Assert.AreEqual(0, CalculationUtilities.SmallestAngularDifference(runwayHeading, windDirection));

        }

        [TestMethod]
        public void TestAngularDifference_180DegreeDifference()
        {
            int runwayHeading = 180;
            int windDirection = 0;

            Assert.AreEqual(-180, CalculationUtilities.SmallestAngularDifference(runwayHeading, windDirection));
        }

        [TestMethod]
        public void TestAngularDifference_WindGreaterThan180()
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

            Assert.AreEqual(-20, CalculationUtilities.SmallestAngularDifference(runwayHeading, windDirection));
        }

    }

}