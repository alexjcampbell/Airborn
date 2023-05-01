using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class RunwayTests
    {
        [TestMethod]
        public void LandingDistanceIncludesDisplacedThreshold()
        {
            Runway runway = new Runway("28");

            // regrettably, yes, these are stored as text in the database
            runway.RunwayLength = "5000";
            runway.DisplacedThresholdFt = "1000";

            Assert.AreEqual(4000, runway.LandingAvailableLength.TotalFeet);
        }

        [TestMethod]
        public void TestRunwayAtSedonaIs1point77()
        {
            double elevation1 = 4736;
            double elevation2 = 4827;
            double runwayLength = 5132;
            double expectedSlope = 1.77;

            double slope = Runway.CalculateSlope(elevation1, elevation2, runwayLength);
            Assert.AreEqual(expectedSlope, slope, 0.01);
        }

        [TestMethod]
        public void TestRunway21IsMinus1point77()
        {
            double elevation1 = 4827;
            double elevation2 = 4736;
            double runwayLength = 5132;
            double expectedSlope = -1.77;

            double slope = Runway.CalculateSlope(elevation1, elevation2, runwayLength);
            Assert.AreEqual(expectedSlope, slope, 0.01);
        }

        [TestMethod]
        public void TestSameElevationHasNoSlope()
        {
            double elevation1 = 5000;
            double elevation2 = 5000;
            double runwayLength = 10000;
            double expectedSlope = 0;

            double slope = Runway.CalculateSlope(elevation1, elevation2, runwayLength);
            Assert.AreEqual(expectedSlope, slope, 0.01);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestZeroLengthThrowsArgumentException()
        {
            double elevation1 = 0;
            double elevation2 = 100;
            double runwayLength = 0;

            Runway.CalculateSlope(elevation1, elevation2, runwayLength);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNegativeLengthThrowsArgumentExpection()
        {
            double elevation1 = 0;
            double elevation2 = 100;
            double runwayLength = -10;

            Runway.CalculateSlope(elevation1, elevation2, runwayLength);
        }
    }
}
