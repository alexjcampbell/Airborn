using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;
using System.Collections.Generic;
using Moq;

namespace Airborn.Tests
{
    [TestClass]
    public class BookDistancesTests
    {

        Mock<JsonFile> jsonFile = new Mock<JsonFile>();

        public void Setup()
        {
            jsonFile.Setup(x => x.TakeoffProfiles).Returns(new JsonPerformanceProfileList());
            jsonFile.Setup(x => x.LandingProfiles).Returns(new JsonPerformanceProfileList());


            jsonFile.Object.TakeoffProfiles.Add(PopulateAndReturnProfile(0, "Takeoff"));
            jsonFile.Object.TakeoffProfiles.Add(PopulateAndReturnProfile(1000, "Takeoff"));
            jsonFile.Object.TakeoffProfiles.Add(PopulateAndReturnProfile(2000, "Takeoff"));
            jsonFile.Object.TakeoffProfiles.Add(PopulateAndReturnProfile(3000, "Takeoff"));

            jsonFile.Object.LandingProfiles.Add(PopulateAndReturnProfile(0, "Landing"));
            jsonFile.Object.LandingProfiles.Add(PopulateAndReturnProfile(1000, "Landing"));
            jsonFile.Object.LandingProfiles.Add(PopulateAndReturnProfile(2000, "Landing"));
            jsonFile.Object.LandingProfiles.Add(PopulateAndReturnProfile(3000, "Landing"));

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

            var bookNumbersList = new BookDistancesList();

            bookNumbersList.PopulateFromJson(jsonFile.Object);

            Assert.IsTrue(bookNumbersList.Count > 0);

        }

        [TestMethod]
        public void TestJsonFileHasMoreThanOneTakeoffResult()
        {
            Setup();

            var bookNumbersList = new BookDistancesList();

            bookNumbersList.PopulateFromJson(jsonFile.Object);

            Assert.IsTrue(bookNumbersList.FindAll(x => x.Scenario == Scenario.Takeoff).Count > 0);

        }

        [TestMethod]
        public void TestJsonFileHasMoreThanOneLandingResult()
        {
            Setup();

            var bookNumbersList = new BookDistancesList();

            bookNumbersList.PopulateFromJson(jsonFile.Object);

            Assert.IsTrue(bookNumbersList.FindAll(x => x.Scenario == Scenario.Landing).Count > 0);

        }

        [TestMethod]
        public void TestJsonFileLandingProfilesGroundRollIsWhatWeExpect()
        {
            Setup();

            var bookNumbersList = new BookDistancesList();

            bookNumbersList.PopulateFromJson(jsonFile.Object);

            decimal expectedGroundRoll_0 = 100;

            Assert.AreEqual(expectedGroundRoll_0,
                bookNumbersList.LandingBookDistances[0].GroundRoll);

        }

        [TestMethod]
        public void TestJsonFileLandingProfiles50FtIsWhatWeExpect()
        {
            Setup();

            var bookNumbersList = new BookDistancesList();

            bookNumbersList.PopulateFromJson(jsonFile.Object);

            decimal expectedGroundRoll_0 = 120;

            Assert.AreEqual(expectedGroundRoll_0,
                bookNumbersList.LandingBookDistances[0].DistanceToClear50Ft);

        }

        [TestMethod]
        public void TestJsonFileTakeoffProfilesGroundRollIsWhatWeExpect()
        {
            Setup();

            var bookNumbersList = new BookDistancesList();

            bookNumbersList.PopulateFromJson(jsonFile.Object);

            decimal expectedGroundRoll_0 = 100;

            Assert.AreEqual(expectedGroundRoll_0,
                bookNumbersList.TakeoffBookDistances[0].GroundRoll);

        }

        [TestMethod]
        public void TestJsonFileTakeoffProfiles50FtIsWhatWeExpect()
        {
            Setup();

            var bookNumbersList = new BookDistancesList();

            bookNumbersList.PopulateFromJson(jsonFile.Object);

            decimal expectedGroundRoll_0 = 600;

            Assert.AreEqual(expectedGroundRoll_0,
                bookNumbersList.TakeoffBookDistances[4].DistanceToClear50Ft);

        }

        [TestMethod]
        public void TestBookDistancesSetup()
        {

            decimal temperature = 10;
            decimal pressureAltitude = 1000;
            decimal groundRoll = 100;
            decimal distanceToClear50Ft = 120;
            Scenario scenario = Scenario.Takeoff;

            BookDistances bookDistances = new BookDistances(scenario, pressureAltitude, temperature, groundRoll, distanceToClear50Ft);

            Assert.AreEqual(temperature, bookDistances.Temperature);
            Assert.AreEqual(pressureAltitude, bookDistances.PressureAltitude);
            Assert.AreEqual(groundRoll, bookDistances.GroundRoll);
            Assert.AreEqual(distanceToClear50Ft, bookDistances.DistanceToClear50Ft);
            Assert.AreEqual(scenario, bookDistances.Scenario);
        }

        [TestMethod]
        public void TestGetTakeoffBookDistancesOnlyReturnsTakeoffResults()
        {
            Setup();

            var bookNumbersList = new BookDistancesList();

            bookNumbersList.PopulateFromJson(jsonFile.Object);

            var takeoffBookDistances = bookNumbersList.TakeoffBookDistances;

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

            var bookNumbersList = new BookDistancesList();

            bookNumbersList.PopulateFromJson(jsonFile.Object);

            var landingBookDistances = bookNumbersList.LandingBookDistances;

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

            var bookNumbersList = new BookDistancesList();

            bookNumbersList.PopulateFromJson(jsonFile.Object);

            var takeoffBookDistances = bookNumbersList.TakeoffBookDistances;
            var landingBookDistances = bookNumbersList.LandingBookDistances;

            Assert.AreEqual(20, takeoffBookDistances.Count);
            Assert.AreEqual(20, landingBookDistances.Count);
        }

        [TestMethod]
        public void TestFindBookDistancesReturnsCorrectResult()
        {
            Setup();

            var bookNumbersList = new BookDistancesList();

            bookNumbersList.PopulateFromJson(jsonFile.Object);

            var takeoffBookDistances = bookNumbersList.TakeoffBookDistances;
            var landingBookDistances = bookNumbersList.LandingBookDistances;

            var takeoffBookDistance = bookNumbersList.FindBookDistance(Scenario.Takeoff, 0, 10);
            var landingBookDistance = bookNumbersList.FindBookDistance(Scenario.Landing, 0, 10);

            Assert.AreEqual(100, takeoffBookDistance.GroundRoll);
            Assert.AreEqual(120, takeoffBookDistance.DistanceToClear50Ft);

            Assert.AreEqual(100, landingBookDistance.GroundRoll);
            Assert.AreEqual(120, landingBookDistance.DistanceToClear50Ft);
        }

        [TestMethod]
        public void TestGetInterpolatedBookDistancesReturnsInterpolatedResult()
        {
            Setup();

            var bookDistancesList = new BookDistancesList();

            //bookDistancesList.GetInterpolateBookDistances(Scenario.Takeoff, 0, 10, 100, 120);

            Assert.AreEqual(true, false);
        }

    }
}