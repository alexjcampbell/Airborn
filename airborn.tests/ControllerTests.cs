using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Airborn.web.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.ComponentModel.DataAnnotations;
using Airborn.Controllers;

namespace Airborn.Tests
{

    // TODO: these are mostly testing the model, not the controller,
    // so we should move them out to CalculagePageModelTests.cs
    // although the dependency injection we get for free from using the controller
    // for DbContext is quite useful here

    [TestClass]
    public class ControllerTests
    {

        private ILogger<HomeController> _doesntDoMuch =
            new Microsoft.Extensions.Logging.Abstractions.NullLogger<HomeController>();


        HomeController _controller;
        CalculatePageModel _model;

        private void InitializeController()
        {
            var mockEnvironment = new Mock<IWebHostEnvironment>();

            mockEnvironment
                .Setup(m => m.EnvironmentName)
                .Returns("Hosting:UnitTestEnvironment")
                ;

            AirportDbContext dbContext = UtilitiesForTesting.GetMockAirportDbContextForTesting();

            _controller = new HomeController(_doesntDoMuch, mockEnvironment.Object, dbContext);
        }


        private void InitializeModel()
        {
            _model = new CalculatePageModel();

            _model.AircraftType = UtilitiesForTesting.Default_AircraftType;
            _model.AltimeterSetting = UtilitiesForTesting.Default_QNH;
            _model.AltimeterSettingType = UtilitiesForTesting.Default_AltimeterSettingType;
            _model.AirportIdentifier = UtilitiesForTesting.Default_AirportIdentifier;
            _model.Temperature = UtilitiesForTesting.Default_Temperature;
            _model.TemperatureType = UtilitiesForTesting.Default_TemperatureType;
            _model.WindDirectionMagnetic = UtilitiesForTesting.Default_WindDirectionMagnetic;
            _model.WindStrength = UtilitiesForTesting.Default_WindStrength;
            _model.AircraftWeight = (int)UtilitiesForTesting.Default_AircraftWeight;
            _model.RootPath = "../../Debug/net7.0/SR22_G2_3400.json";
        }

        [TestMethod]
        public void TestDirectHeadwind_HasNoCrosswindComponent()
        {
            InitializeController();
            InitializeModel();

            var result = _controller.Calculate(_model);

            _model.WindDirectionMagnetic = 220;
            int expectedCrosswind = 0;

            Assert.AreEqual(_model.Results[0].CrosswindComponent, expectedCrosswind);
        }

        [TestMethod]
        public void Test_DirectTailwind_HasNoCrosswindComponent()
        {
            InitializeController();
            InitializeModel();

            _model.WindDirectionMagnetic = 40;
            int expectedCrosswind = 0;

            var result = _controller.Calculate(_model);

            Assert.AreEqual(expectedCrosswind, Math.Round(_model.Results[0].CrosswindComponent.Value));
        }

        [TestMethod]
        public void Test_CrosswindComponent_IsCorrect()
        {
            InitializeController();
            InitializeModel();

            _model.WindDirectionMagnetic = 90;
            decimal expectedCrosswind = -7.6604444311898m;

            var result = _controller.Calculate(_model);

            Assert.AreEqual(expectedCrosswind, _model.Results[0].CrosswindComponent);
        }

        [TestMethod]
        public void Test_CrosswindComponent_IsCorrectForOppositeWind()
        {
            InitializeController();
            InitializeModel();

            _model.WindDirectionMagnetic = 270;
            decimal expectedCrosswind = 7.66044443118978m;

            var result = _controller.Calculate(_model);

            Assert.AreEqual(expectedCrosswind, _model.Results[0].CrosswindComponent);
        }

        [TestMethod]
        public void Test_PressureAltitude_EqualsFieldAltitudeWhenStandardPressureMb()
        {
            InitializeController();
            InitializeModel();

            _model.AltimeterSetting = 1013.25m;
            _model.AltimeterSettingType = AltimeterSettingType.MB;

            var result = _controller.Calculate(_model);

            // altimeter setting of 1013.25m is standard pressure at sea level,
            // so field elevation should equal pressure altitude
            // but due to rounding errors, pressure altitude doesn't quite equal zero ft (sea level)
            double expectedPressureAltitude = 6.825f;
            Assert.AreEqual(expectedPressureAltitude, (double)_model.PressureAltitude.Value.TotalFeet, 0.1f);
        }

