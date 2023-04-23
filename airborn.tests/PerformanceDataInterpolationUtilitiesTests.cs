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
            Assert.AreEqual(3000, PerformanceDataInterpolationUtilities.Interpolate(3000, 4000, 60, 10));
            Assert.AreEqual(3500, PerformanceDataInterpolationUtilities.Interpolate(3000, 4000, 65, 10));
            Assert.AreEqual(3000, PerformanceDataInterpolationUtilities.Interpolate(3000, 4000, 70, 10));
        }

        [TestMethod]
        public void Test_GetLowerBoundForInterpolation()
        {
            Assert.AreEqual(10, PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(10, 10));
            Assert.AreEqual(10, PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(15, 10));
            Assert.AreEqual(20, PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(20, 10));
            Assert.AreEqual(20, PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(25, 10));

            Assert.AreEqual(1000, PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(1000, 1000));
            Assert.AreEqual(1000, PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(1500, 1000));
            Assert.AreEqual(2000, PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(2000, 1000));
            Assert.AreEqual(2000, PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(2500, 1000));

        }
    }
}