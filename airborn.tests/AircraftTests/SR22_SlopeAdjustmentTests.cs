using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests.AircraftTests
{
    [TestClass]
    public class SR22_SlopeAdjustmentTests
    {
        [TestMethod]
        public void Test_SR22_TakeoffSlopeAdjustment_SeaLevel_Upslope()
        {
            // At sea level, upslope adjustment should be 22% per 1% slope
            var logItem = new PerformanceCalculationLog.LogItem("Test");
            double groundRoll = 1000;
            double slope = 2.0; // 2% upslope
            double altitude = 0; // sea level
            
            var adjusted = SR22.CalculateAdjustedGroundRollDistanceForLanding(groundRoll, slope, altitude, logItem);
            
            // Expected: 1000 + (1000 * 0.22 * 2) = 1000 + 440 = 1440
            Assert.AreEqual(1440, adjusted.TotalFeet, 0.01);
        }

        [TestMethod]
        public void Test_SR22_TakeoffSlopeAdjustment_5000ft_Upslope()
        {
            // At 5000 ft, upslope adjustment should be 30% per 1% slope
            var logItem = new PerformanceCalculationLog.LogItem("Test");
            double groundRoll = 1000;
            double slope = 2.0; // 2% upslope
            double altitude = 5000;
            
            var adjusted = SR22.CalculateAdjustedGroundRollDistanceForLanding(groundRoll, slope, altitude, logItem);
            
            // Expected: 1000 + (1000 * 0.30 * 2) = 1000 + 600 = 1600
            Assert.AreEqual(1600, adjusted.TotalFeet, 0.01);
        }

        [TestMethod]
        public void Test_SR22_TakeoffSlopeAdjustment_10000ft_Upslope()
        {
            // At 10000 ft, upslope adjustment should be 43% per 1% slope
            var logItem = new PerformanceCalculationLog.LogItem("Test");
            double groundRoll = 1000;
            double slope = 2.0; // 2% upslope
            double altitude = 10000;
            
            var adjusted = SR22.CalculateAdjustedGroundRollDistanceForLanding(groundRoll, slope, altitude, logItem);
            
            // Expected: 1000 + (1000 * 0.43 * 2) = 1000 + 860 = 1860
            Assert.AreEqual(1860, adjusted.TotalFeet, 0.01);
        }

        [TestMethod]
        public void Test_SR22_TakeoffSlopeAdjustment_2500ft_Upslope_Interpolated()
        {
            // At 2500 ft (halfway between 0 and 5000), adjustment should be interpolated
            // Expected factor: 0.22 + (0.30 - 0.22) * (2500/5000) = 0.22 + 0.08 * 0.5 = 0.26
            var logItem = new PerformanceCalculationLog.LogItem("Test");
            double groundRoll = 1000;
            double slope = 2.0; // 2% upslope
            double altitude = 2500;
            
            var adjusted = SR22.CalculateAdjustedGroundRollDistanceForLanding(groundRoll, slope, altitude, logItem);
            
            // Expected: 1000 + (1000 * 0.26 * 2) = 1000 + 520 = 1520
            Assert.AreEqual(1520, adjusted.TotalFeet, 0.01);
        }

        [TestMethod]
        public void Test_SR22_TakeoffSlopeAdjustment_7500ft_Upslope_Interpolated()
        {
            // At 7500 ft (halfway between 5000 and 10000), adjustment should be interpolated
            // Expected factor: 0.30 + (0.43 - 0.30) * (2500/5000) = 0.30 + 0.13 * 0.5 = 0.365
            var logItem = new PerformanceCalculationLog.LogItem("Test");
            double groundRoll = 1000;
            double slope = 2.0; // 2% upslope
            double altitude = 7500;
            
            var adjusted = SR22.CalculateAdjustedGroundRollDistanceForLanding(groundRoll, slope, altitude, logItem);
            
            // Expected: 1000 + (1000 * 0.365 * 2) = 1000 + 730 = 1730
            Assert.AreEqual(1730, adjusted.TotalFeet, 0.01);
        }

        [TestMethod]
        public void Test_SR22_TakeoffSlopeAdjustment_Downslope_NoAdjustment()
        {
            // Per current implementation, downslope should not decrease distance
            var logItem = new PerformanceCalculationLog.LogItem("Test");
            double groundRoll = 1000;
            double slope = -2.0; // 2% downslope
            double altitude = 5000;
            
            var adjusted = SR22.CalculateAdjustedGroundRollDistanceForLanding(groundRoll, slope, altitude, logItem);
            
            // Expected: No adjustment for downslope
            Assert.AreEqual(1000, adjusted.TotalFeet, 0.01);
        }

        [TestMethod]
        public void Test_SR22_TakeoffSlopeAdjustment_BelowSeaLevel()
        {
            // Below sea level should use sea level values (22%)
            var logItem = new PerformanceCalculationLog.LogItem("Test");
            double groundRoll = 1000;
            double slope = 1.5; // 1.5% upslope
            double altitude = -500; // 500 ft below sea level
            
            var adjusted = SR22.CalculateAdjustedGroundRollDistanceForLanding(groundRoll, slope, altitude, logItem);
            
            // Expected: 1000 + (1000 * 0.22 * 1.5) = 1000 + 330 = 1330
            Assert.AreEqual(1330, adjusted.TotalFeet, 0.01);
        }

        [TestMethod]
        public void Test_SR22_TakeoffSlopeAdjustment_Above10000ft()
        {
            // Above 10000 ft should use 10000 ft values (43%)
            var logItem = new PerformanceCalculationLog.LogItem("Test");
            double groundRoll = 1000;
            double slope = 1.0; // 1% upslope
            double altitude = 12000; // Above 10000 ft
            
            var adjusted = SR22.CalculateAdjustedGroundRollDistanceForLanding(groundRoll, slope, altitude, logItem);
            
            // Expected: 1000 + (1000 * 0.43 * 1) = 1000 + 430 = 1430
            Assert.AreEqual(1430, adjusted.TotalFeet, 0.01);
        }

        [TestMethod]
        public void Test_SR22_TakeoffSlopeAdjustment_ZeroSlope()
        {
            // Zero slope should result in no adjustment
            var logItem = new PerformanceCalculationLog.LogItem("Test");
            double groundRoll = 1000;
            double slope = 0.0; // No slope
            double altitude = 5000;
            
            var adjusted = SR22.CalculateAdjustedGroundRollDistanceForLanding(groundRoll, slope, altitude, logItem);
            
            // Expected: No adjustment
            Assert.AreEqual(1000, adjusted.TotalFeet, 0.01);
        }
    }
}