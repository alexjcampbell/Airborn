using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace Airborn.Tests
{
    [TestClass]
    public class PerformanceCalculatorTests
    {

        private Airport _airport = new Airport { Ident = UtilitiesForTesting.Default_AirportIdentifier };

        private PerformanceCalculator GetCalculator(int windDirectionMagnetic, int windStrength)
        {
            return GetCalculator(windDirectionMagnetic, windStrength, UtilitiesForTesting.GetMockJsonPerformanceFilesForTesting()[0]);
        }

        private PerformanceCalculator GetCalculator(int windDirectionMagnetic, int windStrength, JsonFile jsonFile)
        {

            PerformanceCalculator calculator = new PerformanceCalculator(
                Aircraft.GetAircraftFromAircraftType(UtilitiesForTesting.Default_AircraftType),
                _airport,
                Wind.FromMagnetic(windDirectionMagnetic, windStrength),
                (decimal)UtilitiesForTesting.Default_AircraftWeight,
                jsonFile
                );

            calculator.TemperatureCelcius = 12;
            calculator.AltimeterSettingInMb = 1013;
            calculator.Airport.FieldElevation = 1500;

            return calculator;

        }

        [TestMethod]
        public void Test_PerformanceCalculator_CrosswindComponent_NoCrosswind_IsZero()
        {

            int windDirection = 160;
            int windStrength = 10;
            int runwayHeading = 160;

            Wind wind = Wind.FromMagnetic(windDirection, windStrength);
            Runway runway = Runway.FromMagnetic(runwayHeading);

            PerformanceCalculationResultForRunway result = new PerformanceCalculationResultForRunway(
                runway, wind, new PerformanceCalculationLogItem(""), new Distance()
            );

            Assert.AreEqual(0, result.CrosswindComponent);
            Assert.AreEqual(10, result.HeadwindComponent);
        }

        [TestMethod]
        public void Test_PerformanceCalculator_CrosswindComponent_DirectCrosswind_EqualsTotalWind()
        {
            Wind wind = Wind.FromMagnetic(90, 10);
            Runway runway = Runway.FromMagnetic(180);

            PerformanceCalculationResultForRunway result = new PerformanceCalculationResultForRunway(
                runway, wind, new PerformanceCalculationLogItem(""), new Distance()
            );

            Assert.AreEqual(-10, result.CrosswindComponent);
            Assert.AreEqual(0, (int)result.HeadwindComponent);
        }

        [TestMethod]
        public void Test_PerformanceCalculator_CrosswindComponent_40DegreeCrosswind_IsCorrect()
        {

            Wind wind = Wind.FromMagnetic(0, 10);
            Runway runway = Runway.FromMagnetic(40);


            PerformanceCalculationResultForRunway result = new PerformanceCalculationResultForRunway(
                runway, wind, new PerformanceCalculationLogItem(""), new Distance()
            );

            Assert.AreEqual(-6.42787609686539m, result.CrosswindComponent);
            Assert.AreEqual(7.66044443118978m, result.HeadwindComponent);
        }

        [TestMethod]
        public void Test_PerformanceCalculator_PressureAltitudeAt1500ElevationISAPressure_Is1500()
        {
            PerformanceCalculator calculator = GetCalculator(0, 10);

            calculator.TemperatureCelcius = 11;
            calculator.Airport.FieldElevation = 1500;
            calculator.AltimeterSettingInMb = 1013;

            double expectedPressureAltitude = 1506.825f;

            Assert.AreEqual(
                expectedPressureAltitude,
                (double)calculator.PressureAltitude.TotalFeet,
                0.01f);
        }

        [TestMethod]
        public void Test_PerformanceCalculator_TemperatureCelciusAlwaysPositiveOrZero_ShouldReturnNonNegativeValue()
        {
            PerformanceCalculator calculator = GetCalculator(0, 10);
            calculator.TemperatureCelcius = -5;
            Assert.IsTrue(calculator.TemperatureCelciusAlwaysPositiveOrZero >= 0);
        }

        [TestMethod]
        public void Test_PerformanceCalculator_PressureAltitudeAlwaysPositiveOrZero_ShouldReturnNonNegativeValue()
        {
            PerformanceCalculator calculator = GetCalculator(0, 10);
            calculator.AltimeterSettingInMb = 1040;
            calculator.Airport.FieldElevation = 0;
            Assert.IsTrue(calculator.PressureAltitudeAlwaysPositiveOrZero.TotalFeet >= 0);
        }

        [TestMethod]
        public void Test_PerformanceCalculator_PopulatesResults()
        {
            var airportDbContext = UtilitiesForTesting.GetMockAirportDbContextForTesting();
            JsonFile jsonFile = UtilitiesForTesting.GetMockJsonPerformanceFilesForTesting()[0];

            var calculator = GetCalculator(0, 10, jsonFile);

            calculator.AltimeterSettingInMb = 1013;

            calculator.Calculate(airportDbContext);

            Assert.IsNotNull(calculator.Runways);
            Assert.AreEqual(10, calculator.Runways.Count);
            Assert.AreEqual(10, calculator.Results.Count);
        }

        [TestMethod]
        public void Test_PerformanceCalculator_WhenTemperatureOrPressureAltitudeIsNegative_ShouldPopulateNotes()
        {
            var airportDbContext = UtilitiesForTesting.GetMockAirportDbContextForTesting();
            JsonFile jsonFile = UtilitiesForTesting.GetMockJsonPerformanceFilesForTesting()[0];

            Airport _airport = new Airport { Ident = "KJFK" };

            var calculator = GetCalculator(0, 10);

            calculator.TemperatureCelcius = -10;
            calculator.AltimeterSettingInMb = 1040;
            calculator.Airport.FieldElevation = -100;

            calculator.Calculate(airportDbContext);

            Assert.IsNotNull(calculator.Notes);
            Assert.AreEqual(2, calculator.Notes.Count);

            StringAssert.Contains(calculator.Notes[0], "Pressure altitude is negative");
            StringAssert.Contains(calculator.Notes[1], "Temperature is negative");

        }





    }

}
