using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class CalculationUtilitiesTests_Altitudes
    {


        [TestMethod]
        public void TestCalculatePressureAltitude_3092HgEquals1047Hpa()
        {
            decimal qnh = 1047.115307485672m;
            decimal altimeter = 30.92m;

            Assert.AreEqual(
                CalculationUtilities.CalculatePressureAltitudeAtFieldElevation(qnh, 0),
                CalculationUtilities.CalculatePressureAltitudeAtFieldElevation(
                    CalculationUtilities.ConvertInchesOfMercuryToMillibars(altimeter), 0)
                );
        }

        [TestMethod]
        public void TestGetPressureAltitudeAtAirport_0ftEquals0ft()
        {
            int elevation = 0;
            decimal qnh = 1013.25m;
            int expectedPressureAltitude = 0;

            Assert.AreEqual(
                expectedPressureAltitude,
                CalculationUtilities.CalculatePressureAltitudeAtFieldElevation(qnh, elevation)
                );
        }

        [TestMethod]
        public void TestDensityAltitudeAtSeaLevelISA_EqualsPressureAltitude()
        {
            decimal temperature = 15m;
            decimal expectedDensityAltitude = 0;

            Assert.AreEqual(
                expectedDensityAltitude,
                CalculationUtilities.CalculateDensityAltitudeAtAirport(temperature, 15, 0)
                );
        }

        [TestMethod]
        public void TestDensityAltitudeAt1000ftISA_EqualsPressureAltitude()
        {
            decimal temperature = 13m;
            decimal expectedDensityAltitude = 1000;

            Assert.AreEqual(
                expectedDensityAltitude,
                CalculationUtilities.CalculateDensityAltitudeAtAirport(temperature, 13, 1000)
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