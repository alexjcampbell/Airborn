using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class CalculationUtilitiesTests
    {


        [TestMethod]
        public void Test_AngularDifference_ZerosAnd360s_ReturnZero()
        {
            Assert.AreEqual(0, CalculationUtilities.SmallestAngularDifference(0, 0));
            Assert.AreEqual(0, CalculationUtilities.SmallestAngularDifference(0, 360));
            Assert.AreEqual(0, CalculationUtilities.SmallestAngularDifference(360, 0));
            Assert.AreEqual(0, CalculationUtilities.SmallestAngularDifference(360, 360));
        }

        [TestMethod]
        public void Test_AngularDifference_BetweenZeroAnd179_ReturnPositiveNumbers()
        {
            Assert.AreEqual(45, CalculationUtilities.SmallestAngularDifference(0, 45));
            Assert.AreEqual(90, CalculationUtilities.SmallestAngularDifference(0, 90));
            Assert.AreEqual(135, CalculationUtilities.SmallestAngularDifference(0, 135));


        }

        [TestMethod]
        public void Test_AngularDifference_BetweenZeroAnd179_ReturnNegative()
        {
            Assert.AreEqual(-180, CalculationUtilities.SmallestAngularDifference(0, 180));
            Assert.AreEqual(-135, CalculationUtilities.SmallestAngularDifference(0, 225));
            Assert.AreEqual(-90, CalculationUtilities.SmallestAngularDifference(0, 270));
            Assert.AreEqual(-45, CalculationUtilities.SmallestAngularDifference(0, 315));
        }


        [TestMethod]
        public void Test_DegreesToRadians_ZeroDegrees_EqualsZeroRadians()
        {
            int numberOfDegrees = 0;
            double expectedRadians = 0;

            Assert.AreEqual(CalculationUtilities.DegreesToRadians(numberOfDegrees), expectedRadians);
        }

        [TestMethod]
        public void Test_DegreesToRadians_45Degrees_EqualsOneQuarterOfPi()
        {
            int numberOfDegrees = 45;
            double expectedRadians = 0.78f;

            Assert.AreEqual(
                expectedRadians,
                CalculationUtilities.DegreesToRadians(numberOfDegrees),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_DegreesToRadians_180Degrees_EqualsPi()
        {
            int numberOfDegrees = 180;
            double expectedRadians = 3.14f; // Pi

            Assert.AreEqual(
                expectedRadians,
                CalculationUtilities.DegreesToRadians(numberOfDegrees),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_DegreesToRadians_360Degrees_EqualsTwoPi()
        {
            int numberOfDegrees = 360;
            double expectedRadians = 6.28f; // Pi * 2

            Assert.AreEqual(
                expectedRadians,
                CalculationUtilities.DegreesToRadians(numberOfDegrees),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateHeadwindComponent_0KtsWindAt0Degrees_Equals0kts()
        {
            int windSpeed = 0;
            int windDirection = 0;
            int expectedHeadwind = 0;

            Assert.AreEqual(
                expectedHeadwind,
                CalculationUtilities.HeadwindComponent(windSpeed, windDirection),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateHeadwindComponent_15KtsWindAt0Degrees_Equals15KtsHeadwind()
        {
            int windSpeed = 15;
            int windDirection = 0;
            int expectedHeadwind = 15;

            Assert.AreEqual(
                expectedHeadwind,
                CalculationUtilities.HeadwindComponent(windSpeed, windDirection),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateHeadwindComponent_15KtsWindAt45Degrees_Equals7ktsHeadwind()
        {
            int windSpeed = 15;
            int windDirection = 45;
            double expectedHeadwind = 10.60f;

            Assert.AreEqual(
                expectedHeadwind,
                CalculationUtilities.HeadwindComponent(windSpeed, windDirection),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateHeadwindComponent_15KtsWindAt90Degrees_Equals0ktsHeadwind()
        {
            int windSpeed = 15;
            int windDirection = 90;
            double expectedHeadwind = 0f;

            Assert.AreEqual(
                expectedHeadwind,
                Math.Round(CalculationUtilities.HeadwindComponent(windSpeed, windDirection), 0),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateHeadwindComponent_15KtsWindAt135Degrees_Equals10ktsTailwind()
        {
            int windSpeed = 15;
            int windDirection = 135;
            double expectedTailwind = -10.60f;

            Assert.AreEqual(
                expectedTailwind,
                CalculationUtilities.HeadwindComponent(windSpeed, windDirection),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateHeadwindComponent_15KtsWindAt180Degrees_Equals15ktsTailwind()
        {
            int windSpeed = 15;
            int windDirection = 180;
            double expectedTailwind = -15f;

            Assert.AreEqual(
                expectedTailwind,
                CalculationUtilities.HeadwindComponent(windSpeed, windDirection),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateHeadwindComponent_15KtsWindAt225Degrees_Equals7ktsTailwind()
        {
            int windSpeed = 15;
            int windDirection = 225;
            double expectedTailwind = -10.60f;

            Assert.AreEqual(
                expectedTailwind,
                CalculationUtilities.HeadwindComponent(windSpeed, windDirection),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateHeadwindComponent_15KtsWindAt270Degrees_Equals0ktsTailwind()
        {
            int windSpeed = 15;
            int windDirection = 270;
            double expectedHeadwind = 0f;

            Assert.AreEqual(
                expectedHeadwind,
                Math.Round(CalculationUtilities.HeadwindComponent(windSpeed, windDirection), 0),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateCrosswindComponent_0ktsWindAt0Degrees_Equals0KtsCrosswind()
        {
            int windSpeed = 0;
            int windDirection = 0;
            double expectedCrosswind = 0f;

            Assert.AreEqual(
                expectedCrosswind,
                CalculationUtilities.CrosswindComponent(windSpeed, windDirection),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateCrosswindComponent_15KtsWindAt0Degrees_Equals0KtsCrosswind()
        {
            int windSpeed = 15;
            int windDirection = 0;
            double expectedCrosswind = 0f;

            Assert.AreEqual(
                expectedCrosswind,
                CalculationUtilities.CrosswindComponent(windSpeed, windDirection),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateCrosswindComponent_15KtsWindAt45Degrees_Equals10KtsCrosswind()
        {
            int windSpeed = 15;
            int windDirection = 45;
            double expectedCrosswind = 10.60f;

            Assert.AreEqual(
                expectedCrosswind,
                CalculationUtilities.CrosswindComponent(windSpeed, windDirection),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateCrosswindComponent_15KtsWindAt90Degrees_Equals15KtsCrosswind()
        {
            int windSpeed = 15;
            int windDirection = 90;
            double expectedCrosswind = 15f;

            Assert.AreEqual(
                expectedCrosswind,
                CalculationUtilities.CrosswindComponent(windSpeed, windDirection),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateCrosswindComponent_15KtsWindAt135Degrees_Equals10KtsCrosswind()
        {
            int windSpeed = 15;
            int windDirection = 135;
            double expectedCrosswind = 10.60f;

            Assert.AreEqual(
                expectedCrosswind,
                CalculationUtilities.CrosswindComponent(windSpeed, windDirection),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateCrosswindComponent_15KtsWindAt180Degrees_Equals0KtsCrosswind()
        {
            int windSpeed = 15;
            int windDirection = 180;
            double expectedCrosswind = 0f;

            Assert.AreEqual(
                expectedCrosswind,
                Math.Round(CalculationUtilities.CrosswindComponent(windSpeed, windDirection)),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateCrosswindComponent_15KtsWindAt225Degrees_Equals10KtsCrosswind()
        {
            int windSpeed = 15;
            int windDirection = 225;
            double expectedCrosswind = -10.60f;

            Assert.AreEqual(
                expectedCrosswind,
                CalculationUtilities.CrosswindComponent(windSpeed, windDirection),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateAngularDifferenceBetweenRunwayAndWind_0DegreesRunwayAnd0DegreesWind_Equals0Degrees()
        {
            int runwayHeading = 0;
            int windHeading = 0;
            double expectedDifference = 0;

            Assert.AreEqual(
                expectedDifference,
                CalculationUtilities.AngularDifferenceBetweenRunwayAndWind(runwayHeading, windHeading),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateAngularDifferenceBetweenRunwayAndWind_0DegreesRunwayAnd45DegreesWind_Equals45Degrees()
        {
            int runwayHeading = 0;
            int windHeading = 45;
            double expectedDifference = 45;

            Assert.AreEqual(
                expectedDifference,
                CalculationUtilities.AngularDifferenceBetweenRunwayAndWind(runwayHeading, windHeading),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateAngularDifferenceBetweenRunwayAndWind_0DegreesRunwayAnd90DegreesWind_Equals90Degrees()
        {
            int runwayHeading = 0;
            int windHeading = 90;
            double expectedDifference = 90;

            Assert.AreEqual(
                expectedDifference,
                CalculationUtilities.AngularDifferenceBetweenRunwayAndWind(runwayHeading, windHeading),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateAngularDifferenceBetweenRunwayAndWind_0DegreesRunwayAnd135DegreesWind_Equals135Degrees()
        {
            int runwayHeading = 0;
            int windHeading = 135;
            double expectedDifference = 135;

            Assert.AreEqual(
                expectedDifference,
                CalculationUtilities.AngularDifferenceBetweenRunwayAndWind(runwayHeading, windHeading),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateAngularDifferenceBetweenRunwayAndWind_0DegreesRunwayAnd180DegreesWind_Equals180Degrees()
        {
            int runwayHeading = 0;
            int windHeading = 180;
            double expectedDifference = -180;

            Assert.AreEqual(
                expectedDifference,
                CalculationUtilities.AngularDifferenceBetweenRunwayAndWind(runwayHeading, windHeading),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CalculateAngularDifferenceBetweenRunwayAndWind_0DegreesRunwayAnd225DegreesWind_Equals225Degrees()
        {
            int runwayHeading = 0;
            int windHeading = 225;
            double expectedDifference = -135;

            Assert.AreEqual(
                expectedDifference,
                CalculationUtilities.AngularDifferenceBetweenRunwayAndWind(runwayHeading, windHeading),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }
    }

}