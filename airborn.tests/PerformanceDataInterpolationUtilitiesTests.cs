using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;
using System.Collections.Generic;
using Moq;

namespace Airborn.Tests
{
    [TestClass]
    public class AircraftPerformanceTests
    {


        [TestMethod]
        public void Test_CalculateInterpolationFactor_AtZero_ReturnsZero()
        {
            Assert.AreEqual(0, PerformanceDataInterpolationUtilities.CalculateInterpolationFactor(0, 0, 10));
        }

        [TestMethod]
        public void Test_CalculateInterpolationFactor_AtLowerBound_ReturnsZero()
        {

            // 10 is at the lower bound between 10 and 20, so interpolation factor should be 0
            Assert.AreEqual(0, PerformanceDataInterpolationUtilities.CalculateInterpolationFactor(10, 10, 20));
        }

        [TestMethod]
        public void Test_CalculateInterpolationFactor_AtMidPoint_ReturnsFiftyPercent()
        {

            // 15 is halfway between 10 and 20, so interpolation factor should be 0.5
            Assert.AreEqual(0.5m, PerformanceDataInterpolationUtilities.CalculateInterpolationFactor(15, 10, 20));

        }

        [TestMethod]
        public void Test_CalculateInterpolationFactor_AtUpperBound_ReturnsOne()
        {

            // 20 is at the upper bound between 10 and 20, so interpolation factor should be 1
            Assert.AreEqual(1, PerformanceDataInterpolationUtilities.CalculateInterpolationFactor(20, 10, 20));

        }

        [TestMethod]
        public void Test_GetUpperAndLowerBoundForInterpolation_AtLowerRange_ReturnsLowerRangePlusInterval()
        {
            Assert.AreEqual((3000, 4000), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(3000, 1000));
        }

        [TestMethod]
        public void Test_GetUpperAndLowerBoundForInterpolation_AtMidRange_ReturnsLowerRangePlusInterval()
        {
            Assert.AreEqual((3000, 4000), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(3500, 1000));
        }

        [TestMethod]
        public void Test_GetUpperAndLowerBoundForInterpolation_AlmostAtUpperRange_ReturnsLowerRangePlusInterval()
        {
            Assert.AreEqual((3000, 4000), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(3999, 1000));
        }

        [TestMethod]
        public void Test_GetLowerBoundForInterpolation_AtLowerBound_ReturnsLowerBound()
        {
            Assert.AreEqual(10, PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(10, 10));
        }

        [TestMethod]
        public void Test_GetLowerBoundForInterpolation_AtMidPoint_ReturnsLowerBound()
        {
            Assert.AreEqual(10, PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(15, 10));
        }

        [TestMethod]
        public void Test_GetLowerBoundForInterpolation_AtNextLowerbound_ReturnsNextLowerBound()
        {
            Assert.AreEqual(20, PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(20, 10));
        }

        [TestMethod]
        public void Test_GetLowerBoundForInterpolation_AtNextMidPoint_ReturnsNextMidPoint()
        {
            Assert.AreEqual(20, PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(25, 10));
        }

        [TestMethod]
        public void Test_GetLowerBoundForInterpolation_AtPressureAltitudeLowerBound_ReturnsLowerBound()
        {
            Assert.AreEqual(1000, PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(1000, 1000));
        }

        [TestMethod]
        public void Test_GetLowerBoundForInterpolation_AtPressureAltitudeMidPoint_ReturnsMidPoint()
        {
            Assert.AreEqual(1000, PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(1500, 1000));
        }

        [TestMethod]
        public void Test_GetLowerBoundForInterpolation_AtPressureAltitudeNextLowerBound_ReturnsNextLowerBound()
        {
            Assert.AreEqual(2000, PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(2000, 1000));
        }

        [TestMethod]
        public void Test_GetUpperBoundForInterpolation_AtLowerBound_ReturnsLowerBoundPlusInterval()
        {
            Assert.AreEqual(20, PerformanceDataInterpolationUtilities.GetUpperBoundForInterpolation(10, 10));
        }
    }
}