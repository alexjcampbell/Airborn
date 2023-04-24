using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class PerformanceCalculatorTests
    {

        private PerformanceCalculator GetCalculator(int windDirectionMagnetic, int windStrength)
        {

            PerformanceCalculator calculator = new PerformanceCalculator(
                Aircraft.GetAircraftFromAircraftType(AircraftType.SR22_G2),
                new Airport(),
                Wind.FromMagnetic(windDirectionMagnetic, windStrength),
                2000,
                ""
                );

            calculator.TemperatureCelcius = 12;
            calculator.QNH = 1013;
            calculator.Airport.FieldElevation = 1500;

            return calculator;

        }

        [TestMethod]
        public void Test_CrosswindComponent_NoCrosswind()
        {

            int windDirection = 160;
            int windStrength = 10;
            int runwayHeading = 160;

            Wind wind = Wind.FromMagnetic(windDirection, windStrength);
            Runway runway = Runway.FromMagnetic(runwayHeading);

            PerformanceCalculationResultForRunway result = new PerformanceCalculationResultForRunway(
                runway, wind
            );

            Assert.AreEqual(0, result.CrosswindComponent);
            Assert.AreEqual(10, result.HeadwindComponent);
        }

        [TestMethod]
        public void Test_CrosswindComponent_DirectCrosswind()
        {
            Wind wind = Wind.FromMagnetic(90, 10);
            Runway runway = Runway.FromMagnetic(180);

            PerformanceCalculationResultForRunway result = new PerformanceCalculationResultForRunway(
                runway, wind
            );

            Assert.AreEqual(-10, result.CrosswindComponent);
            Assert.AreEqual(0, (int)result.HeadwindComponent);
        }

        [TestMethod]
        public void Test_CrosswindComponent_40DegreeCrosswind()
        {

            Wind wind = Wind.FromMagnetic(0, 10);
            Runway runway = Runway.FromMagnetic(40);


            PerformanceCalculationResultForRunway result = new PerformanceCalculationResultForRunway(
                runway, wind
            );

            Assert.AreEqual(-6.42787609686539m, result.CrosswindComponent);
            Assert.AreEqual(7.66044443118978m, result.HeadwindComponent);
        }

        [TestMethod]
        public void Test_PressureAltitude()
        {
            PerformanceCalculator calculator = GetCalculator(0, 10);

            calculator.TemperatureCelcius = 11;
            calculator.Airport.FieldElevation = 1500;
            calculator.QNH = 1013;

            decimal expectedPressureAltitude = 1506.825m;

            Assert.AreEqual(expectedPressureAltitude, calculator.PressureAltitude);
        }
    }
}
