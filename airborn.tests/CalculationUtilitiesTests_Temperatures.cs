using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class CalculationUtilitiesTests_Temperatures
    {

        [TestMethod]
        public void TestFahrenheitToCelcius_ZeroDegreesFahrenheitEqualsMinus17Point()
        {
            int numberOfDegreesFahrenheit = 0;
            decimal expectedDegreesCelcius = -17.777777777777777777777777778m;

            Assert.AreEqual(
                expectedDegreesCelcius,
                CalculationUtilities.ConvertFahrenheitToCelcius(numberOfDegreesFahrenheit)
                );
        }

        [TestMethod]
        public void TestFahrenheitToCelcius_32DegreesFahrenheitEqualsZeroDegreesCelcius()
        {
            int numberOfDegreesFahrenheit = 32;
            decimal expectedDegreesCelcius = 0m;

            Assert.AreEqual(
                expectedDegreesCelcius,
                CalculationUtilities.ConvertFahrenheitToCelcius(numberOfDegreesFahrenheit)
                );
        }

        [TestMethod]
        public void TestFahrenheitToCelcius_50DegreesFahrenheitEquals10DegreesCelcius()
        {
            int numberOfDegreesFahrenheit = 50;
            decimal expectedDegreesCelcius = 10m;

            Assert.AreEqual(
                expectedDegreesCelcius,
                CalculationUtilities.ConvertFahrenheitToCelcius(numberOfDegreesFahrenheit)
                );
        }

        [TestMethod]
        public void TestFahrenheitToCelcius_100DegreesFahrenheitEquals37ishDegreesCelcius()
        {
            int numberOfDegreesFahrenheit = 100;
            decimal expectedDegreesCelcius = 37.777777777777777777777777778m;

            Assert.AreEqual(
                expectedDegreesCelcius,
                CalculationUtilities.ConvertFahrenheitToCelcius(numberOfDegreesFahrenheit)
                );
        }

        public void TestCalculateISATemperatureCelciusForPressureAltitude_0ftEquals15C()
        {
            int pressureAltitude = 0;
            decimal expectedTemperature = 15m;

            Assert.AreEqual(
                expectedTemperature,
                CalculationUtilities.CalculateISATemperatureForPressureAltitude(pressureAltitude)
                );
        }

        [TestMethod]
        public void TestCalculateISATemperatureCelciusForPressureAltitude_1000ftEquals13C()
        {
            int pressureAltitude = 1000;
            decimal expectedTemperature = 13;

            Assert.AreEqual(
                expectedTemperature,
                CalculationUtilities.CalculateISATemperatureForPressureAltitude(pressureAltitude)
                );
        }

        [TestMethod]
        public void TestCalculateISATemperatureCelciusForPressureAltitude_10000ftEqualsMinus1C()
        {
            int pressureAltitude = 10000;
            decimal expectedTemperature = -5;

            Assert.AreEqual(
                expectedTemperature,
                CalculationUtilities.CalculateISATemperatureForPressureAltitude(pressureAltitude)
                );
        }

        [TestMethod]
        public void TestDensityAltitudeAtSeaLevelAtISAMinus2DegreesIsCorrect()
        {
            decimal temperature = 13;
            decimal expectedDensityAltitude = -240;
            decimal qnh = 1013.25m;

            decimal pressureAltitude = CalculationUtilities.CalculatePressureAltitudeAtFieldElevation(qnh, 0);
            decimal isaTemperature = CalculationUtilities.CalculateISATemperatureForPressureAltitude(pressureAltitude);

            Assert.AreEqual(
                expectedDensityAltitude,
                CalculationUtilities.CalculateDensityAltitudeAtAirport(temperature, isaTemperature, pressureAltitude)
                );
        }

    }

}