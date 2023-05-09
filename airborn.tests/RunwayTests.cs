using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class RunwayTests
    {


        [TestMethod]
        public void Test_GetRunwayHeading_10R_Returns100()
        {
            Runway runway = new Runway(new Airport(), new Direction(100, 10), "10R");
            Direction expectedHeading = new Direction(100, 0);

            Assert.AreEqual(expectedHeading, runway.Runway_Heading_Magnetic);
        }

        [TestMethod]
        public void Test_GetRunwayHeading_28_Returns280()
        {
            Runway runway = new Runway(new Airport(), new Direction(280, 10), "28");
            Direction expectedHeading = new Direction(280, 0);

            Assert.AreEqual(expectedHeading, runway.Runway_Heading_Magnetic);
        }

        [TestMethod]
        public void Test_GetRunwayHeading_1C_Returns10()
        {
            Runway runway = new Runway(new Airport(), new Direction(10, 10), "1C");
            Direction expectedHeading = new Direction(10, 0);

            Assert.AreEqual(expectedHeading, runway.Runway_Heading_Magnetic);
        }


        [TestMethod]
        public void Test_GetRunwayHeading_01C_Returns10()
        {
            Runway runway = new Runway(new Airport(), new Direction(10, 10), "01C");
            Direction expectedHeading = new Direction(10, 0);

            Assert.AreEqual(expectedHeading, runway.Runway_Heading_Magnetic);
        }

        [TestMethod]
        public void Test_GetRunwayHeading_36R_Returns360()
        {
            Runway runway = new Runway(new Airport(), new Direction(360, 10), "36R");
            Direction expectedHeading = new Direction(360, 0);

            Assert.AreEqual(expectedHeading, runway.Runway_Heading_Magnetic);
        }

        [TestMethod]
        public void Test_GetRunwayHeading_InvalidRunwayName_ReturnsNull()
        {
            Runway runway = new Runway(new Airport(), new Direction(10, 10), "INVALID");

            var result = runway.GetRunwayHeadingFromRunwayName(runway);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Test_RunwayAtSedona_Is1point77()
        {
            double elevation1 = 4736;
            double elevation2 = 4827;
            double runwayLength = 5132;
            double expectedSlope = 1.77;

            double slope = Runway.CalculateSlope(elevation1, elevation2, runwayLength);
            Assert.AreEqual(expectedSlope, slope, 0.01);
        }

        [TestMethod]
        public void Test_Runway21_IsMinus1point77()
        {
            double elevation1 = 4827;
            double elevation2 = 4736;
            double runwayLength = 5132;
            double expectedSlope = -1.77;

            double slope = Runway.CalculateSlope(elevation1, elevation2, runwayLength);
            Assert.AreEqual(expectedSlope, slope, 0.01);
        }

        [TestMethod]
        public void Test_SameElevation_HasNoSlope()
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
        public void Test_CalculateSlopeWithZeroRunwayLength_ThrowsArgumentException()
        {
            double elevation1 = 0;
            double elevation2 = 100;
            double runwayLength = 0;

            Runway.CalculateSlope(elevation1, elevation2, runwayLength);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CalculateSlopeWithNegativeLengthThrowsArgumentExpection()
        {
            double elevation1 = 0;
            double elevation2 = 100;
            double runwayLength = -10;

            Runway.CalculateSlope(elevation1, elevation2, runwayLength);
        }

        [TestMethod]
        public void Test_GetOppositeRunway_ReturnsCorrectOppositeRunways()
        {
            Assert.AreEqual("18R", Runway.GetOppositeRunway("36L"));
            Assert.AreEqual("9L", Runway.GetOppositeRunway("27R"));
            Assert.AreEqual("27C", Runway.GetOppositeRunway("09C"));
        }

        [TestMethod]
        public void Test_RunwayHeading_ReturnsCorrectRunwayHeading_WhenGiven10R()
        {
            Runway runway = new Runway(new Airport(), new Direction(100, 10), "10R");
            Assert.AreEqual(100, runway.Runway_Heading_Magnetic.Value.DirectionTrue);
        }

        [TestMethod]
        public void Test_RunwayHeading_ReturnsCorrectRunwayHeading_WhenGiven10()
        {
            Runway runway = new Runway(new Airport(), new Direction(100, 10), "10");
            Assert.AreEqual(100, runway.Runway_Heading_Magnetic.Value.DirectionTrue);
        }

        [TestMethod]
        public void Test_RunwayLengthFriendly_ReturnsCorrectRunwayLengthAndWidthInformation()
        {
            Runway runway = new Runway(new Airport(), new Direction(100, 0), "1C")
            {
                RunwayLength = 10000,
                RunwayWidth = 150
            };
            Assert.AreEqual("10,000 ft x 150 ft", runway.RunwayLengthFriendly);
        }

        [TestMethod]
        public void Test_RunwayLengthFriendly_ReturnsUnknown_WhenRunwayLengthIsNotAvailable()
        {
            Runway runway = new Runway(new Airport(), new Direction(10, 0), "1C")
            {
                RunwayLength = null,
                RunwayWidth = 150
            };
            Assert.AreEqual("Unknown", runway.RunwayLengthFriendly);
        }

        [TestMethod]
        public void Test_TakeoffAvailableLength_DoesNotIncludeDisplacedThreshold()
        {
            Runway runway = new Runway(new Airport(), new Direction(10, 0), "1C")
            {
                RunwayLength = 10000,
                DisplacedThresholdFt = 1000
            };
            Assert.AreEqual(10000, runway.RunwayLength);
        }

        [TestMethod]
        public void Test_LandingAvailableLength_ReturnsCorrectLength_WhenOnlyRunwayLengthIsAvailable()
        {
            Runway runway = new Runway(new Airport(), new Direction(10, 0), "1C")
            {
                RunwayLength = 9000,
                DisplacedThresholdFt = 1000
            };
            Assert.AreEqual(8000, runway.LandingAvailableLength.TotalFeet);
        }

        [TestMethod]
        public void Test_LandingAvailableLength_WhenBothRunwayLengthAndDisplacedThresholdAreAvailable_ReturnsCorrectLength_()
        {
            Runway runway = new Runway(new Airport(), new Direction(10, 0), "1C")
            {
                RunwayLength = 10000,
                DisplacedThresholdFt = 1000
            };
            Assert.AreEqual(9000, runway.LandingAvailableLength.TotalFeet);
        }

        [TestMethod]
        public void Test_WhenOnlyRunwayLengthAvailable_LandingLengthAvailableEqualsRunwayLength()
        {
            Runway runway = new Runway(new Airport(), new Direction(10, 0), "1C")
            {
                RunwayLength = 10000,
                DisplacedThresholdFt = null
            };
            Assert.AreEqual(10000, runway.LandingAvailableLength.TotalFeet);
        }

    }
}
