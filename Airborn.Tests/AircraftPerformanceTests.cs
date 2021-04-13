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
        public void Test_CalculateInterpolationFactor()
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

        [TestMethod]
        public void Test_GetUpperAndLowerBoundForInterpolation()
        {
            // test it for pressure altitude intervals (1000s)
            Assert.AreEqual((3000, 4000), AircraftPerformance.GetUpperAndLowBoundsForInterpolation(3500, 1000));
            Assert.AreEqual((3000, 4000), AircraftPerformance.GetUpperAndLowBoundsForInterpolation(3000, 1000));
            Assert.AreEqual((3000, 4000), AircraftPerformance.GetUpperAndLowBoundsForInterpolation(3999, 1000));

            Assert.AreEqual((0, 1000), AircraftPerformance.GetUpperAndLowBoundsForInterpolation(500, 1000));
            Assert.AreEqual((0, 1000), AircraftPerformance.GetUpperAndLowBoundsForInterpolation(0, 1000));
            Assert.AreEqual((0, 1000), AircraftPerformance.GetUpperAndLowBoundsForInterpolation(999, 1000));


            // test it for temperature intervals (10s)
            Assert.AreEqual((30, 40), AircraftPerformance.GetUpperAndLowBoundsForInterpolation(35, 10));
            Assert.AreEqual((30, 40), AircraftPerformance.GetUpperAndLowBoundsForInterpolation(30, 10));
            Assert.AreEqual((30, 40), AircraftPerformance.GetUpperAndLowBoundsForInterpolation(39, 10));

            Assert.AreEqual((0, 10), AircraftPerformance.GetUpperAndLowBoundsForInterpolation(0, 10));
            Assert.AreEqual((0, 10), AircraftPerformance.GetUpperAndLowBoundsForInterpolation(5, 10));
            Assert.AreEqual((0, 10), AircraftPerformance.GetUpperAndLowBoundsForInterpolation(9, 10));
        }

        [TestMethod]
        public void Test_FindTakeoffDistance()
        {

            AircraftPerformance ap = AircraftPerformance.CreateFromJson();

            Scenario scenario = new Scenario(300, -20, 250, 20);

            scenario.TemperatureCelcius = 15;
            scenario.PressureAltitude = 1500;

            int takeoffGroundRoll = ap.CalculateTakeoffDistanceGroundRoll(scenario);

            Assert.AreEqual(1193,takeoffGroundRoll);

        }

    }
}