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
            return SetupTestPerformanceData(AircraftType.SR22_G2);
        }

        private static BookPerformanceDataList SetupTestPerformanceData(AircraftType aircraftType)
        {
            decimal lowerWeight = Aircraft.GetAircraftFromAircraftType(aircraftType).GetLowerWeight();
            decimal higherWeight = Aircraft.GetAircraftFromAircraftType(aircraftType).GetHigherWeight();

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

            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 0, 10, lowerWeight, 120, 160));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 1000, 10, lowerWeight, 220, 260));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 2000, 10, lowerWeight, 320, 380));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 3000, 10, lowerWeight, 420, 500));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 4000, 10, lowerWeight, 520, 620));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 5000, 10, lowerWeight, 620, 740));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 0, 20, lowerWeight, 720, 860));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 1000, 20, lowerWeight, 820, 980));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 2000, 20, lowerWeight, 920, 1100));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 3000, 20, lowerWeight, 1020, 1220));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 4000, 20, lowerWeight, 1120, 1340));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 5000, 20, lowerWeight, 1220, 1450));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 0, 30, lowerWeight, 1320, 2820));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 1000, 30, lowerWeight, 1420, 2980));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 2000, 30, lowerWeight, 1520, 2100));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 3000, 30, lowerWeight, 1620, 2220));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 4000, 30, lowerWeight, 1720, 2340));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 5000, 30, lowerWeight, 1820, 2460));

            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 0, 10, higherWeight, 120, 160));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 1000, 10, higherWeight, 220, 260));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 2000, 10, higherWeight, 320, 380));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 3000, 10, higherWeight, 420, 500));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 4000, 10, higherWeight, 520, 620));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 5000, 10, higherWeight, 620, 740));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 0, 20, higherWeight, 720, 860));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 1000, 20, higherWeight, 820, 980));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 2000, 20, higherWeight, 920, 1100));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 3000, 20, higherWeight, 1020, 1220));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 4000, 20, higherWeight, 1120, 1340));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 5000, 20, higherWeight, 1220, 1450));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 0, 30, higherWeight, 1320, 2820));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 1000, 30, higherWeight, 1420, 2980));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 2000, 30, higherWeight, 1520, 2100));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 3000, 30, higherWeight, 1620, 2220));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 4000, 30, higherWeight, 1720, 2340));
            performanceDataList.Add(new BookPerformanceData(Scenario.Landing, 5000, 30, higherWeight, 1820, 2460));





            return performanceDataList;
        }

        private static PeformanceDataInterpolator GetPerformanceInterpolator(
            Scenario scenario,
            decimal actualPressureAltitude,
            decimal actualTemperature,
            decimal actualWeight,
            BookPerformanceDataList performanceDataList)
        {
            return new PeformanceDataInterpolator(
                scenario,
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
            decimal actualWeight = 3400;
            AircraftType aircraftType = AircraftType.SR22_G2;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight,
                performanceDataList
            );

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(aircraftType);

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.GetInterpolatedBookDistance(
                aircraft);

            Assert.AreEqual(850, interpolatedPerformanceData.DistanceGroundRoll);
            Assert.AreEqual(970, interpolatedPerformanceData.DistanceToClear50Ft);

        }

        [TestMethod]
        public void TestInterpolatePerformanceDataActualPressureAltitudeEqualsUpperBound()
        {
            decimal actualPressureAltitude = 2000;
            decimal actualTemperature = 20;
            decimal actualAircraftWeight = 3000;
            AircraftType aircraftType = AircraftType.SR22_G2;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList
            );

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(aircraftType);

            InterpolatedPerformanceData interpolatedPerformanceData =
                interpolator.GetInterpolatedBookDistance(aircraft);

            Assert.AreEqual(910, interpolatedPerformanceData.DistanceGroundRoll);
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
            Assert.AreEqual(250, interpolatedPerformanceData.DistanceGroundRoll);
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
            Assert.AreEqual(250, interpolatedPerformanceData.DistanceGroundRoll);
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
            Assert.AreEqual(200, interpolatedPerformanceData.DistanceGroundRoll);
            Assert.AreEqual(240, interpolatedPerformanceData.DistanceToClear50Ft);
        }

        [TestMethod]
        public void TestInterpolateByTemperatureWhenAtUpperTemperatureRange()
        {
            decimal actualPressureAltitude = 2000;
            decimal actualTemperature = 30;
            decimal actualAircraftWeight = 3400;
            AircraftType aircraftType = AircraftType.SR22_G2;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData(aircraftType);

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperature(
                interpolator.FindBookDistance(actualPressureAltitude, actualTemperature, actualAircraftWeight),
                interpolator.FindBookDistance(actualPressureAltitude, actualTemperature, actualAircraftWeight),
                actualAircraftWeight

            );

            // we're at 30 degrees C, so we should get the 30 degrees C data
            // which is 300ft ground roll and 360ft to clear 50ft obstacle
            // the data is interpolated between the 20 degrees C and 30 degrees C data
            Assert.AreEqual(1950, interpolatedPerformanceData.DistanceGroundRoll);
            Assert.AreEqual(3070, interpolatedPerformanceData.DistanceToClear50Ft);
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

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData(AircraftType.C172_SP);

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(
                Scenario.Takeoff,
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
        public void TestTakeoffDistancesAreHigherWhenWeightIncreases()
        {
            AircraftType aircraftType = AircraftType.C172_SP;

            decimal actualPressureAltitude = 2000;
            decimal actualTemperature = 20;
            decimal actualAircraftWeight = 2400;
            Scenario scenario = Scenario.Takeoff;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData(aircraftType);

            PeformanceDataInterpolator interpolatorLower = GetPerformanceInterpolator(
                scenario,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList);

            InterpolatedPerformanceData lowerData =
                interpolatorLower.GetInterpolatedBookDistance(
                    Aircraft.GetAircraftFromAircraftType(aircraftType));

            actualAircraftWeight = 2550;
            actualPressureAltitude = 2000;
            actualTemperature = 20;

            PeformanceDataInterpolator interpolatorUpper = GetPerformanceInterpolator(
                scenario,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList);

            InterpolatedPerformanceData upperData =
                interpolatorUpper.GetInterpolatedBookDistance(
                    Aircraft.GetAircraftFromAircraftType(aircraftType));

            Assert.IsTrue(upperData.DistanceGroundRoll > lowerData.DistanceGroundRoll);
            Assert.IsTrue(upperData.DistanceToClear50Ft > lowerData.DistanceToClear50Ft);

        }

        /// <summary>
        /// Test that the takeoff and landing distances are the same when the aircraft weight changes
        /// This doesn't make a lot of sense from first principles, but the POH for C172 and SR22 doesn't
        /// provide landing distances for anything other than max gross weight, so that's what we've got
        /// in the Json
        /// </summary>
        [TestMethod]
        public void TestLandingDistancesAreSameWhenWeightChanges()
        {
            AircraftType aircraftType = AircraftType.C172_SP;

            decimal actualPressureAltitude = 2000;
            decimal actualTemperature = 20;
            decimal actualAircraftWeight = 2400;
            Scenario scenario = Scenario.Landing;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData(aircraftType);

            PeformanceDataInterpolator interpolatorLower = GetPerformanceInterpolator(
                scenario,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList);

            InterpolatedPerformanceData lowerData =
                interpolatorLower.GetInterpolatedBookDistance(
                    Aircraft.GetAircraftFromAircraftType(aircraftType));

            actualAircraftWeight = 2550;
            actualPressureAltitude = 2000;
            actualTemperature = 20;


            PeformanceDataInterpolator interpolatorUpper = GetPerformanceInterpolator(
                scenario,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList);

            InterpolatedPerformanceData upperData =
                interpolatorUpper.GetInterpolatedBookDistance(
                    Aircraft.GetAircraftFromAircraftType(aircraftType));

            Assert.IsTrue(upperData.DistanceGroundRoll == lowerData.DistanceGroundRoll);
            Assert.IsTrue(upperData.DistanceToClear50Ft == lowerData.DistanceToClear50Ft);

        }




    }
}