        [TestMethod]
        public void Test_PressureAltitude_EqualsFieldAltitudeWhenStandardPressureHg()
        {
            InitializeController();
            InitializeModel();

            _model.AltimeterSetting = 29.92m;
            _model.AltimeterSettingType = AltimeterSettingType.HG;

            var result = _controller.Calculate(_model);

            double expectedPressureAltitude = 0;

            // Altimeter setting of 29.92 is standard pressure at sea level, so field elevation
            // should equal pressure altitude. However, rounding errors mean pressure altitude doesn't 
            // quite equal zero ft (sea level) as you'd expect
            Assert.AreEqual(
                expectedPressureAltitude,
                (double)_model.PressureAltitude.Value.TotalFeet,
                0.1f);
        }

        [TestMethod]
        public void Test_PressureAltitude_IsCorrectWhenAltimeterSettingIsLowerThanStandard()
        {
            InitializeController();
            InitializeModel();

            _model.AltimeterSetting = 29.82m;
            _model.AltimeterSettingType = AltimeterSettingType.HG;

            var result = _controller.Calculate(_model);

            // lower pressure means higher altitude, at a rate of ~90 feet per inch of mercury
            decimal expectedPressureAltitude = UtilitiesForTesting.Default_FieldElevation + 92.4522894637524m;

            Assert.AreEqual(expectedPressureAltitude, _model.PressureAltitude.Value.TotalFeet);
        }

        [TestMethod]
        public void Test_PressureAltitude_IsCorrectWhenAltimeterSettingIsHigherThanStandard()
        {
            InitializeController();
            InitializeModel();

            _model.AltimeterSetting = 30.02m;
            _model.AltimeterSettingType = AltimeterSettingType.HG;

            var result = _controller.Calculate(_model);

            // higher pressure means lower altitude, at a rate of ~90 feet per inch of mercury
            decimal expectedPressureAltitude = UtilitiesForTesting.Default_FieldElevation - 92.4522894130836m;

            Assert.AreEqual(expectedPressureAltitude, _model.PressureAltitude.Value.TotalFeet);
        }

        [TestMethod]
        public void Test_ReturnResults_EvenWhenPressureAltitudeIsNegative()
        {
            InitializeController();
            InitializeModel();

            _model.AltimeterSetting = 1040;
            _model.AltimeterSettingType = AltimeterSettingType.MB;

            var result = _controller.Calculate(_model);

            Assert.IsTrue(_model.PressureAltitude.Value.TotalFeet < 0);

            Assert.IsTrue(_model.PressureAltitudeAlwaysPositiveOrZero == 0);

            // 1040 hPa will result in a negative pressure altitude, but the POH doesn't provide performance
            // information for negative pressure altitudes - so, in the PerformanceCalcator we treat negative
            // pressure altitudes as zero. We test if that this working by checking to see that there are 
            // performance results being returned (if our override of negative pressure altitudes to zero
            // wasn't working, it would return no results because it wouldn't find any in the JSON for like -
            // 400 ft pressure altitudes)
            Assert.IsTrue(_model.Results.Count > 0);

            Assert.IsTrue(_model.Notes.Find(p => p.Contains("Pressure altitude is negative")) != null);
        }

        [TestMethod]
        public void Test_PressureAltitude_ShouldOnlyOverrideToZeroWhenPressureAltitudeIsNegative()
        {
            InitializeController();
            InitializeModel();

            _model.AltimeterSetting = 1000;
            _model.AltimeterSettingType = AltimeterSettingType.MB;

            decimal expectedPressureAltitude = 361.725m;

            var result = _controller.Calculate(_model);

            Assert.IsTrue(_model.PressureAltitude.Value.TotalFeet > 0);

            Assert.AreEqual(expectedPressureAltitude, _model.PressureAltitudeAlwaysPositiveOrZero);
        }

        [TestMethod]
        public void Test_ReturnResults_EvenWhenTemperatureIsNegative()
        {
            InitializeController();
            InitializeModel();

            _model.Temperature = -10;
            _model.TemperatureType = TemperatureType.C;

            var result = _controller.Calculate(_model);

            // 1040 hPa will result in a negative pressure altitude, but the POH doesn't provide performance
            // information for negative pressure altitudes - so, in the PerformanceCalcator we treat negative
            // pressure altitudes as zero. We test if that this working by checking to see that there are 
            // performance results being returned (if our override of negative pressure altitudes to zero
            // wasn't working, it would return no results because it wouldn't find any in the JSON for like -
            // 400 ft pressure altitudes)
            Assert.IsTrue(_model.Results.Count > 0);

            Assert.IsTrue(_model.Notes.Find(p => p.Contains("Temperature is negative")) != null);
        }

