using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class CalculationUtilitiesTests_Altitudes
    {


        [TestMethod]
        public void TestCalculatePressureAltitude_3092Hg_Equals1047Hpa()
        {
            decimal qnh = 1047.115307485672m;
            decimal altimeter = 30.92m;

            Assert.AreEqual(
                CalculationUtilities.PressureAltitudeAtFieldElevation(qnh, 0),
                CalculationUtilities.PressureAltitudeAtFieldElevation(
                    CalculationUtilities.InchesOfMercuryToMillibars(altimeter), 0)
                );
        }

        [TestMethod]
        public void TestGetPressureAltitudeAtAirport_0ft_Equals0ft()
        {
            int elevation = 0;
            decimal qnh = 1013.25m;
            int expectedPressureAltitude = 0;

            Assert.AreEqual(
                expectedPressureAltitude,
                CalculationUtilities.PressureAltitudeAtFieldElevation(qnh, elevation)
                );
        }

        [TestMethod]
        public void TestDensityAltitudeAtSeaLevelISA_Equals_PressureAltitude()
        {
            decimal temperature = 15m;
            decimal expectedDensityAltitude = 0;

            Assert.AreEqual(
                expectedDensityAltitude,
                CalculationUtilities.DensityAltitudeAtAirport(temperature, 15, 0)
                );
        }

        [TestMethod]
        public void TestDensityAltitudeAt1000ftISA_Equals_PressureAltitude()
        {
            decimal temperature = 13m;
            decimal expectedDensityAltitude = 1000;

            Assert.AreEqual(
                expectedDensityAltitude,
                CalculationUtilities.DensityAltitudeAtAirport(temperature, 13, 1000)
                );
        }

        [TestMethod]
        public void TestDensityAltitudeAtSeaLevelAtISAMinus2Degrees_IsCorrect()
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