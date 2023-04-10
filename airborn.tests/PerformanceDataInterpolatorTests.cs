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
            decimal lowerWeight = Aircraft.GetAircraftFromAircraftType(AircraftType.SR22_G2).GetLowerWeight();
            decimal higherWeight = Aircraft.GetAircraftFromAircraftType(AircraftType.SR22_G2).GetHigherWeight();

            BookPerformanceDataList performanceDataList = new BookPerformanceDataList((int)lowerWeight, (int)higherWeight);

            // lower weight

            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 0, 10, lowerWeight, 100, 120));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 1000, 10, lowerWeight, 200, 240));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 2000, 10, lowerWeight, 300, 360));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 3000, 10, lowerWeight, 400, 480));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 4000, 10, lowerWeight, 500, 600));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 5000, 10, lowerWeight, 600, 720));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 0, 20, lowerWeight, 700, 840));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 1000, 20, lowerWeight, 800, 960));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 2000, 20, lowerWeight, 900, 1080));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 3000, 20, lowerWeight, 1000, 1200));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 4000, 20, lowerWeight, 1100, 1320));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 5000, 20, lowerWeight, 1200, 1440));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 0, 30, lowerWeight, 1300, 2840));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 1000, 30, lowerWeight, 1400, 2960));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 2000, 30, lowerWeight, 1500, 2080));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 3000, 30, lowerWeight, 1600, 2200));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 4000, 30, lowerWeight, 1700, 2320));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 5000, 30, lowerWeight, 1800, 2440));


            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 0, 10, higherWeight, 150, 170));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 1000, 10, higherWeight, 250, 270));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 2000, 10, higherWeight, 350, 370));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 3000, 10, higherWeight, 450, 470));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 4000, 10, higherWeight, 550, 670));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 5000, 10, higherWeight, 650, 770));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 0, 20, higherWeight, 750, 870));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 1000, 20, higherWeight, 850, 970));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 2000, 20, higherWeight, 950, 1130));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 3000, 20, higherWeight, 1050, 1270));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 4000, 20, higherWeight, 1150, 1370));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 5000, 20, higherWeight, 1250, 1470));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 0, 30, higherWeight, 1750, 870));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 1000, 30, higherWeight, 1850, 3970));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 2000, 30, higherWeight, 1950, 3070));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 3000, 30, higherWeight, 2050, 3270));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 4000, 30, higherWeight, 2150, 3370));
            performanceDataList.Add(new BookPerformanceData(Scenario.Takeoff, 5000, 30, higherWeight, 2250, 3470));

            return performanceDataList;
        }

        private static PeformanceDataInterpolator GetPerformanceInterpolator(
            decimal actualPressureAltitude,
            decimal actualTemperature,
            decimal actualWeight,
            BookPerformanceDataList performanceDataList)
        {
            return new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight,
                performanceDataList
            );
        }


        [TestMethod]
        public void TestInterpolatePerformanceDataActualPressureAltitudeEqualsLowerBound()
        {
            decimal actualPressureAltitude = 1000;
            decimal actualTemperature = 20;
            decimal actualWeight = 3000;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight,
                performanceDataList
            );

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(AircraftType.SR22_G2);

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.GetInterpolatedBookDistance(aircraft, Scenario.Takeoff);

            Assert.AreEqual(810, interpolatedPerformanceData.GroundRoll);
            Assert.AreEqual(962, interpolatedPerformanceData.DistanceToClear50Ft);

        }

        [TestMethod]
        public void TestInterpolatePerformanceDataActualPressureAltitudeEqualsUpperBound()
        {
            decimal actualPressureAltitude = 2000;
            decimal actualTemperature = 20;
            decimal actualAircraftWeight = 3000;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList
            );

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(AircraftType.SR22_G2);

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.GetInterpolatedBookDistance(aircraft, Scenario.Takeoff);

            Assert.AreEqual(910, interpolatedPerformanceData.GroundRoll);
            Assert.AreEqual(1090, interpolatedPerformanceData.DistanceToClear50Ft);

        }

        [TestMethod]
        public void TestInterpolateByPressureAltitude()
        {
            decimal actualPressureAltitude = 1500;
            decimal actualTemperature = 20;
            decimal actualAircraftWeight = 3000;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByPressureAltitude(
                performanceDataList[1],
                performanceDataList[2],
                performanceDataList[3],
                performanceDataList[4],
                actualAircraftWeight
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
            decimal actualAircraftWeight = 3000;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperature(
                performanceDataList[1],
                performanceDataList[2],
                actualAircraftWeight
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
            decimal actualAircraftWeight = 3000;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperature(
                performanceDataList[1],
                performanceDataList[2],
                actualAircraftWeight
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
            decimal actualAircraftWeight = 3000;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperature(
                performanceDataList[2],
                performanceDataList[3],
                actualAircraftWeight

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
            decimal actualAircraftWeight = 3000;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList
            );

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByPressureAltitude(
                    performanceDataList[0],
                    performanceDataList[1],
                    performanceDataList[3],
                    performanceDataList[4],
                    actualAircraftWeight
                );
            });
        }

        [TestMethod]
        public void TestInterpolateByTemperatureOutOfRange()
        {
            decimal actualPressureAltitude = 2000;
            decimal actualTemperature = -20;
            decimal actualAircraftWeight = 3000;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperature(
                    performanceDataList[0],
                    performanceDataList[1],
                    actualAircraftWeight
                );
            });
        }


        [TestMethod]
        public void TestUpperWeightDistancesAreHigherThanLowerWeightDistances()
        {
            decimal actualPressureAltitude = 2000;
            decimal actualTemperature = 20;
            decimal actualAircraftWeight = 2900;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolatorLower = GetPerformanceInterpolator(
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList);

            InterpolatedPerformanceData lowerData =
                interpolatorLower.GetInterpolatedBookDistance(
                    Aircraft.GetAircraftFromAircraftType(AircraftType.SR22_G2),
                    Scenario.Takeoff);

            actualAircraftWeight = 3400;

            PeformanceDataInterpolator interpolatorUpper = GetPerformanceInterpolator(
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList);

            InterpolatedPerformanceData upperData =
                interpolatorUpper.GetInterpolatedBookDistance(
                    Aircraft.GetAircraftFromAircraftType(AircraftType.SR22_G2),
                    Scenario.Takeoff);

            Assert.IsTrue(upperData.GroundRoll > lowerData.GroundRoll);
            Assert.IsTrue(upperData.DistanceToClear50Ft > lowerData.DistanceToClear50Ft);

        }
    }
}