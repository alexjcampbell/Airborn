using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;
using System.Collections.Generic;
using Moq;

namespace Airborn.Tests
{
    [TestClass]
    public class PerformanceDataTests
    {

        Mock<JsonFile> jsonFileLowerWeight = new Mock<JsonFile>();
        Mock<JsonFile> jsonFileHigherWeight = new Mock<JsonFile>();

        Aircraft aircraft = new SR22T_G5();

        private int _aircraftLowerWeight = Aircraft.GetAircraftFromAircraftType(AircraftType.SR22_G2).GetLowerWeight();
        private int _aircraftHigherWeight = Aircraft.GetAircraftFromAircraftType(AircraftType.SR22_G2).GetHigherWeight();

        private const double _minimumDistancePrecision = 0.01d;

        public void Setup()
        {
            jsonFileLowerWeight.Setup(x => x.TakeoffProfiles).Returns(new JsonPerformanceProfileList());
            jsonFileLowerWeight.Setup(x => x.LandingProfiles).Returns(new JsonPerformanceProfileList());

            jsonFileLowerWeight.Object.AircraftWeight = _aircraftLowerWeight;

            jsonFileLowerWeight.Object.TakeoffProfiles.Add(PopulateAndReturnProfile(0, "Takeoff"));
            jsonFileLowerWeight.Object.TakeoffProfiles.Add(PopulateAndReturnProfile(1000, "Takeoff"));
            jsonFileLowerWeight.Object.TakeoffProfiles.Add(PopulateAndReturnProfile(2000, "Takeoff"));
            jsonFileLowerWeight.Object.TakeoffProfiles.Add(PopulateAndReturnProfile(3000, "Takeoff"));

            jsonFileLowerWeight.Object.LandingProfiles.Add(PopulateAndReturnProfile(0, "Landing"));
            jsonFileLowerWeight.Object.LandingProfiles.Add(PopulateAndReturnProfile(1000, "Landing"));
            jsonFileLowerWeight.Object.LandingProfiles.Add(PopulateAndReturnProfile(2000, "Landing"));
            jsonFileLowerWeight.Object.LandingProfiles.Add(PopulateAndReturnProfile(3000, "Landing"));

            jsonFileHigherWeight.Setup(x => x.TakeoffProfiles).Returns(new JsonPerformanceProfileList());
            jsonFileHigherWeight.Setup(x => x.LandingProfiles).Returns(new JsonPerformanceProfileList());

            jsonFileHigherWeight.Object.AircraftWeight = _aircraftHigherWeight;

            jsonFileHigherWeight.Object.TakeoffProfiles.Add(PopulateAndReturnProfile(0, "Takeoff"));
            jsonFileHigherWeight.Object.TakeoffProfiles.Add(PopulateAndReturnProfile(1000, "Takeoff"));
            jsonFileHigherWeight.Object.TakeoffProfiles.Add(PopulateAndReturnProfile(2000, "Takeoff"));
            jsonFileHigherWeight.Object.TakeoffProfiles.Add(PopulateAndReturnProfile(3000, "Takeoff"));

            jsonFileHigherWeight.Object.LandingProfiles.Add(PopulateAndReturnProfile(0, "Landing"));
            jsonFileHigherWeight.Object.LandingProfiles.Add(PopulateAndReturnProfile(1000, "Landing"));
            jsonFileHigherWeight.Object.LandingProfiles.Add(PopulateAndReturnProfile(2000, "Landing"));
            jsonFileHigherWeight.Object.LandingProfiles.Add(PopulateAndReturnProfile(3000, "Landing"));

        }

        public JsonPerformanceProfile PopulateAndReturnProfile(int pressureAltitude, string type)
        {
            JsonPerformanceProfile profile = new JsonPerformanceProfile();
            profile.PressureAltitude = pressureAltitude;
            profile.Type = type;
            profile.GroundRoll = new JsonPerformanceProfileResultList();
            profile.Clear50FtObstacle = new JsonPerformanceProfileResultList();

            for (int i = 1; i < 6; i++)
            {
                JsonPerformanceProfileResult groundRoll = new JsonPerformanceProfileResult();
                groundRoll.Temperature = i * 10;

                // use 1/10th of the pressure altitude to add some upward slope as pressure altitude increasess
                groundRoll.Distance = (i * 100) * (int)(0.1m * pressureAltitude + 1); // +1 to avoid multipling by zero and getting zero
                profile.GroundRoll.Add(groundRoll);

                JsonPerformanceProfileResult clear50Ft = new JsonPerformanceProfileResult();
                clear50Ft.Temperature = i * 10;

                // use 1/10th of the pressure altitude to add some upward slope as pressure altitude increases
                clear50Ft.Distance = (i * 120) * (int)(0.1m * pressureAltitude + 1); // +1 to avoid multipling by zero and getting zero
                profile.Clear50FtObstacle.Add(clear50Ft);

            }

            return profile;
        }

