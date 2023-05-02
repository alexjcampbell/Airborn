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
            return SetupTestPerformanceData(UtilitiesForTesting.Default_AircraftType);
        }

        private static BookPerformanceDataList SetupTestPerformanceData(AircraftType aircraftType)
        {
            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(aircraftType);

            return BookPerformanceDataTestSetup.GenerateSampleData(aircraftType);
        }

        private static PeformanceDataInterpolator GetPerformanceInterpolator(
       Scenario scenario,
       Distance actualPressureAltitude,
       decimal actualTemperature,
       decimal actualWeight
       )
        {
            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            return new PeformanceDataInterpolator(
                scenario,
                actualPressureAltitude,
                actualTemperature,
                actualWeight,
                performanceDataList,
                new PerformanceCalculationLogItem("")
            );
        }

        private static PeformanceDataInterpolator GetPerformanceInterpolator(
            Scenario scenario,
            Distance actualPressureAltitude,
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
        public void TestInterpolatePerformanceDataActualPressureAltitudeEqualsZeroAndTemperatureEqualsZero_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(0);
            decimal actualTemperature = 0;
            decimal actualWeight = 3000;

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight
                );

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(UtilitiesForTesting.Default_AircraftType);

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.GetInterpolatedBookDistances(
                aircraft);

            Assert.AreEqual(300, interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet);
            Assert.AreEqual(360, interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet);

        }

        [TestMethod]
        public void TestInterpolatePerformanceDataActualPressureAltitudeEqualsLowerBound_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(1000);
            decimal actualTemperature = 20;
            decimal actualWeight = 3000;

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight
                );

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(UtilitiesForTesting.Default_AircraftType);

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.GetInterpolatedBookDistances(
                aircraft);

            Assert.AreEqual(1320, interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet);
            Assert.AreEqual(1584, interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet);

        }

        [TestMethod]
        public void TestInterpolatePerformanceDataActualPressureAltitudeEqualsUpperBound_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(2000);
            decimal actualTemperature = 20;
            decimal actualWeight = 3000;

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight
                );

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(UtilitiesForTesting.Default_AircraftType);

            InterpolatedPerformanceData interpolatedPerformanceData =
                interpolator.GetInterpolatedBookDistances(aircraft);

            Assert.AreEqual(2320, interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet);
            Assert.AreEqual(2784, interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet);

        }


        [TestMethod]
        public void TestInterpolateByPressureAltitudeOnlyBottomOfRange_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(0);
            decimal actualTemperature = 20;
            decimal actualWeight = 3000;

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight
                );

            BookPerformanceData d1 = new BookPerformanceData(
                Scenario.Takeoff,
                Distance.FromFeet(0), // pressure altitude
                actualTemperature,
                actualWeight,
                Distance.FromFeet(1000), // distance ground roll
                Distance.FromFeet(20000) // distance to clear 50 ft
            );

            BookPerformanceData d2 = new BookPerformanceData(
                Scenario.Takeoff,
                Distance.FromFeet(1000), // pressure altitude
                actualTemperature,
                actualWeight,
                Distance.FromFeet(3000), // distance ground roll
                Distance.FromFeet(40000) // distance to clear 50 ft
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByPressureAltitudeOnly(
                d1,
                d2,
                actualWeight
            );


            Assert.AreEqual(1000, interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet);
            Assert.AreEqual(20000, interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet);
        }

        [TestMethod]
        public void TestInterpolateByPressureAltitudeOnlyMidRange_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(1500);
            decimal actualTemperature = 20;
            decimal actualWeight = 3000;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight
                );

            BookPerformanceData d1 = new BookPerformanceData(
                Scenario.Takeoff,
                Distance.FromFeet(1000), // actual pressure altitude
                actualTemperature, // actual temperature
                actualWeight,
                Distance.FromFeet(1000), // weight
                Distance.FromFeet(2000) // distance to clear 50 ft
            );

            BookPerformanceData d2 = new BookPerformanceData(
                Scenario.Takeoff,
                Distance.FromFeet(2000), // actual pressure altitude
                actualTemperature, // actual temperature
                actualWeight,
                Distance.FromFeet(3000), // weight
                Distance.FromFeet(4000) // distance to clear 50 ft
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByPressureAltitudeOnly(
                d1,
                d2,
                actualWeight
            );


            Assert.AreEqual(2000, interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet);
            Assert.AreEqual(3000, interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet);
        }


        [TestMethod]
        public void TestInterpolateByPressureAltitudeOnlyTopOfRange_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(2000);
            decimal actualTemperature = 20;
            decimal actualWeight = 3000;

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight
                );

            BookPerformanceData d1 = new BookPerformanceData(
                Scenario.Takeoff,
                Distance.FromFeet(1000), // pressure altitude
                10, // temperature
                actualWeight,
                Distance.FromFeet(1500), // ground roll
                Distance.FromFeet(20000) // distance to clear 50ft
            );

            BookPerformanceData d2 = new BookPerformanceData(
                Scenario.Takeoff,
                Distance.FromFeet(2000), // pressure altitude
                10, // temperature
                actualWeight,
                Distance.FromFeet(3000), // ground roll
                Distance.FromFeet(40000) // distance to clear 50ft
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByPressureAltitudeOnly(
                d1,
                d2,
                actualWeight
            );


            Assert.AreEqual(3000, interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet);
            Assert.AreEqual(40000, interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet);
        }

        [TestMethod]
        public void TestInterpolateByTemperatureOnlyBottomOfRange_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(2000);
            decimal actualTemperature = 10;
            decimal actualWeight = 3000;

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight
                );

            BookPerformanceData d1 = new BookPerformanceData(
                Scenario.Takeoff,
                actualPressureAltitude, // pressure altitude
                10, // temperature
                actualWeight,
                Distance.FromFeet(1000), // ground roll
                Distance.FromFeet(2000) // to clear 50ft obstacle
            );

            BookPerformanceData d2 = new BookPerformanceData(
                Scenario.Takeoff,
                actualPressureAltitude, // pressure altitude
                20, // temperature
                actualWeight,
                Distance.FromFeet(3000), // ground roll
                Distance.FromFeet(4000) // to clear 50ft obstacle
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperatureOnly(
                d1,
                d2,
                actualWeight
            );

            // we're at 20 degrees C, so we should get the 20 degrees C data
            // which is 200ft ground roll and 240ft to clear 50ft obstacle
            // the data is interpolated between the 20 degrees C and 30 degrees C data
            Assert.AreEqual(1000, interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet);
            Assert.AreEqual(2000, interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet);
        }

        [TestMethod]
        public void TestInterpolateByTemperatureOnlyMidRange_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(1500);
            decimal actualTemperature = 15;
            decimal actualWeight = 3000;

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight
                );

            BookPerformanceData d1 = new BookPerformanceData(
                Scenario.Takeoff,
                actualPressureAltitude, // pressure altitude
                10, // temperature
                actualWeight,
                Distance.FromFeet(1000), // ground roll
                Distance.FromFeet(2000) // to clear 50ft obstacle
            );

            BookPerformanceData d2 = new BookPerformanceData(
                Scenario.Takeoff,
                actualPressureAltitude, // pressure altitude
                20, // temperature
                actualWeight,
                Distance.FromFeet(3000), // ground roll
                Distance.FromFeet(4000) // to clear 50ft obstacle
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperatureOnly(
                d1,
                d2,
                actualWeight
            );

            // we're at 25 degrees C, so we should get the 25 degrees C data
            // which is 250ft ground roll and 300ft to clear 50ft obstacle
            // the data is interpolated between the 20 degrees C and 30 degrees C data
            Assert.AreEqual(2000, interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet);
            Assert.AreEqual(3000, interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet);
        }


        [TestMethod]
        public void TestInterpolateByTemperatureWhenAtUpperTemperatureRange_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(1500);
            decimal actualTemperature = 20;
            decimal actualWeight = 3000;

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight
                );

            BookPerformanceData d1 = new BookPerformanceData(
                Scenario.Takeoff,
                actualPressureAltitude, // pressure altitude
                10, // temperature
                actualWeight,
                Distance.FromFeet(1000), // ground roll
                Distance.FromFeet(2000) // to clear 50ft obstacle
            );

            BookPerformanceData d2 = new BookPerformanceData(
                Scenario.Takeoff,
                actualPressureAltitude, // pressure altitude
                20, // temperature
                actualWeight,
                Distance.FromFeet(3000), // ground roll
                Distance.FromFeet(4000) // to clear 50ft obstacle
            );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperatureOnly(
                d1,
                d2,
                actualWeight
            );

            // we're at 25 degrees C, so we should get the 25 degrees C data
            // which is 250ft ground roll and 300ft to clear 50ft obstacle
            // the data is interpolated between the 20 degrees C and 30 degrees C data
            Assert.AreEqual(3000, interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet);
            Assert.AreEqual(4000, interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInterpolateByPressureAltitudeOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            Distance actualPressureAltitude = Distance.FromFeet(-1000);
            decimal actualTemperature = 20;
            decimal actualWeight = 3000;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData();

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight
                );

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByPressureAltitude(
                performanceDataList[0],
                performanceDataList[1],
                performanceDataList[3],
                performanceDataList[4],
                actualWeight
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInterpolateByTemperature_WhenOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            Distance actualPressureAltitude = Distance.FromFeet(2000);
            decimal actualTemperature = -20;
            decimal actualWeight = 3000;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData(UtilitiesForTesting.Default_AircraftType);

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight
                );

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                InterpolatedPerformanceData interpolatedPerformanceData = interpolator.InterpolateByTemperatureOnly(
                    performanceDataList[0],
                    performanceDataList[1],
                    actualWeight
                );
            });
        }

        [TestMethod]
        public void TestWhenWeightIncreases_TakeoffDistancesAreHigher()
        {

            Distance actualPressureAltitude = Distance.FromFeet(2000);
            decimal actualTemperature = 20;
            decimal actualAircraftWeight = 2900;

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(UtilitiesForTesting.Default_AircraftType);

            Scenario scenario = Scenario.Takeoff;

            BookPerformanceDataList performanceDataList = SetupTestPerformanceData(UtilitiesForTesting.Default_AircraftType);

            PeformanceDataInterpolator interpolatorLower = GetPerformanceInterpolator(
                scenario,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList);

            InterpolatedPerformanceData lowerData =
                interpolatorLower.GetInterpolatedBookDistances(
                    aircraft);

            actualAircraftWeight = 3400;

            PeformanceDataInterpolator interpolatorUpper = GetPerformanceInterpolator(
                scenario,
                actualPressureAltitude,
                actualTemperature,
                actualAircraftWeight,
                performanceDataList);

            InterpolatedPerformanceData upperData =
                interpolatorUpper.GetInterpolatedBookDistances(
                    aircraft);

            Assert.IsTrue(upperData.DistanceGroundRoll.Value.TotalFeet > lowerData.DistanceGroundRoll.Value.TotalFeet);
            Assert.IsTrue(upperData.DistanceToClear50Ft.Value.TotalFeet > lowerData.DistanceToClear50Ft.Value.TotalFeet);

        }


    }
}