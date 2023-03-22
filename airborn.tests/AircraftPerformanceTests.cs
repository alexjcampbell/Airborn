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
        public void TestJsonRead()
        {
            PerformanceCalculator calculator = GetCalculator(250, 20);

            calculator.Calculate(_testJsonPath);


            // test that the Json de-serializer has read at least one profile
            Assert.IsTrue(calculator.Results[0].JsonFile.TakeoffProfiles.Count > 0);

            // test that the Json de-serializer has read the performance data 
            // for at least one profile
            Assert.IsTrue(calculator.Results[0].JsonFile.TakeoffProfiles[0].GroundRoll.Count > 0);

            // test that the pressureAltitude and ground roll distance match our
            // sample file
            Assert.AreEqual(0, calculator.Results[0].JsonFile.TakeoffProfiles[0].PressureAltitude = 0);
            Assert.AreEqual(917, calculator.Results[0].JsonFile.TakeoffProfiles[0].GroundRoll[0].Distance);
            Assert.AreEqual(1000, calculator.Results[0].JsonFile.TakeoffProfiles[1].PressureAltitude = 1000);
            Assert.AreEqual(1011, calculator.Results[0].JsonFile.TakeoffProfiles[1].GroundRoll[0].Distance);
        }

        [TestMethod]
        public void Test_CalculateInterpolationFactor()
        {

            Assert.AreEqual(0.5, PerformanceCalculator.CalculateInterpolationFactor(15, 10, 20));
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