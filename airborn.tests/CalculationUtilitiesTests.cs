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
            Assert.AreEqual(0, CalculationUtilities.AngularDifference(0, 0));
            Assert.AreEqual(0, CalculationUtilities.AngularDifference(0, 360));
            Assert.AreEqual(0, CalculationUtilities.AngularDifference(360, 0));
            Assert.AreEqual(0, CalculationUtilities.AngularDifference(360, 360));
        }

        [TestMethod]
        public void TestAngularDifference_BetweenZeroAnd179ReturnPositiveNumbers()
        {
            Assert.AreEqual(45, CalculationUtilities.AngularDifference(0, 45));
            Assert.AreEqual(90, CalculationUtilities.AngularDifference(0, 90));
            Assert.AreEqual(135, CalculationUtilities.AngularDifference(0, 135));


        }

        [TestMethod]
        public void TestAngularDifference_BetweenZeroAnd179ReturnNegative()
        {
            Assert.AreEqual(-180, CalculationUtilities.AngularDifference(0, 180));
            Assert.AreEqual(-135, CalculationUtilities.AngularDifference(0, 225));
            Assert.AreEqual(-90, CalculationUtilities.AngularDifference(0, 270));
            Assert.AreEqual(-45, CalculationUtilities.AngularDifference(0, 315));
        }


        [TestMethod]
        public void TestDegreesToRadians_ZeroDegreesEqualsZeroRadians()
        {
            int numberOfDegrees = 0;
            decimal expectedRadians = 0;

            Assert.AreEqual(CalculationUtilities.ConvertDegreesToRadians(numberOfDegrees), expectedRadians);
        }

        [TestMethod]
        public void TestDegreesToRadians_45DegreesEqualsOneQuarterOfPi()
        {
            int numberOfDegrees = 45;
            decimal expectedRadians = 0.7853981633974475000000000010m;

            Assert.AreEqual(
                expectedRadians,
                CalculationUtilities.ConvertDegreesToRadians(numberOfDegrees)
                );
        }

        [TestMethod]
        public void TestDegreesToRadians_180DegreesEqualsPi()
        {
            int numberOfDegrees = 180;
            decimal expectedRadians = 3.1415926535897900000000000040m; // Pi
            
            Assert.AreEqual(
                expectedRadians,
                CalculationUtilities.ConvertDegreesToRadians(numberOfDegrees)
                );
        }

               [TestMethod]
        public void TestDegreesToRadians_360DegreesEqualsTwoPi()
        {
            int numberOfDegrees = 360;
            decimal expectedRadians = 6.2831853071795800000000000080m; // Pi * 2
            
            Assert.AreEqual(expectedRadians, CalculationUtilities.ConvertDegreesToRadians(numberOfDegrees));
        } 

        // ConvertFahrenheitToCelcius
        // 0 degrees fahrenheit
        // 32 degrees fahrenheit
        // 50 degrees fahrenheit
        // 100 degrees fahrenheit

        // ConvertInchesOfMercuryToMillibars
        // 29.92 inches of mercury

        //CalculateHeadwindComponent
        // CalculateISATemperatureForPressureAltitude
        // sea level
        // 1000 ft
        // 10000 ft

        //CalculateHeadwindComponent
        // 15 kts
        // 0 degrees
        // 45 degrees
        // 90 degrees
        // 135 degrees
        // 180 degrees
        // 225 degrees
        // 270 degrees
        // 315 degrees
        // 360 degrees

        // 0 kts
        // 0 degrees
        // 45 degrees
        // 90 degrees
        // 135 degrees
        // 180 degrees
        // 225 degrees
        // 270 degrees
        // 315 degrees
        // 360 degrees

        //CalculateCrosswindComponent
        // 15 kts
        // 0 degrees
        // 45 degrees
        // 90 degrees
        // 135 degrees
        // 180 degrees
        // 225 degrees
        // 270 degrees
        // 315 degrees
        // 360 degrees

        // 0 kts
        // 0 degrees
        // 45 degrees
        // 90 degrees
        // 135 degrees
        // 180 degrees
        // 225 degrees
        // 270 degrees
        // 315 degrees
        // 360 degrees        

        // CalculateAngularDifferenceBetweenRunwayAndWind
        // Runway heading 0 degrees
        // Wind heading 0 degrees
        // Wind heading 45 degrees
        // Wind heading 90 degrees
        // Wind heading 135 degrees
        // Wind heading 180 degrees
        // Wind heading 225 degrees
        // Wind heading 270 degrees
        // Wind heading 315 degrees
        // Wind heading 360 degrees

        // GetPressureAltitudeAtAirport



    }

}