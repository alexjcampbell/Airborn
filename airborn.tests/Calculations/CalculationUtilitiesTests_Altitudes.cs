using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class CalculationUtilitiesTests_Altitudes
    {

        [TestMethod]
        public void Test_GetPressureAltitudeAtAirport_0ft_Equals0ft()
        {
            int elevation = 0;
            double qnh = 1013.25f;
            int expectedPressureAltitude = 0;

            Assert.AreEqual(
                expectedPressureAltitude,
                CalculationUtilities.PressureAltitudeAtFieldElevation(qnh, elevation),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }


        [TestMethod]
        public void Test_CalculatePressureAltitude_3092Hg_Equals1047Hpa()
        {
            double qnh = 1047.11f;
            double altimeter = 30.92f;

            Assert.AreEqual(
                CalculationUtilities.PressureAltitudeAtFieldElevation(qnh, 0),
                CalculationUtilities.PressureAltitudeAtFieldElevation(
                    CalculationUtilities.InchesOfMercuryToMillibars(altimeter), 0
                    ),
                1f
                );
        }

        [TestMethod]
        public void Test_CalculatePressureAltitude_2992Hg_Equals1013Hpa()
        {
            double qnh = 1013.25f;
            double altimeter = 29.92f;

            Assert.AreEqual(
                CalculationUtilities.PressureAltitudeAtFieldElevation(qnh, 0),
                CalculationUtilities.PressureAltitudeAtFieldElevation(
                    CalculationUtilities.InchesOfMercuryToMillibars(altimeter), 0
                    ),
                1f
                );
        }

        [TestMethod]
        public void Test_DensityAltitudeAtSeaLevelISA_Equals_PressureAltitude()
        {
            double temperature = 15f;
            double expectedDensityAltitude = 0;

            Assert.AreEqual(
                expectedDensityAltitude,
                CalculationUtilities.DensityAltitudeAtAirport(temperature, 15, 0),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_DensityAltitudeAt1000ftISA_Equals_PressureAltitude()
        {
            double temperature = 13f;
            double expectedDensityAltitude = 1000;

            Assert.AreEqual(
                expectedDensityAltitude,
                CalculationUtilities.DensityAltitudeAtAirport(temperature, 13, 1000),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_DensityAltitudeAtSeaLevelAtISAMinus2Degrees_IsCorrect()
        {
            double temperature = 13;
            double expectedDensityAltitude = -240;
            double qnh = 1013.25f;

            double pressureAltitude = CalculationUtilities.PressureAltitudeAtFieldElevation(qnh, 0);
            double isaTemperature = CalculationUtilities.ISATemperatureForPressureAltitude(pressureAltitude);

            Assert.AreEqual(
                expectedDensityAltitude,
                CalculationUtilities.DensityAltitudeAtAirport(temperature, isaTemperature, pressureAltitude),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

    }

}