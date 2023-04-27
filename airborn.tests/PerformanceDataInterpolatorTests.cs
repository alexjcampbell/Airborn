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

        const AircraftType _defaultAircraftType = AircraftType.SR22_G2;

        private static BookPerformanceDataList SetupTestPerformanceData()
        {
            return SetupTestPerformanceData(AircraftType.SR22_G2);
        }

        private static BookPerformanceDataList SetupTestPerformanceData(AircraftType aircraftType)
        {
            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(aircraftType);

            return BookPerformanceDataTestSetup.GenerateSampleData(aircraftType);
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
                performanceDataList,
                new PerformanceCalculationLogItem("")
            );
        }


        [TestMethod]
        public void TestInterpolatePerformanceDataActualPressureAltitudeEqualsZeroAndTemperatureEqualsZero()
        {
            decimal actualPressureAltitude = 0;
            decimal actualTemperature = 0;
            decimal actualWeight = 3000;
            AircraftType aircraftType = AircraftType.SR22_G2;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight,
                performanceDataList,
                new PerformanceCalculationLogItem("")
            );

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(aircraftType);

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.GetInterpolatedBookDistances(
                aircraft);

            Assert.AreEqual(300, interpolatedPerformanceData.DistanceGroundRoll);
            Assert.AreEqual(360, interpolatedPerformanceData.DistanceToClear50Ft);

        }

        [TestMethod]
        public void TestInterpolatePerformanceDataActualPressureAltitudeEqualsLowerBound()
        {
            decimal actualPressureAltitude = 1000;
            decimal actualTemperature = 20;
            decimal actualWeight = 3000;
            AircraftType aircraftType = AircraftType.SR22_G2;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight,
                performanceDataList,
                new PerformanceCalculationLogItem("")
            );

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(aircraftType);

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.GetInterpolatedBookDistances(
                aircraft);

            Assert.AreEqual(1320, interpolatedPerformanceData.DistanceGroundRoll);
            Assert.AreEqual(1584, interpolatedPerformanceData.DistanceToClear50Ft);

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
                performanceDataList,
                new PerformanceCalculationLogItem("")

            );

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(aircraftType);

            InterpolatedPerformanceData interpolatedPerformanceData =
                interpolator.GetInterpolatedBookDistances(aircraft);

            Assert.AreEqual(2320, interpolatedPerformanceData.DistanceGroundRoll);
            Assert.AreEqual(2784, interpolatedPerformanceData.DistanceToClear50Ft);

        }


        [TestMethod]
        public void TestInterpolateByPressureAltitudeOnlyBottomOfRange()
        {
            decimal actualPressureAltitude = 0;
            decimal actualTemperature = 20;
            decimal actualAircraftWeight = 3000;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList,
                new PerformanceCalculationLogItem("")
            );

            BookPerformanceData d1 = new BookPerformanceData(
                Scenario.Takeoff,
                0, // pressure altitude
                actualTemperature,
                actualAircraftWeight,
                1000, // distance ground roll
                20000 // distance to clear 50 ft
            );

            BookPerformanceData d2 = new BookPerformanceData(
                Scenario.Takeoff,
                1000, // pressure altitude
                actualTemperature,
                actualAircraftWeight,
                3000, // distance ground roll
                40000 // distance to clear 50 ft
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByPressureAltitudeOnly(
                d1,
                d2,
                actualAircraftWeight
            );


            Assert.AreEqual(1000, interpolatedPerformanceData.DistanceGroundRoll);
            Assert.AreEqual(20000, interpolatedPerformanceData.DistanceToClear50Ft);
        }

        [TestMethod]
        public void TestInterpolateByPressureAltitudeOnlyMidRange()
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
                performanceDataList,
                new PerformanceCalculationLogItem("")
            );

            BookPerformanceData d1 = new BookPerformanceData(
                Scenario.Takeoff,
                1000, // actual pressure altitude
                10, // actual temperature
                actualAircraftWeight,
                1000, // weight
                2000 // distance to clear 50 ft
            );

            BookPerformanceData d2 = new BookPerformanceData(
                Scenario.Takeoff,
                2000, // actual pressure altitude
                10, // actual temperature
                actualAircraftWeight,
                3000, // weight
                4000 // distance to clear 50 ft
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByPressureAltitudeOnly(
                d1,
                d2,
                actualAircraftWeight
            );


            Assert.AreEqual(2000, interpolatedPerformanceData.DistanceGroundRoll);
            Assert.AreEqual(3000, interpolatedPerformanceData.DistanceToClear50Ft);
        }


        [TestMethod]
        public void TestInterpolateByPressureAltitudeOnlyTopOfRange()
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
                performanceDataList,
                new PerformanceCalculationLogItem("")
            );

            BookPerformanceData d1 = new BookPerformanceData(
                Scenario.Takeoff,
                1000, // pressure altitude
                10, // temperature
                actualAircraftWeight,
                1500, // ground roll
                20000 // distance to clear 50ft
            );

            BookPerformanceData d2 = new BookPerformanceData(
                Scenario.Takeoff,
                2000, // pressure altitude
                10, // temperature
                actualAircraftWeight,
                3000, // ground roll
                40000 // distance to clear 50ft
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByPressureAltitudeOnly(
                d1,
                d2,
                actualAircraftWeight
            );


            Assert.AreEqual(3000, interpolatedPerformanceData.DistanceGroundRoll);
            Assert.AreEqual(40000, interpolatedPerformanceData.DistanceToClear50Ft);
        }

        [TestMethod]
        public void TestInterpolateByTemperatureOnlyBottomOfRange()
        {
            decimal actualPressureAltitude = 2000;
            decimal actualTemperature = 10;
            decimal actualAircraftWeight = 3000;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList,
                new PerformanceCalculationLogItem("")
            );

            BookPerformanceData d1 = new BookPerformanceData(
                Scenario.Takeoff,
                2000, // pressure altitude
                10, // temperature
                actualAircraftWeight,
                1000, // ground roll
                2000 // to clear 50ft obstacle
            );

            BookPerformanceData d2 = new BookPerformanceData(
                Scenario.Takeoff,
                2000, // pressure altitude
                20, // temperature
                actualAircraftWeight,
                3000, // ground roll
                4000 // to clear 50ft obstacle
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperatureOnly(
                d1,
                d2,
                actualAircraftWeight
            );

            // we're at 20 degrees C, so we should get the 20 degrees C data
            // which is 200ft ground roll and 240ft to clear 50ft obstacle
            // the data is interpolated between the 20 degrees C and 30 degrees C data
            Assert.AreEqual(1000, interpolatedPerformanceData.DistanceGroundRoll);
            Assert.AreEqual(2000, interpolatedPerformanceData.DistanceToClear50Ft);
        }

        [TestMethod]
        public void TestInterpolateByTemperatureOnlyMidRange()
        {
            decimal actualPressureAltitude = 1500;
            decimal actualTemperature = 15;
            decimal actualAircraftWeight = 3000;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = new PeformanceDataInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList,
                new PerformanceCalculationLogItem("")
            );

            BookPerformanceData d1 = new BookPerformanceData(
                Scenario.Takeoff,
                2000, // pressure altitude
                10, // temperature
                actualAircraftWeight,
                1000, // ground roll
                2000 // to clear 50ft obstacle
            );

            BookPerformanceData d2 = new BookPerformanceData(
                Scenario.Takeoff,
                2000, // pressure altitude
                20, // temperature
                actualAircraftWeight,
                3000, // ground roll
                4000 // to clear 50ft obstacle
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperatureOnly(
                d1,
                d2,
                actualAircraftWeight
            );

            // we're at 25 degrees C, so we should get the 25 degrees C data
            // which is 250ft ground roll and 300ft to clear 50ft obstacle
            // the data is interpolated between the 20 degrees C and 30 degrees C data
            Assert.AreEqual(2000, interpolatedPerformanceData.DistanceGroundRoll);
            Assert.AreEqual(3000, interpolatedPerformanceData.DistanceToClear50Ft);
        }




        [TestMethod]
        public void TestInterpolateByTemperatureWhenAtUpperTemperatureRange()
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
                performanceDataList,
                new PerformanceCalculationLogItem("")
            );

            BookPerformanceData d1 = new BookPerformanceData(
                Scenario.Takeoff,
                2000, // pressure altitude
                10, // temperature
                actualAircraftWeight,
                1000, // ground roll
                2000 // to clear 50ft obstacle
            );

            BookPerformanceData d2 = new BookPerformanceData(
                Scenario.Takeoff,
                2000, // pressure altitude
                20, // temperature
                actualAircraftWeight,
                3000, // ground roll
                4000 // to clear 50ft obstacle
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperatureOnly(
                d1,
                d2,
                actualAircraftWeight
            );

            // we're at 25 degrees C, so we should get the 25 degrees C data
            // which is 250ft ground roll and 300ft to clear 50ft obstacle
            // the data is interpolated between the 20 degrees C and 30 degrees C data
            Assert.AreEqual(3000, interpolatedPerformanceData.DistanceGroundRoll);
            Assert.AreEqual(4000, interpolatedPerformanceData.DistanceToClear50Ft);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
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
                performanceDataList,
                new PerformanceCalculationLogItem("")
            );


            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByPressureAltitude(
                performanceDataList[0],
                performanceDataList[1],
                performanceDataList[3],
                performanceDataList[4],
                actualAircraftWeight
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
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
                InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperatureOnly(
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
                interpolatorLower.GetInterpolatedBookDistances(
                    Aircraft.GetAircraftFromAircraftType(aircraftType));

            actualAircraftWeight = 2550;

            PeformanceDataInterpolator interpolatorUpper = GetPerformanceInterpolator(
                scenario,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList);

            InterpolatedPerformanceData upperData =
                interpolatorUpper.GetInterpolatedBookDistances(
                    Aircraft.GetAircraftFromAircraftType(aircraftType));

            Assert.IsTrue(upperData.DistanceGroundRoll > lowerData.DistanceGroundRoll);
            Assert.IsTrue(upperData.DistanceToClear50Ft > lowerData.DistanceToClear50Ft);

        }


    }
}