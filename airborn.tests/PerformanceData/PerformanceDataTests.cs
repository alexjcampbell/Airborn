using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Airborn.web.Models;
using Airborn.web.Models.PerformanceData;

namespace Airborn.Tests
{
    [TestClass]
    public class PerformanceDataTests
    {

        private JsonFile jsonFileLowerWeight;
        private JsonFile jsonFileHigherWeight;

        int _aircraftLowerWeight = Aircraft.GetAircraftFromAircraftType(UtilitiesForTesting.Default_AircraftType).LowestPossibleWeight;
        int _aircraftHigherWeight = Aircraft.GetAircraftFromAircraftType(UtilitiesForTesting.Default_AircraftType).HighestPossibleWeight;

        private Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(UtilitiesForTesting.Default_AircraftType);

        [TestInitialize]
        public void Setup()
        {
            List<JsonFile> jsonFiles = UtilitiesForTesting.GetMockJsonPerformanceFilesForTesting();

            jsonFileLowerWeight = jsonFiles[0];
            jsonFileHigherWeight = jsonFiles[1];

        }

        [TestMethod]
        public void Test_JsonFileHasMoreThanOneResult()
        {
            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            Assert.IsTrue(bookNumbersList.Count > 0);

        }

        [TestMethod]
        public void Test_JsonFileHasMoreThanOneTakeoffResult()
        {

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            Assert.IsTrue(bookNumbersList.FindAll(x => x.Scenario == Scenario.Takeoff).Count > 0);

        }

        [TestMethod]
        public void Test_JsonFileHasMoreThanOneLandingResult()
        {

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            Assert.IsTrue(bookNumbersList.FindAll(x => x.Scenario == Scenario.Landing).Count > 0);

        }

        [TestMethod]
        public void Test_JsonFileTakeoffProfilesGroundRollIsWhatWeExpect()
        {
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            double expectedGroundRoll_0 = 10;

            Assert.AreEqual(
                expectedGroundRoll_0,
                bookNumbersList.TakeoffPerformanceData[0].DistanceGroundRoll.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

        }

        [TestMethod]
        public void Test_JsonFileTakeoffProfiles50FtIsWhatWeExpect()
        {
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            double expectedDistance = 168;

            Assert.AreEqual(
                expectedDistance,
                bookNumbersList.TakeoffPerformanceData[4].DistanceToClear50Ft.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_JsonFileLandingProfilesGroundRollIsWhatWeExpect()
        {

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            double expectedGroundRoll_0 = 10;

            Assert.AreEqual(
                expectedGroundRoll_0,
                bookNumbersList.LandingPerformanceData[0].DistanceGroundRoll.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );

        }

        [TestMethod]
        public void Test_JsonFileLandingProfiles50FtIsWhatWeExpect()
        {
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            double expectedDistance = 119.999;

            Assert.AreEqual(
                expectedDistance,
                bookNumbersList.LandingPerformanceData[0].DistanceToClear50Ft.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison)
                ;

        }

        [TestMethod]
        public void Test_BookDistancesSetup()
        {

            double temperature = 10;
            Distance pressureAltitude = Distance.FromFeet(1000);
            Distance groundRoll = Distance.FromFeet(100);
            Distance distanceToClear50Ft = Distance.FromFeet(120);
            double aircraftWeight = 3000;
            Scenario scenario = Scenario.Takeoff;

            PerformanceDataBase bookDistances = new BookPerformanceData(
                scenario,
                pressureAltitude,
                temperature,
                aircraftWeight,
                groundRoll,
                distanceToClear50Ft
                );

            Assert.AreEqual(temperature, bookDistances.Temperature);
            Assert.AreEqual(pressureAltitude, bookDistances.PressureAltitude);
            Assert.AreEqual(groundRoll, bookDistances.DistanceGroundRoll);
            Assert.AreEqual(distanceToClear50Ft, bookDistances.DistanceToClear50Ft);
            Assert.AreEqual(scenario, bookDistances.Scenario);
        }

        [TestMethod]
        public void Test_GetTakeoffBookDistancesOnlyReturnsTakeoffResults()
        {

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            var takeoffBookDistances = bookNumbersList.TakeoffPerformanceData;

            Assert.IsTrue(takeoffBookDistances.Count > 0);

            foreach (var bookDistance in takeoffBookDistances)
            {
                Assert.AreEqual(Scenario.Takeoff, bookDistance.Scenario);
            }
        }

        [TestMethod]
        public void Test_GetLandingBookDistancesOnlyReturnsLandingResults()
        {

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            var landingBookDistances = bookNumbersList.LandingPerformanceData;

            Assert.IsTrue(landingBookDistances.Count > 0);

            foreach (var bookDistance in landingBookDistances)
            {
                Assert.AreEqual(Scenario.Landing, bookDistance.Scenario);
            }
        }

        [TestMethod]
        public void Test_GetBookDistancesReturnsCorrectNumberOfResults()
        {

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            var takeoffBookDistances = bookNumbersList.TakeoffPerformanceData;
            var landingBookDistances = bookNumbersList.LandingPerformanceData;

            Assert.AreEqual(48, takeoffBookDistances.Count);
            Assert.AreEqual(48, landingBookDistances.Count);
        }

        [TestMethod]
        public void Test_FindBookDistancesReturnsCorrectResult()
        {

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            var takeoffBookDistances = bookNumbersList.TakeoffPerformanceData;
            var landingBookDistances = bookNumbersList.LandingPerformanceData;

            Distance pressureAltitude = Distance.FromFeet(0);
            double temperature = 10;
            double aircraftWeight = 2900;


            var takeoffBookDistance = bookNumbersList.FindBookDistance(Scenario.Takeoff, pressureAltitude, temperature, aircraftWeight);
            var landingBookDistance = bookNumbersList.FindBookDistance(Scenario.Landing, pressureAltitude, temperature, aircraftWeight);

            Assert.AreEqual(20, takeoffBookDistance.DistanceGroundRoll.Value.TotalFeet);
            Assert.AreEqual(132, takeoffBookDistance.DistanceToClear50Ft.Value.TotalFeet);

            Assert.AreEqual(20, landingBookDistance.DistanceGroundRoll.Value.TotalFeet);
            Assert.AreEqual(132, landingBookDistance.DistanceToClear50Ft.Value.TotalFeet);
        }


    }
}