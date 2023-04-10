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
        public void PerformanceDataInterpolationUtilitiesTests()
        {

            // 15 is halfway between 10 and 20, so interpolation factor should be 0.5
            Assert.AreEqual(0.5m, PerformanceDataInterpolationUtilities.CalculateInterpolationFactor(15, 10, 20));

            // 10 is at the lower bound between 10 and 20, so interpolation factor should be 0
            Assert.AreEqual(0, PerformanceDataInterpolationUtilities.CalculateInterpolationFactor(10, 10, 20));

            // 20 is at the upper bound between 10 and 20, so interpolation factor should be 1
            Assert.AreEqual(1, PerformanceDataInterpolationUtilities.CalculateInterpolationFactor(20, 10, 20));

        }

        [TestMethod]
        public void Test_GetUpperAndLowerBoundForInterpolation()
        {
            // test it for pressure altitude intervals (1000s)
            Assert.AreEqual((3000, 4000), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(3500, 1000));
            Assert.AreEqual((3000, 4000), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(3000, 1000));
            Assert.AreEqual((3000, 4000), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(3999, 1000));

            Assert.AreEqual((0, 1000), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(500, 1000));
            Assert.AreEqual((0, 1000), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(0, 1000));
            Assert.AreEqual((0, 1000), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(999, 1000));


            // test it for temperature intervals (10s)
            Assert.AreEqual((30, 40), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(35, 10));
            Assert.AreEqual((30, 40), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(30, 10));
            Assert.AreEqual((30, 40), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(39, 10));

            Assert.AreEqual((0, 10), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(0, 10));
            Assert.AreEqual((0, 10), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(5, 10));
            Assert.AreEqual((0, 10), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(9, 10));

            Assert.AreEqual((0, 10), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(0, 10));
            Assert.AreEqual((0, 10), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(5, 10));
            Assert.AreEqual((0, 10), PerformanceDataInterpolationUtilities.GetUpperAndLowBoundsForInterpolation(9, 10));
        }

        [TestMethod]
        public void Test_Interpolate()
        {
            Assert.AreEqual(1175, PerformanceDataInterpolationUtilities.Interpolate(1150, 1200, 15, 10));
            Assert.AreEqual(1100, PerformanceDataInterpolationUtilities.Interpolate(1100, 1200, 10, 10));
            Assert.AreEqual(1420, PerformanceDataInterpolationUtilities.Interpolate(1400, 1500, 1200, 1000));
        }



    }
}