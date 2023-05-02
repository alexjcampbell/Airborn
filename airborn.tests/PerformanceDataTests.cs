using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;
using System.Collections.Generic;
using Moq;

namespace Airborn.Tests
{
    [TestClass]
    public class PerformanceDataTests
    {

        private JsonFile jsonFileLowerWeight;
        private JsonFile jsonFileHigherWeight;

        int _aircraftLowerWeight = Aircraft.GetAircraftFromAircraftType(UtilitiesForTesting.Default_AircraftType).GetLowerWeight();
        int _aircraftHigherWeight = Aircraft.GetAircraftFromAircraftType(UtilitiesForTesting.Default_AircraftType).GetHigherWeight();

        private Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(UtilitiesForTesting.Default_AircraftType);

        [TestInitialize]
        public void Setup()
        {
            List<JsonFile> jsonFiles = UtilitiesForTesting.GetMockJsonPerformanceFilesForTesting();

            jsonFileLowerWeight = jsonFiles[0];
            jsonFileHigherWeight = jsonFiles[1];

        }

        [TestMethod]
        public void TestJsonFileHasMoreThanOneResult()
        {
            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            Assert.IsTrue(bookNumbersList.Count > 0);

        }

        [TestMethod]
        public void TestJsonFileHasMoreThanOneTakeoffResult()
        {

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            Assert.IsTrue(bookNumbersList.FindAll(x => x.Scenario == Scenario.Takeoff).Count > 0);

        }

        [TestMethod]
        public void TestJsonFileHasMoreThanOneLandingResult()
        {

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            Assert.IsTrue(bookNumbersList.FindAll(x => x.Scenario == Scenario.Landing).Count > 0);

        }

        [TestMethod]
        public void TestJsonFileTakeoffProfilesGroundRollIsWhatWeExpect()
        {
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            double expectedGroundRoll_0 = 10;

            Assert.AreEqual(
                expectedGroundRoll_0,
                (double)bookNumbersList.TakeoffPerformanceData[0].DistanceGroundRoll.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDistanceComparison
                );

        }

        [TestMethod]
        public void TestJsonFileTakeoffProfiles50FtIsWhatWeExpect()
        {
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            double expectedDistance = 168;

            Assert.AreEqual(
                expectedDistance,
                (double)bookNumbersList.TakeoffPerformanceData[4].DistanceToClear50Ft.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDistanceComparison
                );
        }

        [TestMethod]
        public void TestJsonFileLandingProfilesGroundRollIsWhatWeExpect()
        {

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            double expectedGroundRoll_0 = 10;

            Assert.AreEqual(
                expectedGroundRoll_0,
                (double)bookNumbersList.LandingPerformanceData[0].DistanceGroundRoll.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDistanceComparison
                );

        }

        [TestMethod]
        public void TestJsonFileLandingProfiles50FtIsWhatWeExpect()
        {
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            double expectedDistance = 119.999;

            Assert.AreEqual(
                expectedDistance,
                (double)bookNumbersList.LandingPerformanceData[0].DistanceToClear50Ft.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDistanceComparison)
                ;

        }

        [TestMethod]
        public void TestBookDistancesSetup()
        {

            decimal temperature = 10;
            Distance pressureAltitude = Distance.FromFeet(1000);
            Distance groundRoll = Distance.FromFeet(100);
            Distance distanceToClear50Ft = Distance.FromFeet(120);
            decimal aircraftWeight = 3000;
            Scenario scenario = Scenario.Takeoff;

            PerformanceData bookDistances = new BookPerformanceData(
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
        public void TestGetTakeoffBookDistancesOnlyReturnsTakeoffResults()
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
        public void TestGetLandingBookDistancesOnlyReturnsLandingResults()
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
        public void TestGetBookDistancesReturnsCorrectNumberOfResults()
        {

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            var takeoffBookDistances = bookNumbersList.TakeoffPerformanceData;
            var landingBookDistances = bookNumbersList.LandingPerformanceData;

            Assert.AreEqual(48, takeoffBookDistances.Count);
            Assert.AreEqual(48, landingBookDistances.Count);
        }

        [TestMethod]
        public void TestFindBookDistancesReturnsCorrectResult()
        {

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);

            var takeoffBookDistances = bookNumbersList.TakeoffPerformanceData;
            var landingBookDistances = bookNumbersList.LandingPerformanceData;

            Distance pressureAltitude = Distance.FromFeet(0);
            decimal temperature = 10;
            decimal aircraftWeight = 2900;


            var takeoffBookDistance = bookNumbersList.FindBookDistance(Scenario.Takeoff, pressureAltitude, temperature, aircraftWeight);
            var landingBookDistance = bookNumbersList.FindBookDistance(Scenario.Landing, pressureAltitude, temperature, aircraftWeight);

            Assert.AreEqual(20, takeoffBookDistance.DistanceGroundRoll.Value.TotalFeet);
            Assert.AreEqual(132, takeoffBookDistance.DistanceToClear50Ft.Value.TotalFeet);

            Assert.AreEqual(20, landingBookDistance.DistanceGroundRoll.Value.TotalFeet);
            Assert.AreEqual(132, landingBookDistance.DistanceToClear50Ft.Value.TotalFeet);
        }


    }
}