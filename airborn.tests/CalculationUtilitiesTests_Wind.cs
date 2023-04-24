using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class CalculationUtilitiesTests
    {


        [TestMethod]
        public void TestAngularDifference_ZerosAnd360sReturnZero()
        {
            Assert.AreEqual(0, CalculationUtilities.SmallestAngularDifference(0, 0));
            Assert.AreEqual(0, CalculationUtilities.SmallestAngularDifference(0, 360));
            Assert.AreEqual(0, CalculationUtilities.SmallestAngularDifference(360, 0));
            Assert.AreEqual(0, CalculationUtilities.SmallestAngularDifference(360, 360));
        }

        [TestMethod]
        public void TestAngularDifference_BetweenZeroAnd179ReturnPositiveNumbers()
        {
            Assert.AreEqual(45, CalculationUtilities.SmallestAngularDifference(0, 45));
            Assert.AreEqual(90, CalculationUtilities.SmallestAngularDifference(0, 90));
            Assert.AreEqual(135, CalculationUtilities.SmallestAngularDifference(0, 135));


        }

        [TestMethod]
        public void TestAngularDifference_BetweenZeroAnd179ReturnNegative()
        {
            Assert.AreEqual(-180, CalculationUtilities.SmallestAngularDifference(0, 180));
            Assert.AreEqual(-135, CalculationUtilities.SmallestAngularDifference(0, 225));
            Assert.AreEqual(-90, CalculationUtilities.SmallestAngularDifference(0, 270));
            Assert.AreEqual(-45, CalculationUtilities.SmallestAngularDifference(0, 315));
        }


        [TestMethod]
        public void TestDegreesToRadians_ZeroDegreesEqualsZeroRadians()
        {
            int numberOfDegrees = 0;
            decimal expectedRadians = 0;

            Assert.AreEqual(CalculationUtilities.DegreesToRadians(numberOfDegrees), expectedRadians);
        }

        [TestMethod]
        public void TestDegreesToRadians_45DegreesEqualsOneQuarterOfPi()
        {
            int numberOfDegrees = 45;
            decimal expectedRadians = 0.7853981633974475000000000010m;

            Assert.AreEqual(
                expectedRadians,
                CalculationUtilities.DegreesToRadians(numberOfDegrees)
                );
        }

        [TestMethod]
        public void TestDegreesToRadians_180DegreesEqualsPi()
        {
            int numberOfDegrees = 180;
            decimal expectedRadians = 3.1415926535897900000000000040m; // Pi

            Assert.AreEqual(
                expectedRadians,
                CalculationUtilities.DegreesToRadians(numberOfDegrees)
                );
        }

        [TestMethod]
        public void TestDegreesToRadians_360DegreesEqualsTwoPi()
        {
            int numberOfDegrees = 360;
            decimal expectedRadians = 6.2831853071795800000000000080m; // Pi * 2

            Assert.AreEqual(expectedRadians, CalculationUtilities.DegreesToRadians(numberOfDegrees));
        }

        [TestMethod]
        public void TestCalculateHeadwindComponent_0KtsWindAt0DegreesEquals0kts()
        {
            int windSpeed = 0;
            int windDirection = 0;
            int expectedHeadwind = 0;

            Assert.AreEqual(
                expectedHeadwind,
                CalculationUtilities.HeadwindComponent(windSpeed, windDirection)
                );
        }

        [TestMethod]
        public void TestCalculateHeadwindComponent_15KtsWindAt0DegreesEquals15KtsHeadwind()
        {
            int windSpeed = 15;
            int windDirection = 0;
            int expectedHeadwind = 15;

            Assert.AreEqual(
                expectedHeadwind,
                CalculationUtilities.HeadwindComponent(windSpeed, windDirection)
                );
        }

        [TestMethod]
        public void TestCalculateHeadwindComponent_15KtsWindAt45DegreesEquals7ktsHeadwind()
        {
            int windSpeed = 15;
            int windDirection = 45;
            decimal expectedHeadwind = 10.6066017177982m;

            Assert.AreEqual(
                expectedHeadwind,
                CalculationUtilities.HeadwindComponent(windSpeed, windDirection)
                );
        }

        [TestMethod]
        public void TestCalculateHeadwindComponent_15KtsWindAt90DegreesEquals0ktsHeadwind()
        {
            int windSpeed = 15;
            int windDirection = 90;
            decimal expectedHeadwind = 0m;

            Assert.AreEqual(
                expectedHeadwind,
                Math.Round(CalculationUtilities.HeadwindComponent(windSpeed, windDirection), 0)
                );
        }

        [TestMethod]
        public void TestCalculateHeadwindComponent_15KtsWindAt135DegreesEquals10ktsTailwind()
        {
            int windSpeed = 15;
            int windDirection = 135;
            decimal expectedTailwind = -10.6066017177982m;

            Assert.AreEqual(
                expectedTailwind,
                CalculationUtilities.HeadwindComponent(windSpeed, windDirection)
                );
        }

        [TestMethod]
        public void TestCalculateHeadwindComponent_15KtsWindAt180DegreesEquals15ktsTailwind()
        {
            int windSpeed = 15;
            int windDirection = 180;
            decimal expectedTailwind = -15m;

            Assert.AreEqual(
                expectedTailwind,
                CalculationUtilities.HeadwindComponent(windSpeed, windDirection)
                );
        }

        [TestMethod]
        public void TestCalculateHeadwindComponent_15KtsWindAt225DegreesEquals7ktsTailwind()
        {
            int windSpeed = 15;
            int windDirection = 225;
            decimal expectedTailwind = -10.6066017177983m;

            Assert.AreEqual(
                expectedTailwind,
                CalculationUtilities.HeadwindComponent(windSpeed, windDirection)
                );
        }

        [TestMethod]
        public void TestCalculateHeadwindComponent_15KtsWindAt270DegreesEquals0ktsTailwind()
        {
            int windSpeed = 15;
            int windDirection = 270;
            decimal expectedHeadwind = 0m;

            Assert.AreEqual(
                expectedHeadwind,
                Math.Round(CalculationUtilities.HeadwindComponent(windSpeed, windDirection), 0)
                );
        }

        [TestMethod]
        public void TestCalculateCrosswindComponent_0ktsWindAt0DegreesEquals0KtsCrosswind()
        {
            int windSpeed = 0;
            int windDirection = 0;
            decimal expectedCrosswind = 0m;

            Assert.AreEqual(
                expectedCrosswind,
                CalculationUtilities.CrosswindComponent(windSpeed, windDirection)
                );
        }

        [TestMethod]
        public void TestCalculateCrosswindComponent_15KtsWindAt0DegreesEquals0KtsCrosswind()
        {
            int windSpeed = 15;
            int windDirection = 0;
            decimal expectedCrosswind = 0m;

            Assert.AreEqual(
                expectedCrosswind,
                CalculationUtilities.CrosswindComponent(windSpeed, windDirection)
                );
        }

        [TestMethod]
        public void TestCalculateCrosswindComponent_15KtsWindAt45DegreesEquals10KtsCrosswind()
        {
            int windSpeed = 15;
            int windDirection = 45;
            decimal expectedCrosswind = 10.6066017177982m;

            Assert.AreEqual(
                expectedCrosswind,
                CalculationUtilities.CrosswindComponent(windSpeed, windDirection)
                );
        }

        [TestMethod]
        public void TestCalculateCrosswindComponent_15KtsWindAt90DegreesEquals15KtsCrosswind()
        {
            int windSpeed = 15;
            int windDirection = 90;
            decimal expectedCrosswind = 15m;

            Assert.AreEqual(
                expectedCrosswind,
                CalculationUtilities.CrosswindComponent(windSpeed, windDirection)
                );
        }

        [TestMethod]
        public void TestCalculateCrosswindComponent_15KtsWindAt135DegreesEquals10KtsCrosswind()
        {
            int windSpeed = 15;
            int windDirection = 135;
            decimal expectedCrosswind = 10.6066017177982m;

            Assert.AreEqual(
                expectedCrosswind,
                CalculationUtilities.CrosswindComponent(windSpeed, windDirection)
                );
        }

        [TestMethod]
        public void TestCalculateCrosswindComponent_15KtsWindAt180DegreesEquals0KtsCrosswind()
        {
            int windSpeed = 15;
            int windDirection = 180;
            decimal expectedCrosswind = 0m;

            Assert.AreEqual(
                expectedCrosswind,
                Math.Round(CalculationUtilities.CrosswindComponent(windSpeed, windDirection))
                );
        }

        [TestMethod]
        public void TestCalculateCrosswindComponent_15KtsWindAt225DegreesEquals10KtsCrosswind()
        {
            int windSpeed = 15;
            int windDirection = 225;
            decimal expectedCrosswind = -10.6066017177982m;

            Assert.AreEqual(
                expectedCrosswind,
                CalculationUtilities.CrosswindComponent(windSpeed, windDirection)
                );
        }

        [TestMethod]
        public void TestCalculateAngularDifferenceBetweenRunwayAndWind_0DegreesRunwayAnd0DegreesWindEquals0Degrees()
        {
            int runwayHeading = 0;
            int windHeading = 0;
            decimal expectedDifference = 0;

            Assert.AreEqual(
                expectedDifference,
                CalculationUtilities.AngularDifferenceBetweenRunwayAndWind(runwayHeading, windHeading)
                );
        }

        [TestMethod]
        public void TestCalculateAngularDifferenceBetweenRunwayAndWind_0DegreesRunwayAnd45DegreesWindEquals45Degrees()
        {
            int runwayHeading = 0;
            int windHeading = 45;
            decimal expectedDifference = 45;

            Assert.AreEqual(
                expectedDifference,
                CalculationUtilities.AngularDifferenceBetweenRunwayAndWind(runwayHeading, windHeading)
                );
        }

        [TestMethod]
        public void TestCalculateAngularDifferenceBetweenRunwayAndWind_0DegreesRunwayAnd90DegreesWindEquals90Degrees()
        {
            int runwayHeading = 0;
            int windHeading = 90;
            decimal expectedDifference = 90;

            Assert.AreEqual(
                expectedDifference,
                CalculationUtilities.AngularDifferenceBetweenRunwayAndWind(runwayHeading, windHeading)
                );
        }

        [TestMethod]
        public void TestCalculateAngularDifferenceBetweenRunwayAndWind_0DegreesRunwayAnd135DegreesWindEquals135Degrees()
        {
            int runwayHeading = 0;
            int windHeading = 135;
            decimal expectedDifference = 135;

            Assert.AreEqual(
                expectedDifference,
                CalculationUtilities.AngularDifferenceBetweenRunwayAndWind(runwayHeading, windHeading)
                );
        }

        [TestMethod]
        public void TestCalculateAngularDifferenceBetweenRunwayAndWind_0DegreesRunwayAnd180DegreesWindEquals180Degrees()
        {
            int runwayHeading = 0;
            int windHeading = 180;
            decimal expectedDifference = -180;

            Assert.AreEqual(
                expectedDifference,
                CalculationUtilities.AngularDifferenceBetweenRunwayAndWind(runwayHeading, windHeading)
                );
        }

        [TestMethod]
        public void TestCalculateAngularDifferenceBetweenRunwayAndWind_0DegreesRunwayAnd225DegreesWindEquals225Degrees()
        {
            int runwayHeading = 0;
            int windHeading = 225;
            decimal expectedDifference = -135;

            Assert.AreEqual(
                expectedDifference,
                CalculationUtilities.AngularDifferenceBetweenRunwayAndWind(runwayHeading, windHeading)
                );
        }
    }

}