using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;
using System.Collections.Generic;

namespace Airborn.Tests
{
    [TestClass]
    public class AircraftPerformanceTests
    {

        private const string _testJsonPath = "../../Debug/net6.0/SR22_G2_3400.json";

        private int _defaultMagneticVariation = -20;

        private PerformanceCalculator GetCalculator(int windDirectionMagnetic, int windStrength)
        {

            PerformanceCalculator calculator = new PerformanceCalculator(
                AircraftType.SR22_G2,
                new Airport(),
                Wind.FromMagnetic(windDirectionMagnetic, _defaultMagneticVariation, windStrength),
                _testJsonPath
                );

            calculator.TemperatureCelcius = 12;
            calculator.QNH = 1013;
            calculator.Airport.FieldElevation = 1500;
            calculator.Airport.Ident = "KCRQ";

            return calculator;

        }


        [TestMethod]
        public void Test_CalculateInterpolationFactor()
        {

            Assert.AreEqual(0.5m, PerformanceCalculator.CalculateInterpolationFactor(15, 10, 20));
            Assert.AreEqual(0, PerformanceCalculator.CalculateInterpolationFactor(10, 10, 20));
            Assert.AreEqual(1, PerformanceCalculator.CalculateInterpolationFactor(20, 10, 20));

        }

        [TestMethod]
        public void Test_GetUpperAndLowerBoundForInterpolation()
        {
            // test it for pressure altitude intervals (1000s)
            Assert.AreEqual((3000, 4000), PerformanceCalculator.GetUpperAndLowBoundsForInterpolation(3500, 1000));
            Assert.AreEqual((3000, 4000), PerformanceCalculator.GetUpperAndLowBoundsForInterpolation(3000, 1000));
            Assert.AreEqual((3000, 4000), PerformanceCalculator.GetUpperAndLowBoundsForInterpolation(3999, 1000));

            Assert.AreEqual((0, 1000), PerformanceCalculator.GetUpperAndLowBoundsForInterpolation(500, 1000));
            Assert.AreEqual((0, 1000), PerformanceCalculator.GetUpperAndLowBoundsForInterpolation(0, 1000));
            Assert.AreEqual((0, 1000), PerformanceCalculator.GetUpperAndLowBoundsForInterpolation(999, 1000));


            // test it for temperature intervals (10s)
            Assert.AreEqual((30, 40), PerformanceCalculator.GetUpperAndLowBoundsForInterpolation(35, 10));
            Assert.AreEqual((30, 40), PerformanceCalculator.GetUpperAndLowBoundsForInterpolation(30, 10));
            Assert.AreEqual((30, 40), PerformanceCalculator.GetUpperAndLowBoundsForInterpolation(39, 10));

            Assert.AreEqual((0, 10), PerformanceCalculator.GetUpperAndLowBoundsForInterpolation(0, 10));
            Assert.AreEqual((0, 10), PerformanceCalculator.GetUpperAndLowBoundsForInterpolation(5, 10));
            Assert.AreEqual((0, 10), PerformanceCalculator.GetUpperAndLowBoundsForInterpolation(9, 10));

            Assert.AreEqual((0, 10), PerformanceCalculator.GetUpperAndLowBoundsForInterpolation(0, 10));
            Assert.AreEqual((0, 10), PerformanceCalculator.GetUpperAndLowBoundsForInterpolation(5, 10));
            Assert.AreEqual((0, 10), PerformanceCalculator.GetUpperAndLowBoundsForInterpolation(9, 10));
        }

        [TestMethod]
        public void Test_Interpolate()
        {
            Assert.AreEqual(1175, PerformanceCalculator.Interpolate(1150, 1200, 15, 10));
            Assert.AreEqual(1100, PerformanceCalculator.Interpolate(1100, 1200, 10, 10));
            Assert.AreEqual(1420, PerformanceCalculator.Interpolate(1400, 1500, 1200, 1000));
        }



    }
}