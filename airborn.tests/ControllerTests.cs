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
using Airborn.web.Controllers;

namespace Airborn.Tests
{

    // TODO: these are mostly testing the model, not the controller,
    // so we should move them out to CalculagePageModelTests.cs
    // although the dependency injection we get for free from using the controller
    // for DbContext is quite useful here

    [TestClass]
    public class ControllerTests
    {

        private ILogger<PlanFlightController> _doesntDoMuch =
            new Microsoft.Extensions.Logging.Abstractions.NullLogger<PlanFlightController>();


        PlanFlightController _controller;
        CalculatePageModel _model;

        internal static PlanFlightController InitializeController(ILogger<PlanFlightController> _doesntDoMuch)
        {
            var mockEnvironment = new Mock<IWebHostEnvironment>();

            mockEnvironment
                .Setup(m => m.EnvironmentName)
                .Returns("Hosting:UnitTestEnvironment")
                ;

            AirbornDbContext dbContext = UtilitiesForTesting.GetMockAirportDbContextForTesting();

            return new PlanFlightController(_doesntDoMuch, mockEnvironment.Object, dbContext);
        }


        internal static CalculatePageModel InitializeModel()
        {
            CalculatePageModel model = new CalculatePageModel();

            model.AircraftType = UtilitiesForTesting.Default_AircraftType.ToString();
            model.AltimeterSetting = UtilitiesForTesting.Default_QNH;
            model.AltimeterSettingType = UtilitiesForTesting.Default_AltimeterSettingType;
            model.AirportIdentifier = UtilitiesForTesting.Default_AirportIdentifier;
            model.Temperature = UtilitiesForTesting.Default_Temperature;
            model.TemperatureType = UtilitiesForTesting.Default_TemperatureType;
            model.WindDirectionMagnetic = UtilitiesForTesting.Default_WindDirectionMagnetic;
            model.WindStrength = UtilitiesForTesting.Default_WindStrength;
            model.AircraftWeight = (int)UtilitiesForTesting.Default_AircraftWeight;
            model.RootPath = "../../Debug/net7.0/SR22_G2_3400.json";

            return model;
        }

        [TestInitialize]
        public void Initialize()
        {
            _controller = ControllerTests.InitializeController(_doesntDoMuch);
            _model = ControllerTests.InitializeModel();
        }

        [TestMethod]
        public void Test_ReturnResults_EvenWhenTemperatureIsNegative()
        {

            _model.Temperature = -10;
            _model.TemperatureType = TemperatureType.C;

            var result = _controller.Calculate(_model);

            // the POH doesn't provide performance information for negative temperatures - so, in the
            // PerformanceCalcator we treat negative temperatures as zero. We test if that this working
            // by checking to see that there are performance results being returned (if our override of
            // negative temperatures to zero wasn't working, it would return no results because it
            // wouldn't find any in the JSON for like -10C temperatures)
            Assert.IsTrue(_model.Results.Count > 0);

            Assert.IsTrue(_model.Notes.Find(p => p.Contains("Temperature is negative")) != null);
        }

        [TestMethod]
        public void Test_TakeoffGroundRollDistance_IsGreaterWhenAircraftWeightIncreases()
        {

            _model.AircraftWeight = 2900;

            var result = _controller.Calculate(_model);

            double? takeoffGroundRollDistanceLowerWeight = _model.Results[0].Takeoff_GroundRoll;

            _model = InitializeModel();

            _model.AircraftWeight = 3400;

            result = _controller.Calculate(_model);
            double? takeoffGroundRollDistanceHigherWeight = _model.Results[0].Takeoff_GroundRoll;

            Assert.IsTrue(takeoffGroundRollDistanceHigherWeight > takeoffGroundRollDistanceLowerWeight);
        }

        [TestMethod]
        public void Test_LandingGroundRollDistance_IsSameWhenAircraftWeightChanges()
        {
            // Set the aircraft weight to the lower weight
            _model.AircraftWeight = 2900;

            // Calculate the takeoff ground roll distance
            var result = _controller.Calculate(_model);

            // Get the takeoff ground roll distance at the lower weight
            double? landingGroundRollDistanceLowerWeight = _model.Results[0].Landing_GroundRoll;

            // Reinitialize the model
            InitializeModel();

            // Set the aircraft weight to the higher weight
            _model.AircraftWeight = 3400;

            // Calculate the takeoff ground roll distance
            result = _controller.Calculate(_model);

            // Get the takeoff ground roll distance at the higher weight
            double? landingGroundRollDistanceHigherWeight = _model.Results[0].Landing_GroundRoll;

            // Assert that the takeoff ground roll distance at the higher weight is equal to the takeoff ground roll distance at the lower weight
            Assert.IsTrue(landingGroundRollDistanceHigherWeight == landingGroundRollDistanceLowerWeight);
        }


    }
}