        [TestMethod]
        public void TestJsonFileHasMoreThanOneResult()
        {
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight.Object, jsonFileHigherWeight.Object);

            Assert.IsTrue(bookNumbersList.Count > 0);

        }

        [TestMethod]
        public void TestJsonFileHasMoreThanOneTakeoffResult()
        {
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight.Object, jsonFileHigherWeight.Object);

            Assert.IsTrue(bookNumbersList.FindAll(x => x.Scenario == Scenario.Takeoff).Count > 0);

        }

        [TestMethod]
        public void TestJsonFileHasMoreThanOneLandingResult()
        {
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight.Object, jsonFileHigherWeight.Object);

            Assert.IsTrue(bookNumbersList.FindAll(x => x.Scenario == Scenario.Landing).Count > 0);

        }

        [TestMethod]
        public void TestJsonFileLandingProfilesGroundRollIsWhatWeExpect()
        {
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight.Object, jsonFileHigherWeight.Object);

            double expectedGroundRoll_0 = 100;

            Assert.AreEqual(
                expectedGroundRoll_0,
                (double)bookNumbersList.LandingPerformanceData[0].DistanceGroundRoll.Value.TotalFeet,
                _minimumDistancePrecision
                );

        }

        [TestMethod]
        public void TestJsonFileLandingProfiles50FtIsWhatWeExpect()
        {
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight.Object, jsonFileHigherWeight.Object);

            double expectedDistance = 120;

            Assert.AreEqual(
                expectedDistance,
                (double)bookNumbersList.LandingPerformanceData[0].DistanceToClear50Ft.Value.TotalFeet,
                _minimumDistancePrecision);

        }

        [TestMethod]
        public void TestJsonFileTakeoffProfilesGroundRollIsWhatWeExpect()
        {
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight.Object, jsonFileHigherWeight.Object);

            double expectedGroundRoll_0 = 100;

            Assert.AreEqual(
                expectedGroundRoll_0,
                (double)bookNumbersList.TakeoffPerformanceData[0].DistanceGroundRoll.Value.TotalFeet,
                _minimumDistancePrecision
                );

        }

        [TestMethod]
        public void TestJsonFileTakeoffProfiles50FtIsWhatWeExpect()
        {
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight.Object, jsonFileHigherWeight.Object);

            double expectedDistance = 600;

            Assert.AreEqual(
                expectedDistance,
                (double)bookNumbersList.TakeoffPerformanceData[4].DistanceToClear50Ft.Value.TotalFeet,
                _minimumDistancePrecision
                );
        }

        [TestMethod]
        public void TestBookDistancesSetup()
        {

            decimal temperature = 10;
            decimal pressureAltitude = 1000;
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
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight.Object, jsonFileHigherWeight.Object);

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
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight.Object, jsonFileHigherWeight.Object);

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
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight.Object, jsonFileHigherWeight.Object);

            var takeoffBookDistances = bookNumbersList.TakeoffPerformanceData;
            var landingBookDistances = bookNumbersList.LandingPerformanceData;

            Assert.AreEqual(40, takeoffBookDistances.Count);
            Assert.AreEqual(40, landingBookDistances.Count);
        }

        [TestMethod]
        public void TestFindBookDistancesReturnsCorrectResult()
        {
            Setup();

            var bookNumbersList = new BookPerformanceDataList(_aircraftLowerWeight, _aircraftHigherWeight);

            bookNumbersList.PopulateFromJson(aircraft, jsonFileLowerWeight.Object, jsonFileHigherWeight.Object);

            var takeoffBookDistances = bookNumbersList.TakeoffPerformanceData;
            var landingBookDistances = bookNumbersList.LandingPerformanceData;

            decimal pressureAltitude = 0;
            decimal temperature = 10;
            decimal aircraftWeight = 2900;


            var takeoffBookDistance = bookNumbersList.FindBookDistance(Scenario.Takeoff, pressureAltitude, temperature, aircraftWeight);
            var landingBookDistance = bookNumbersList.FindBookDistance(Scenario.Landing, pressureAltitude, temperature, aircraftWeight);

            Assert.AreEqual(100, takeoffBookDistance.DistanceGroundRoll.Value.TotalFeet);
            Assert.AreEqual(120, takeoffBookDistance.DistanceToClear50Ft.Value.TotalFeet);

            Assert.AreEqual(100, landingBookDistance.DistanceGroundRoll.Value.TotalFeet);
            Assert.AreEqual(120, landingBookDistance.DistanceToClear50Ft.Value.TotalFeet);
        }


    }
}