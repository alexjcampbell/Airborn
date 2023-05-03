using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class CalculationUtilitiesTests_Temperatures
    {

        [TestMethod]
        public void Test_FahrenheitToCelcius_ZeroDegreesFahrenheit_EqualsMinus17Point()
        {
            int numberOfDegreesFahrenheit = 0;
            decimal expectedDegreesCelcius = -17.777777777777777777777777778m;

            Assert.AreEqual(
                expectedDegreesCelcius,
                CalculationUtilities.FahrenheitToCelcius(numberOfDegreesFahrenheit)
                );
        }

        [TestMethod]
        public void Test_FahrenheitToCelcius_32DegreesFahrenheit_EqualsZeroDegreesCelcius()
        {
            int numberOfDegreesFahrenheit = 32;
            decimal expectedDegreesCelcius = 0m;

            Assert.AreEqual(
                expectedDegreesCelcius,
                CalculationUtilities.FahrenheitToCelcius(numberOfDegreesFahrenheit)
                );
        }

        [TestMethod]
        public void Test_FahrenheitToCelcius_50DegreesFahrenheit_Equals10DegreesCelcius()
        {
            int numberOfDegreesFahrenheit = 50;
            decimal expectedDegreesCelcius = 10m;

            Assert.AreEqual(
                expectedDegreesCelcius,
                CalculationUtilities.FahrenheitToCelcius(numberOfDegreesFahrenheit)
                );
        }

        [TestMethod]
        public void Test_FahrenheitToCelcius_100DegreesFahrenheit_Equals37ishDegreesCelcius()
        {
            int numberOfDegreesFahrenheit = 100;
            decimal expectedDegreesCelcius = 37.777777777777777777777777778m;

            Assert.AreEqual(
                expectedDegreesCelcius,
                CalculationUtilities.FahrenheitToCelcius(numberOfDegreesFahrenheit)
                );
        }

        public void Test_CalculateISATemperatureCelciusForPressureAltitude_0ft_Equals15C()
        {
            int pressureAltitude = 0;
            decimal expectedTemperature = 15m;

            Assert.AreEqual(
                expectedTemperature,
                CalculationUtilities.ISATemperatureForPressureAltitude(pressureAltitude)
                );
        }

        [TestMethod]
        public void Test_CalculateISATemperatureCelciusForPressureAltitude_1000ft_Equals13C()
        {
            int pressureAltitude = 1000;
            decimal expectedTemperature = 13;

            Assert.AreEqual(
                expectedTemperature,
                CalculationUtilities.ISATemperatureForPressureAltitude(pressureAltitude)
                );
        }

        [TestMethod]
        public void Test_CalculateISATemperatureCelciusForPressureAltitude_10000ft_Equals_Minus1C()
        {
            // lapse rate is 2 degrees per 1000ft = 20 degrees per 10000ft
            // ISA temperature at sea level is 15 degrees celcius
            // so ISA temperature at 10000ft is 15 - 20 = -5 degrees celcius
            int pressureAltitude = 10000;
            decimal expectedTemperature = -5;

            Assert.AreEqual(
                expectedTemperature,
                CalculationUtilities.ISATemperatureForPressureAltitude(pressureAltitude)
                );
        }

        [TestMethod]
        public void Test_DensityAltitudeAtSeaLevel_AtISAMinus2Degrees_IsCorrect()
        {
            decimal temperature = 13;
            decimal expectedDensityAltitude = -240;
            decimal qnh = 1013.25m;

            decimal pressureAltitude = CalculationUtilities.PressureAltitudeAtFieldElevation(qnh, 0);
            decimal isaTemperature = CalculationUtilities.ISATemperatureForPressureAltitude(pressureAltitude);

            Assert.AreEqual(
                expectedDensityAltitude,
                CalculationUtilities.DensityAltitudeAtAirport(temperature, isaTemperature, pressureAltitude)
                );
        }

    }

}