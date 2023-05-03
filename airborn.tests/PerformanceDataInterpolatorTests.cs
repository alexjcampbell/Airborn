using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;
using Airborn.web.Models.PerformanceData;


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
       double actualTemperature,
       double actualWeight
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
            double actualTemperature,
            double actualWeight,
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
        public void Test_InterpolatePerformanceDataActualPressureAltitudeEqualsZeroAndTemperatureEqualsZero_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(0);
            double actualTemperature = 0;
            double actualWeight = 3000;

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight
                );

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(UtilitiesForTesting.Default_AircraftType);

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.GetInterpolatedBookDistances(
                aircraft);

            double expectedGroundRoll = 300;

            Assert.AreEqual(
                expectedGroundRoll,
                interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

            double expectedDistanceToClear50Ft = 360;

            Assert.AreEqual(
                expectedDistanceToClear50Ft,
                interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_InterpolatePerformanceDataActualPressureAltitudeEqualsLowerBound_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(1000);
            double actualTemperature = 20;
            double actualWeight = 3000;

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight
                );

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(UtilitiesForTesting.Default_AircraftType);

            InterpolatedPerformanceData interpolatedPerformanceData = interpolator.GetInterpolatedBookDistances(
                aircraft);

            double expectedGroundRoll = 1320;

            Assert.AreEqual(
                expectedGroundRoll,
                interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

            double expectedDistanceToClear50Ft = 1584;

            Assert.AreEqual(
                expectedDistanceToClear50Ft,
                interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

        }

        [TestMethod]
        public void Test_InterpolatePerformanceDataActualPressureAltitudeEqualsUpperBound_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(2000);
            double actualTemperature = 20;
            double actualWeight = 3000;

            PeformanceDataInterpolator interpolator = GetPerformanceInterpolator(
                Scenario.Takeoff,
                actualPressureAltitude,
                actualTemperature,
                actualWeight
                );

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(UtilitiesForTesting.Default_AircraftType);

            InterpolatedPerformanceData interpolatedPerformanceData =
                interpolator.GetInterpolatedBookDistances(aircraft);

            double expectedGroundRoll = 2320;

            Assert.AreEqual(
                expectedGroundRoll,
                interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

            double expectedDistanceToClear50Ft = 2784;

            Assert.AreEqual(
                expectedDistanceToClear50Ft,
                interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

        }


        [TestMethod]
        public void Test_InterpolateByPressureAltitudeOnlyBottomOfRange_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(0);
            double actualTemperature = 20;
            double actualWeight = 3000;

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

            double expectedGroundRoll = 1000;

            Assert.AreEqual(
                expectedGroundRoll,
                interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

            double expectedDistanceToClear50Ft = 20000;

            Assert.AreEqual(
                expectedDistanceToClear50Ft,
                interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_InterpolateByPressureAltitudeOnlyMidRange_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(1500);
            double actualTemperature = 20;
            double actualWeight = 3000;

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

            double expectedGroundRoll = 2000;

            Assert.AreEqual(
                expectedGroundRoll,
                interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

            double expectedDistanceToClear50Ft = 3000;

            Assert.AreEqual(
                expectedDistanceToClear50Ft,
                interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }


        [TestMethod]
        public void Test_InterpolateByPressureAltitudeOnlyTopOfRange_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(2000);
            double actualTemperature = 20;
            double actualWeight = 3000;

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

            double expectedGroundRoll = 3000;

            Assert.AreEqual(
                expectedGroundRoll,
                interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

            double expectedDistanceToClear50Ft = 40000;

            Assert.AreEqual(
                expectedDistanceToClear50Ft,
                interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

        }

        [TestMethod]
        public void Test_InterpolateByTemperatureOnlyBottomOfRange_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(2000);
            double actualTemperature = 10;
            double actualWeight = 3000;

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

            double expectedGroundRoll = 1000;

            Assert.AreEqual(
                expectedGroundRoll,
                interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

            double expectedDistanceToClear50Ft = 2000;

            Assert.AreEqual(
                expectedDistanceToClear50Ft,
                interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_InterpolateByTemperatureOnlyMidRange_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(1500);
            double actualTemperature = 15;
            double actualWeight = 3000;

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


            double expectedGroundRoll = 2000;

            Assert.AreEqual(
                expectedGroundRoll,
                interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

            double expectedDistanceToClear50Ft = 3000;

            Assert.AreEqual(
                expectedDistanceToClear50Ft,
                interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }


        [TestMethod]
        public void Test_InterpolateByTemperatureWhenAtUpperTemperatureRange_ReturnsCorrectResults()
        {
            Distance actualPressureAltitude = Distance.FromFeet(1500);
            double actualTemperature = 20;
            double actualWeight = 3000;

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


            double expectedGroundRoll = 3000;

            Assert.AreEqual(
                expectedGroundRoll,
                interpolatedPerformanceData.DistanceGroundRoll.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

            double expectedDistanceToClear50Ft = 4000;

            Assert.AreEqual(
                expectedDistanceToClear50Ft,
                interpolatedPerformanceData.DistanceToClear50Ft.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_InterpolateByPressureAltitudeOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            Distance actualPressureAltitude = Distance.FromFeet(-1000);
            double actualTemperature = 20;
            double actualWeight = 3000;

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
        public void Test_InterpolateByTemperature_WhenOutOfRange_ThrowsArgumentOutOfRangeException()
        {
            Distance actualPressureAltitude = Distance.FromFeet(2000);
            double actualTemperature = -20;
            double actualWeight = 3000;

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
        public void Test_WhenWeightIncreases_TakeoffDistancesAreHigher()
        {

            Distance actualPressureAltitude = Distance.FromFeet(2000);
            double actualTemperature = 20;
            double actualAircraftWeight = 2900;

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