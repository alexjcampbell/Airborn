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

            Scenario scenario = new Scenario(300, -20, 250, 20);

            scenario.TemperatureCelcius = 12;
            scenario.QNH = 1013;
            scenario.FieldElevation = 1500;

            AircraftPerformanceBase ap = AircraftPerformanceBase.CreateFromJson(scenario, "../../SR22_G2.json");

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

            Scenario scenario = new Scenario(300, -20, 250, 20);

            scenario.TemperatureCelcius = 15;
            scenario.QNH = 1013;
            scenario.FieldElevation = 1500;

            AircraftPerformanceBase ap = AircraftPerformanceBase.CreateFromJson(scenario, "../../SR22_G2.json");

            Assert.AreEqual(0.5, AircraftPerformanceBase.CalculateInterpolationFactor(15,10,20));
            Assert.AreEqual(0, AircraftPerformanceBase.CalculateInterpolationFactor(10,10,20));
            Assert.AreEqual(1, AircraftPerformanceBase.CalculateInterpolationFactor(20,10,20));

        }

        [TestMethod]
        public void TestFindDistanceForPressureAltitudeAndTemperatureFromJson()
        {
            Scenario scenario = new Scenario(300, -20, 250, 20);

            scenario.TemperatureCelcius = 15;
            scenario.QNH = 1013;
            scenario.FieldElevation = 1500;

            AircraftPerformanceBase ap = AircraftPerformanceBase.CreateFromJson(scenario, "../../SR22_G2.json");

            Assert.AreEqual(1092,ap.Profiles.FindByPressureAltitude(ScenarioMode.Takeoff_GroundRoll, 1000).GroundRoll.FindByTemperature(10));
            Assert.AreEqual(1176,ap.Profiles.FindByPressureAltitude(ScenarioMode.Takeoff_GroundRoll, 1000).GroundRoll.FindByTemperature(20));

            Assert.AreEqual(1691,ap.Profiles.FindByPressureAltitude(ScenarioMode.Takeoff_50FtClearance, 1000).Clear50FtObstacle.FindByTemperature(10));
            Assert.AreEqual(1813,ap.Profiles.FindByPressureAltitude(ScenarioMode.Takeoff_50FtClearance, 1000).Clear50FtObstacle.FindByTemperature(20));

        }

        [TestMethod]
        public void Test_GetUpperAndLowerBoundForInterpolation()
        {
            // test it for pressure altitude intervals (1000s)
            Assert.AreEqual((3000, 4000), AircraftPerformanceBase.GetUpperAndLowBoundsForInterpolation(3500, 1000));
            Assert.AreEqual((3000, 4000), AircraftPerformanceBase.GetUpperAndLowBoundsForInterpolation(3000, 1000));
            Assert.AreEqual((3000, 4000), AircraftPerformanceBase.GetUpperAndLowBoundsForInterpolation(3999, 1000));

            Assert.AreEqual((0, 1000), AircraftPerformanceBase.GetUpperAndLowBoundsForInterpolation(500, 1000));
            Assert.AreEqual((0, 1000), AircraftPerformanceBase.GetUpperAndLowBoundsForInterpolation(0, 1000));
            Assert.AreEqual((0, 1000), AircraftPerformanceBase.GetUpperAndLowBoundsForInterpolation(999, 1000));


            // test it for temperature intervals (10s)
            Assert.AreEqual((30, 40), AircraftPerformanceBase.GetUpperAndLowBoundsForInterpolation(35, 10));
            Assert.AreEqual((30, 40), AircraftPerformanceBase.GetUpperAndLowBoundsForInterpolation(30, 10));
            Assert.AreEqual((30, 40), AircraftPerformanceBase.GetUpperAndLowBoundsForInterpolation(39, 10));

            Assert.AreEqual((0, 10), AircraftPerformanceBase.GetUpperAndLowBoundsForInterpolation(0, 10));
            Assert.AreEqual((0, 10), AircraftPerformanceBase.GetUpperAndLowBoundsForInterpolation(5, 10));
            Assert.AreEqual((0, 10), AircraftPerformanceBase.GetUpperAndLowBoundsForInterpolation(9, 10));
        }

        [TestMethod]
        public void Test_Interpolate()
        {
            Assert.AreEqual(1175, AircraftPerformanceBase.Interpolate(1150, 1200, 15, 10));
            Assert.AreEqual(1420, AircraftPerformanceBase.Interpolate(1400, 1500, 1200, 1000));
        }

        [TestMethod]
        public void Test_Takeoff_GroundRollDistance()
        {
            Scenario scenario = new Scenario(300, -20, 250, 0);

            scenario.TemperatureCelcius = 15;
            scenario.QNH = 1013;
            scenario.FieldElevation = 1500;

            AircraftPerformanceBase ap = AircraftPerformanceBase.CreateFromJson(scenario, "../../SR22_G2.json");

            double? result = ap.Takeoff_GroundRoll;

            Assert.AreEqual(1193,result);

        }    

        [TestMethod]
        public void Test_Takeoff_50FtClearanceDistance()
        {
            Scenario scenario = new Scenario(300, -20, 250, 0);

            scenario.TemperatureCelcius = 15;
            scenario.QNH = 1013;
            scenario.FieldElevation = 1500;

            AircraftPerformanceBase ap = AircraftPerformanceBase.CreateFromJson(scenario, "../../SR22_G2.json");

            double? result = ap.Takeoff_50FtClearance;

            Assert.AreEqual(1840,result);

        }      
        
       [TestMethod]
        public void Test_Landing_GroundRollDistance()
        {
            Scenario scenario = new Scenario(300, -20, 250, 0);

            scenario.TemperatureCelcius = 15;
            scenario.QNH = 1013;
            scenario.FieldElevation = 1500;

            AircraftPerformanceBase ap = AircraftPerformanceBase.CreateFromJson(scenario, "../../SR22_G2.json");

            double? result = ap.Landing_GroundRoll;

            Assert.AreEqual(1205,result);

        }       

       [TestMethod]
        public void Test_Landing_50FtClearanceDistance()
        {
            Scenario scenario = new Scenario(300, -20, 250, 0);

            scenario.TemperatureCelcius = 15;
            scenario.QNH = 1013;
            scenario.FieldElevation = 1500;

            AircraftPerformanceBase ap = AircraftPerformanceBase.CreateFromJson(scenario, "../../SR22_G2.json");

            double? result = ap.Landing_50FtClearance;

            Assert.AreEqual(2435,result);

        }      

       [TestMethod]
        public void Test_TakeoffAdjustmentsForHeadwind()
        {
            // 12 knots straight down the runway
            Scenario scenario = new Scenario(300, -20, 300, 12);

            scenario.TemperatureCelcius = 15;
            scenario.QNH = 1013;
            scenario.FieldElevation = 1500;

            AircraftPerformanceBase ap = AircraftPerformanceBase.CreateFromJson(scenario, "../../SR22_G2.json");

            double? result = ap.Takeoff_GroundRoll;

            Assert.AreEqual(1073.7,result);

        }           

       [TestMethod]
        public void Test_TakeoffAdjustmentsForTailwind()
        {
            // 12 knots straight down the runway
            Scenario scenario = new Scenario(300, -20, 300, -2);

            scenario.TemperatureCelcius = 15;
            scenario.QNH = 1013;
            scenario.FieldElevation = 1500;

            AircraftPerformanceBase ap = AircraftPerformanceBase.CreateFromJson(scenario, "../../SR22_G2.json");

            double? result = ap.Takeoff_GroundRoll;

            Assert.AreEqual(1312.3000000000002,result);

        }              


       [TestMethod]
        public void Test_LandingAdjustmentsForHeadwind()
        {
            // 12 knots straight down the runway
            Scenario scenario = new Scenario(300, -20, 300, 13);

            scenario.TemperatureCelcius = 15;
            scenario.QNH = 1013;
            scenario.FieldElevation = 1500;

            AircraftPerformanceBase ap = AircraftPerformanceBase.CreateFromJson(scenario, "../../SR22_G2.json");

            double? result = ap.Landing_GroundRoll;

            Assert.AreEqual(1084.5,result);

        }           

       [TestMethod]
        public void Test_LandingAdjustmentsForTailwind()
        {
            // 12 knots straight down the runway
            Scenario scenario = new Scenario(300, -20, 300, -2);

            scenario.TemperatureCelcius = 15;
            scenario.QNH = 1013;
            scenario.FieldElevation = 1500;

            AircraftPerformanceBase ap = AircraftPerformanceBase.CreateFromJson(scenario, "../../SR22_G2.json");

            double? result = ap.Landing_GroundRoll;

            Assert.AreEqual(1325.5,result);

        }              

    }
}