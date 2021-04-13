using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class AircraftPerformanceTests
    {

        [TestMethod]
        public void TestJsonRead()
        {

            AircraftPerformance ap = AircraftPerformance.CreateFromJson();

            // test that the Json de-serializer has read at least one profile
            Assert.IsTrue(ap.Profiles.Count > 0);

            // test that the Json de-serializer has read the performance data 
            // for at least one profile
            Assert.IsTrue(ap.Profiles[0].GroundRoll.Count > 0);

            // test that the pressureAltitude and ground roll distance match our
            // sample file
            Assert.AreEqual(0, ap.Profiles[0].PressureAltitude = 0);
            Assert.AreEqual(917, ap.Profiles[0].GroundRoll[0].Distance);
            Assert.AreEqual(1000, ap.Profiles[1].PressureAltitude = 1000);
            Assert.AreEqual(1011, ap.Profiles[1].GroundRoll[0].Distance);
        }

        [TestMethod]
        public void TestInterpolate()
        {

            AircraftPerformance ap = AircraftPerformance.CreateFromJson();

            Assert.AreEqual(0.5, AircraftPerformance.CalculateInterpolationFactor(15,10,20));
            Assert.AreEqual(0, AircraftPerformance.CalculateInterpolationFactor(10,10,20));
            Assert.AreEqual(1, AircraftPerformance.CalculateInterpolationFactor(20,10,20));

        }

        [TestMethod]
        public void TestFindDistanceForPressureAltitudeAndTemperatureFromJson()
        {

            AircraftPerformance ap = AircraftPerformance.CreateFromJson();

            Assert.AreEqual(1092,ap.Profiles.FindByPressureAltitude(1000).GroundRoll.FindByTemperature(10));
            Assert.AreEqual(1176,ap.Profiles.FindByPressureAltitude(1000).GroundRoll.FindByTemperature(20));

            Assert.AreEqual(1691,ap.Profiles.FindByPressureAltitude(1000).Clear50FtObstacle.FindByTemperature(10));
            Assert.AreEqual(1813,ap.Profiles.FindByPressureAltitude(1000).Clear50FtObstacle.FindByTemperature(20));

        }

    }
}