using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class ScenarioTests
    {

        private int _defaultMagneticVariation = -20;

        private PerformanceCalculator GetCalculator(int windDirectionMagnetic, int windStrength)
        {

            PerformanceCalculator calculator = new PerformanceCalculator(
                AircraftType.SR22_G2,
                new Airport(),
                Wind.FromMagnetic(windDirectionMagnetic, _defaultMagneticVariation, windStrength),
                ""
                );

            calculator.TemperatureCelcius = 12;
            calculator.QNH = 1013;
            calculator.Airport.FieldElevation = 1500;

            return calculator;

        }

        [TestMethod]
        public void TestTrueVsMagnetic()
        {

            Airport airport = new Airport();
            Runway runway = Runway.FromMagnetic(340, _defaultMagneticVariation);
            Wind wind = Wind.FromMagnetic(200, _defaultMagneticVariation, 10);

            PerformanceCalculator scenario = GetCalculator(200, 10);

            // we said 340 magnetic for the runway, so with -20 mag var
            // the true direction for runway heading should be 0
            Assert.AreEqual(0, runway.RunwayHeading.DirectionTrue);

            // we said 200 magnetic for the wind, so with -20 mag var
            // the true direction for wind should be 220
            Assert.AreEqual(220, scenario.Wind.Direction.DirectionTrue);

        }

        [TestMethod]
        public void TestAngularDifference_WindLeftOfRunway()
        {
            Wind wind = Wind.FromMagnetic(310, _defaultMagneticVariation, 10);
            Runway runway = Runway.FromMagnetic(010, _defaultMagneticVariation);

            Assert.AreEqual(-60, CalculationUtilities.AngularDifference(10, 310));

        }

        [TestMethod]
        public void TestAngularDifference_WindRightOfRunway()
        {
            Assert.AreEqual(20, CalculationUtilities.AngularDifference(350, 10));
        }


        [TestMethod]
        public void TestAngularDifference_WindStraightDownRunway()
        {

            Assert.AreEqual(0, CalculationUtilities.AngularDifference(10, 10));

        }

        [TestMethod]
        public void TestAngularDifference_WindStraightDownOppositeRunway()
        {
    
            Assert.AreEqual(-180, CalculationUtilities.AngularDifference(10, 190));
        }

        [TestMethod]
        public void TestAngularDifference_WindGreaterThan180()
        {
            Assert.AreEqual(-150, CalculationUtilities.AngularDifference(160, 10));
        }

        [TestMethod]
        public void Test_CrosswindComponent_NoCrosswind()
        {

            Wind wind = Wind.FromMagnetic(160, _defaultMagneticVariation, 10);
            Runway runway = Runway.FromMagnetic(160, _defaultMagneticVariation);

            PerformanceCalculationResultForRunway result = new PerformanceCalculationResultForRunway(
                runway, wind
            );

            Assert.AreEqual(0, result.CrosswindComponent);
            Assert.AreEqual(10, result.HeadwindComponent);
        }

        [TestMethod]
        public void Test_CrosswindComponent_DirectCrosswind()
        {

            Wind wind = Wind.FromMagnetic(90, _defaultMagneticVariation, 10);
            Runway runway = Runway.FromMagnetic(180, _defaultMagneticVariation);

            PerformanceCalculationResultForRunway result = new PerformanceCalculationResultForRunway(
                runway, wind
            );            

            Assert.AreEqual(-10, result.CrosswindComponent);
            Assert.AreEqual(0, result.HeadwindComponent);
        }

        [TestMethod]
        public void Test_CrosswindComponent_40DegreeCrosswind()
        {

            Wind wind = Wind.FromMagnetic(0, _defaultMagneticVariation, 10);
            Runway runway = Runway.FromMagnetic(40, _defaultMagneticVariation);


            PerformanceCalculationResultForRunway result = new PerformanceCalculationResultForRunway(
                runway, wind
            );

            Assert.AreEqual(-6, result.CrosswindComponent);
            Assert.AreEqual(8, result.HeadwindComponent);
        }

        [TestMethod]
        public void Test_PressureAltitude()
        {
            PerformanceCalculator calculator = GetCalculator(0, 10);

            calculator.TemperatureCelcius = 11;
            calculator.Airport.FieldElevation = 1500;
            calculator.QNH = 1013;

            Assert.AreEqual(1500, calculator.PressureAltitude);
        }
    }
}
