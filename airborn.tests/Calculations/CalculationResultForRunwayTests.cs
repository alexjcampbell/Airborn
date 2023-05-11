using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace Airborn.Tests
{
    [TestClass]
    public class CalculationResultForRunwayTests
    {

        private Airport _airport = new Airport { Ident = UtilitiesForTesting.Default_AirportIdentifier };

        private Calculation GetCalculator(int windDirectionMagnetic, int windStrength)
        {
            return GetCalculator(windDirectionMagnetic, windStrength, UtilitiesForTesting.GetMockJsonPerformanceFilesForTesting()[0]);
        }

        private Calculation GetCalculator(int windDirectionMagnetic, int windStrength, JsonFile jsonFile)
        {

            Calculation calculator = new Calculation(
                Aircraft.GetAircraftFromAircraftType(UtilitiesForTesting.Default_AircraftType),
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
        public void Test_PerformanceCalculator_CrosswindComponent_NoCrosswind_IsZero()
        {

            int windDirection = 160;
            int windStrength = 10;
            int runwayHeading = 160;

            Wind wind = Wind.FromMagnetic(windDirection, windStrength);
            Runway runway = Runway.FromMagnetic(new Airport(), runwayHeading, "1C");

            CalculationResultForRunway result = new CalculationResultForRunway(
                runway, wind, new PerformanceCalculationLog.LogItem(""), new Distance()
            );

            double expectedCrosswindComponent = 0;

            Assert.AreEqual(
                expectedCrosswindComponent,
                result.CrosswindComponent,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

            double expectedHeadwindComponent = 10;

            Assert.AreEqual(
                expectedHeadwindComponent,
                result.HeadwindComponent,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_PerformanceCalculator_CrosswindComponent_DirectCrosswind_EqualsTotalWind()
        {
            Wind wind = Wind.FromMagnetic(90, 10);
            Runway runway = Runway.FromMagnetic(new Airport(), 180, "1C");

            CalculationResultForRunway result = new CalculationResultForRunway(
                runway, wind, new PerformanceCalculationLog.LogItem(""), new Distance()
            );

            double expectedCrosswindComponent = -10;

            Assert.AreEqual(
                expectedCrosswindComponent,
                result.CrosswindComponent,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );


            double expectedHeadwindComponent = 0;

            Assert.AreEqual(
                expectedHeadwindComponent,
                result.HeadwindComponent,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_PerformanceCalculator_CrosswindComponent_40DegreeCrosswind_IsCorrect()
        {

            Wind wind = Wind.FromMagnetic(0, 10);
            Runway runway = Runway.FromMagnetic(new Airport(), 40, "1C");


            CalculationResultForRunway result = new CalculationResultForRunway(
                runway, wind, new PerformanceCalculationLog.LogItem(""), new Distance()
            );

            double expectedCrosswindComponent = -6.42f;

            Assert.AreEqual(
                expectedCrosswindComponent,
                result.CrosswindComponent,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

            double expectedHeadwindComponent = 7.66f;

            Assert.AreEqual(
                expectedHeadwindComponent,
                result.HeadwindComponent,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

    }

}
