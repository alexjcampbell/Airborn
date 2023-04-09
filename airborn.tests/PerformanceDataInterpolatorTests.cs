using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;
using System.Collections.Generic;
using Moq;
using System;

namespace Airborn.Tests
{
    [TestClass]
    public class PerformanceDataInterpolatorTests
    {
        private static BookPerformanceDataList SetupTestPerformanceData()
        {
            BookPerformanceDataList performanceDataList = new BookPerformanceDataList();
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 0, 10, 100, 120));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 1000, 10, 200, 240));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 2000, 10, 300, 360));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 3000, 10, 400, 480));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 4000, 10, 500, 600));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 5000, 10, 600, 720));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 0, 20, 700, 840));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 1000, 20, 800, 960));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 2000, 20, 900, 1080));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 3000, 20, 1000, 1200));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 4000, 20, 1100, 1320));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 5000, 20, 1200, 1440));
            return performanceDataList;
        }

        private static PeformanceDataInterpolator GetPerformanceInterpolator(decimal actualPressureAltitude, decimal actualTemperature, BookPerformanceDataList performanceDataList)
        {
            return new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                performanceDataList
            );
        }


        [TestMethod]
        public void TestInterpolatePerformanceDataActualPressureAltitudeEqualsLowerBound()
        {
            decimal actualPressureAltitude = 1000;
            decimal actualTemperature = 20;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                performanceDataList
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.GetInterpolatedBookDistance(Scenario.Takeoff);

            // we're at 1000ft pressure altitude, so we should get the 1000ft pressure altitude data
            // which is 800ft ground roll and 960ft to clear 50ft obstacle
            Assert.AreEqual(800, interpolatedPerformanceData.GroundRoll);
            Assert.AreEqual(960, interpolatedPerformanceData.DistanceToClear50Ft);

        }

        [TestMethod]
        public void TestInterpolatePerformanceDataActualPressureAltitudeEqualsUpperBound()
        {
            decimal actualPressureAltitude = 2000;
            decimal actualTemperature = 20;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                performanceDataList
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.GetInterpolatedBookDistance(Scenario.Takeoff);

            // we're at 2000ft pressure altitude, so we should get the 2000ft pressure altitude data
            // which is 900 ground roll and 1080ft to clear 50ft obstacle
            Assert.AreEqual(900, interpolatedPerformanceData.GroundRoll);
            Assert.AreEqual(1080, interpolatedPerformanceData.DistanceToClear50Ft);

        }

        [TestMethod]
        public void TestInterpolateByPressureAltitude()
        {
            decimal actualPressureAltitude = 1500;
            decimal actualTemperature = 20;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                performanceDataList
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByPressureAltitude(
                performanceDataList[1],
                performanceDataList[2]
            );

            // we're at 1500ft pressure altitude, so we should get the 1500ft pressure altitude data
            // which is 250ft ground roll and 300ft to clear 50ft obstacle
            // the data is interpolated between the 1000ft and 2000ft pressure altitude data
            Assert.AreEqual(250, interpolatedPerformanceData.GroundRoll);
            Assert.AreEqual(300, interpolatedPerformanceData.DistanceToClear50Ft);
        }

        [TestMethod]
        public void TestInterpolateByTemperatureWhenInBetweenTemperatureRange()
        {
            decimal actualPressureAltitude = 1500;
            decimal actualTemperature = 25;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                performanceDataList
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperature(
                performanceDataList[1],
                performanceDataList[2]
            );

            // we're at 25 degrees C, so we should get the 25 degrees C data
            // which is 250ft ground roll and 300ft to clear 50ft obstacle
            // the data is interpolated between the 20 degrees C and 30 degrees C data
            Assert.AreEqual(250, interpolatedPerformanceData.GroundRoll);
            Assert.AreEqual(300, interpolatedPerformanceData.DistanceToClear50Ft);
        }

        [TestMethod]
        public void TestInterpolateByTemperatureWhenAtLowerTemperatureRange()
        {
            decimal actualPressureAltitude = 2000;
            decimal actualTemperature = 20;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                performanceDataList
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperature(
                performanceDataList[1],
                performanceDataList[2]
            );

            // we're at 20 degrees C, so we should get the 20 degrees C data
            // which is 200ft ground roll and 240ft to clear 50ft obstacle
            // the data is interpolated between the 20 degrees C and 30 degrees C data
            Assert.AreEqual(200, interpolatedPerformanceData.GroundRoll);
            Assert.AreEqual(240, interpolatedPerformanceData.DistanceToClear50Ft);
        }

        [TestMethod]
        public void TestInterpolateByTemperatureWhenAtUpperTemperatureRange()
        {
            decimal actualPressureAltitude = 2000;
            decimal actualTemperature = 30;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                performanceDataList
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperature(
                performanceDataList[2],
                performanceDataList[3]
            );

            // we're at 30 degrees C, so we should get the 30 degrees C data
            // which is 300ft ground roll and 360ft to clear 50ft obstacle
            // the data is interpolated between the 20 degrees C and 30 degrees C data
            Assert.AreEqual(300, interpolatedPerformanceData.GroundRoll);
            Assert.AreEqual(360, interpolatedPerformanceData.DistanceToClear50Ft);
        }

        [TestMethod]
        public void TestInterpolateByPressureAltitudeOutOfRange()
        {
            decimal actualPressureAltitude = -1000;
            decimal actualTemperature = 20;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                performanceDataList
            );

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByPressureAltitude(
                    performanceDataList[0],
                    performanceDataList[1]
                );
            });
        }

        [TestMethod]
        public void TestInterpolateByTemperatureOutOfRange()
        {
            decimal actualPressureAltitude = 2000;
            decimal actualTemperature = -20;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(actualPressureAltitude, actualTemperature, performanceDataList);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperature(
                    performanceDataList[0],
                    performanceDataList[1]
                );
            });
        }
    }
}