using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace Airborn.Tests
{
    [TestClass]
    public class CalculationTests
    {

        private Airport _airport = new Airport { Ident = UtilitiesForTesting.Default_AirportIdentifier };

        private Calculation GetCalculator(int windDirectionMagnetic, int windStrength)
        {
            return GetCalculator(windDirectionMagnetic, windStrength, UtilitiesForTesting.GetMockJsonPerformanceFilesForTesting()[0]);
        }

        private Calculation GetCalculator(int windDirectionMagnetic, int windStrength, JsonFile jsonFile)
        {
            return GetCalculator(windDirectionMagnetic, windStrength, jsonFile, UtilitiesForTesting.Default_AircraftType);
        }

        private Calculation GetCalculator(int windDirectionMagnetic, int windStrength, JsonFile jsonFile, AircraftType aircraftType)
        {

            Calculation calculator = new Calculation(
                Aircraft.GetAircraftFromAircraftType(aircraftType),
                _airport,
                Wind.FromMagnetic(windDirectionMagnetic, windStrength),
                UtilitiesForTesting.Default_AircraftWeight,
                jsonFile
                );

            calculator.TemperatureCelcius = 12;
            calculator.AltimeterSettingInMb = 1013;
            calculator.Airport.FieldElevation = 1500;

            return calculator;

        }

        [TestMethod]
        public void Test_PerformanceCalculator_PressureAltitudeAt1500ElevationISAPressure_Is1500()
        {
            Calculation calculator = GetCalculator(0, 10);

            calculator.TemperatureCelcius = 11;
            calculator.Airport.FieldElevation = 1500;
            calculator.AltimeterSettingInMb = 1013;

            double expectedPressureAltitude = 1506.82f;

            Assert.AreEqual(
                expectedPressureAltitude,
                calculator.PressureAltitude.TotalFeet,
                0.01f);
        }

        [TestMethod]
        public void Test_PerformanceCalculator_TemperatureCelciusAlwaysPositiveOrZero_ShouldReturnNonNegativeValue()
        {
            Calculation calculator = GetCalculator(0, 10);
            calculator.TemperatureCelcius = -5;

            Assert.IsTrue(calculator.TemperatureCelciusAdjustedForBounds >= 0);
        }

        [TestMethod]
        public void Test_PerformanceCalculator_PressureAltitudeAlwaysPositiveOrZero_ShouldReturnNonNegativeValue()
        {
            Calculation calculator = GetCalculator(0, 10);
            calculator.AltimeterSettingInMb = 1040;
            calculator.Airport.FieldElevation = 0;

            Assert.IsTrue(calculator.PressureAltitudeAdjustedForBounds.TotalFeet >= 0);
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

        [TestMethod]
        public void Test_PerformanceCalculator_WhenTemperatureIsGreaterThanMaximum_ShouldPopulateNotes()
        {
            var airportDbContext = UtilitiesForTesting.GetMockAirportDbContextForTesting();
            JsonFile jsonFile = UtilitiesForTesting.GetMockJsonPerformanceFilesForTesting()[0];

            Airport _airport = new Airport { Ident = "KJFK" };

            var calculator = GetCalculator(0, 10, jsonFile, AircraftType.C172_SP);

            calculator.AircraftWeight = 2500;
            calculator.TemperatureCelcius = 50;
            calculator.AltimeterSettingInMb = 1040;
            calculator.Airport.FieldElevation = 1000;

            calculator.Calculate(airportDbContext);

            Assert.IsNotNull(calculator.Notes);
            Assert.AreEqual(1, calculator.Notes.Count);

            StringAssert.Contains(calculator.Notes[0], "but POH data only provides performance information for temperatures up to");

        }

        [TestMethod]
        public void Test_PerformanceCalculator_WhenPressureAltitudeIsGreaterThanMaximum_ShouldPopulateNotes()
        {
            var airportDbContext = UtilitiesForTesting.GetMockAirportDbContextForTesting();
            JsonFile jsonFile = UtilitiesForTesting.GetMockJsonPerformanceFilesForTesting()[0];

            Airport _airport = new Airport { Ident = "KJFK" };

            var calculator = GetCalculator(0, 10, jsonFile, AircraftType.C172_SP);

            calculator.AircraftWeight = 2500;
            calculator.TemperatureCelcius = 30;
            calculator.AltimeterSettingInMb = 1040;
            calculator.Airport.FieldElevation = 11000; // 1000 feet above maximum

            calculator.Calculate(airportDbContext);

            Assert.IsNotNull(calculator.Notes);
            Assert.AreEqual(1, calculator.Notes.Count);

            StringAssert.Contains(calculator.Notes[0], "but POH data only provides performance information for pressure altitudes up to");

        }


    }

}