        [TestMethod]
        public void Test_RunwaysWithMostHeadwind_AreFirst()
        {
            InitializeController();
            InitializeModel();

            _model.WindDirectionMagnetic = 220;

            var result = _controller.Calculate(_model);

            int expectedRunwayHeading = 220;

            Assert.AreEqual(expectedRunwayHeading, _model.ResultsSortedByHeadwind[0].Runway.RunwayHeading.DirectionMagnetic);

            // we need to reset the model because the controller modifies it in the previous pass
            InitializeModel();

            _model.WindDirectionMagnetic = 90;
            expectedRunwayHeading = 90;

            result = _controller.Calculate(_model);

            Assert.AreEqual(expectedRunwayHeading, _model.ResultsSortedByHeadwind[0].Runway.RunwayHeading.DirectionMagnetic);


            // we need to reset the model again because the controller modifies it in the previous pass
            InitializeModel();

            // this is weird, but 360 degrees also = 0 degrees
            _model.WindDirectionMagnetic = 360;
            expectedRunwayHeading = 0;

            result = _controller.Calculate(_model);

            Assert.AreEqual(expectedRunwayHeading, _model.ResultsSortedByHeadwind[0].Runway.RunwayHeading.DirectionMagnetic);

        }

        [TestMethod]
        public void Test_RunwaysWithMostTailwind_AreLast()
        {
            InitializeController();
            InitializeModel();

            _model.WindDirectionMagnetic = 220;

            var result = _controller.Calculate(_model);

            // with a 220 degree wind, runway 4 should have the most tailwind
            // yes, it's expected runway heading is zero
            int expectedRunwayHeading = 40;

            Assert.AreEqual(expectedRunwayHeading, _model.ResultsSortedByHeadwind[_model.Results.Count - 1].Runway.RunwayHeading.DirectionMagnetic);
        }

        [TestMethod]
        public void Test_RunwaysWithMostCrosswind_AreInMiddle()
        {
            InitializeController();
            InitializeModel();

            _model.WindDirectionMagnetic = 220;

            var result = _controller.Calculate(_model);

            // with a 220 degree wind, runway 36 should have the most tailwind
            // yes, it's expected runway heading is zero
            int expectedRunwayHeading = 180;

            Assert.AreEqual(expectedRunwayHeading, _model.ResultsSortedByHeadwind[2].Runway.RunwayHeading.DirectionMagnetic);
        }


        [TestMethod]
        public void Test_IsBestWindTrueWhenRunway_IsMostAlignedWithWind()
        {
            InitializeController();
            InitializeModel();

            var result = _controller.Calculate(_model);

            Assert.IsTrue(_model.ResultsSortedByHeadwind.First().IsBestWind);
        }

        [TestMethod]
        public void Test_IsBestWindFalseWhenRunway_IsNotMostAlignedWithWind()
        {
            InitializeController();
            InitializeModel();

            var result = _controller.Calculate(_model);

            Assert.IsFalse(_model.ResultsSortedByHeadwind.Last().IsBestWind);
        }

        [TestMethod]
        public void Test_TakeoffGroundRollDistance_IsGreaterWhenAircraftWeightIncreases()
        {
            InitializeController();
            InitializeModel();

            _model.AircraftWeight = 2900;

            var result = _controller.Calculate(_model);

            decimal? takeoffGroundRollDistanceLowerWeight = _model.Results[0].Takeoff_GroundRoll;

            InitializeModel();

            _model.AircraftWeight = 3400;

            result = _controller.Calculate(_model);
            decimal? takeoffGroundRollDistanceHigherWeight = _model.Results[0].Takeoff_GroundRoll;

            Assert.IsTrue(takeoffGroundRollDistanceHigherWeight > takeoffGroundRollDistanceLowerWeight);
        }

        [TestMethod]
        public void Test_LandingGroundRollDistance_IsSameWhenAircraftWeightChanges()
        {
            // Initialize the controller and model
            InitializeController();
            InitializeModel();

            // Set the aircraft weight to the lower weight
            _model.AircraftWeight = 2900;

            // Calculate the takeoff ground roll distance
            var result = _controller.Calculate(_model);

            // Get the takeoff ground roll distance at the lower weight
            decimal? landingGroundRollDistanceLowerWeight = _model.Results[0].Landing_GroundRoll;

            // Reinitialize the model
            InitializeModel();

            // Set the aircraft weight to the higher weight
            _model.AircraftWeight = 3400;

            // Calculate the takeoff ground roll distance
            result = _controller.Calculate(_model);

            // Get the takeoff ground roll distance at the higher weight
            decimal? landingGroundRollDistanceHigherWeight = _model.Results[0].Landing_GroundRoll;

            // Assert that the takeoff ground roll distance at the higher weight is equal to the takeoff ground roll distance at the lower weight
            Assert.IsTrue(landingGroundRollDistanceHigherWeight == landingGroundRollDistanceLowerWeight);
        }


    }
}