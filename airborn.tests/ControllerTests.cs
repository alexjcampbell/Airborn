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
    [TestClass]
    public class ControllerTests
    {
        private ILogger<HomeController> _doesntDoMuch =
            new Microsoft.Extensions.Logging.Abstractions.NullLogger<HomeController>();

        // Default values for the runway
        const string _default_airport_Ident = "KJFK";
        const int _default_Runway_Id = 1;
        const string _default_RunwayWidth = "150";
        const string _default_RunwayLength = "10000";
        const string _default_Runway_Name = "22R";

        const int _default_Runway_Heading = 220;

        // Default values for the airport
        const int _default_FieldElevation = 0;

        // Default values for the model
        const AircraftType _default_AircraftType = AircraftType.C172_SP;
        const decimal _default_AltimeterSetting = 29.92m;
        const AltimeterSettingType _default_AltimeterSettingType = AltimeterSettingType.HG;
        const string _default_AirportIdentifier = "KJFK";
        const int _default_Temperature = 15;
        const TemperatureType _default_TemperatureType = TemperatureType.C;
        const int _default_WindDirectionMagnetic = 220;
        const int _default_WindStrength = 10;
        const int _default_TemperatureCelcius = 12;
        const int _default_QNH = 1013;
        const double _default_AircraftWeight = 2500;

        List<Runway> TestRunways = new List<Runway>();
        List<Airport> TestAirports = new List<Airport>();


        private HomeController InitializeAndGetController()
        {
            var mockEnvironment = new Mock<IWebHostEnvironment>();

            mockEnvironment
                .Setup(m => m.EnvironmentName)
                .Returns("Hosting:UnitTestEnvironment")
                ;

            var airportDbContext = new Mock<AirportDbContext>();


            var runwayDbSet = new Mock<DbSet<Runway>>();
            airportDbContext.Setup(o => o.Runways).Returns(() => runwayDbSet.Object);
            
            SetupTestRunway(_default_Runway_Heading, _default_Runway_Id, _default_Runway_Name);

            // Set up the DbSet as an IQueryable so it can be enumerated.
            var queryableRunways = TestRunways.AsQueryable();
            runwayDbSet.As<IQueryable<Runway>>().Setup(m => m.Provider).Returns(queryableRunways.Provider);
            runwayDbSet.As<IQueryable<Runway>>().Setup(m => m.Expression).Returns(queryableRunways.Expression);
            runwayDbSet.As<IQueryable<Runway>>().Setup(m => m.ElementType).Returns(queryableRunways.ElementType);
            runwayDbSet.As<IQueryable<Runway>>().Setup(m => m.GetEnumerator()).Returns(() => queryableRunways.GetEnumerator());

            var airportDbSet = new Mock<DbSet<Airport>>();
            airportDbContext.Setup(o => o.Airports).Returns(() => airportDbSet.Object);

            var testAirports = new List<Airport>(){
                new Airport()
            };

            testAirports[0].Ident = _default_airport_Ident;
            testAirports[0].FieldElevation = _default_FieldElevation;

            // Set up the DbSet as an IQueryable so it can be enumerated.
            var queryableAirports = testAirports.AsQueryable();
            airportDbSet.As<IQueryable<Airport>>().Setup(m => m.Provider).Returns(queryableAirports.Provider);
            airportDbSet.As<IQueryable<Airport>>().Setup(m => m.Expression).Returns(queryableAirports.Expression);
            airportDbSet.As<IQueryable<Airport>>().Setup(m => m.ElementType).Returns(queryableAirports.ElementType);
            airportDbSet.As<IQueryable<Airport>>().Setup(m => m.GetEnumerator()).Returns(() => queryableAirports.GetEnumerator());


            var controller = new HomeController(_doesntDoMuch, mockEnvironment.Object, airportDbContext.Object);

            return controller;
        }

        private void SetupTestRunway(int runwayId, int runwayHeading, string runwayName)
        {
            Runway runway = new Runway(Direction.FromMagnetic(runwayHeading, 0));

            runway.Airport_Ident = _default_airport_Ident;
            runway.Runway_Id = runwayId;
            runway.RunwayWidth = _default_RunwayWidth;
            runway.RunwayLength = _default_RunwayLength;
            runway.Runway_Name = runwayName;

            TestRunways.Add(runway);
            
        }

        private CalculatePageModel InitializeAndGetModel()
        {
            CalculatePageModel model = new CalculatePageModel();

            model.AircraftType = _default_AircraftType;
            model.AltimeterSetting = _default_AltimeterSetting;
            model.AltimeterSettingType = _default_AltimeterSettingType;
            model.AirportIdentifier = _default_AirportIdentifier;
            model.Temperature = _default_Temperature;
            model.TemperatureType = _default_TemperatureType;
            model.WindDirectionMagnetic = _default_WindDirectionMagnetic;
            model.WindStrength = _default_WindStrength;
            model.AircraftWeight = _default_AircraftWeight;
            model.RootPath = AircraftPerformanceTests.TestJsonPath;

            return model;
        }

        [TestMethod]
        public void TestDirectHeadwindHasNoCrosswindComponent()
        {
            HomeController controller = InitializeAndGetController();
            CalculatePageModel model = InitializeAndGetModel();

            var result = controller.Calculate(model);

            Assert.AreEqual(model.Results[0].CrosswindComponent, 0);
        }

        [TestMethod]
        public void TestPressureAltitudeEqualsFieldAltitudeWhenStandardPressure()
        {
            HomeController controller = InitializeAndGetController();
            CalculatePageModel model = InitializeAndGetModel();

            model.AltimeterSetting = 1013.25m;
            model.AltimeterSettingType = AltimeterSettingType.MB;

            var result = controller.Calculate(model);

            // altimeter setting of 1013.25 is standard pressure at sea level,
            // so field elevation should equal pressure altitude
            Assert.AreEqual(_default_FieldElevation, model.PressureAltitude);
        }

        // test best runway for wind
        [TestMethod]
        public void TestRunwaysMostIntoWindAreFirst()
        {
            HomeController controller = InitializeAndGetController();
            CalculatePageModel model = InitializeAndGetModel();

            model.WindDirectionMagnetic = 220;

            var result = controller.Calculate(model);

            // 22R will already be in there from the default setup
            SetupTestRunway(2, 230, "23L");
            SetupTestRunway(3, 160, "16");
            SetupTestRunway(4, 360, "36");
            SetupTestRunway(5, 90, "9L");

            int expectedRunwayHeading = 220;

            Assert.AreEqual(expectedRunwayHeading, model.ResultsSortedByHeadwind[0].Runway.RunwayHeading.DirectionMagnetic);

            // we need to reset the model because the controller modifies it in the previous pass
            model = InitializeAndGetModel();

            model.WindDirectionMagnetic = 90;
            expectedRunwayHeading = 90;

            result = controller.Calculate(model);

            Assert.AreEqual(expectedRunwayHeading, model.ResultsSortedByHeadwind[0].Runway.RunwayHeading.DirectionMagnetic);


            // we need to reset the model again because the controller modifies it in the previous pass
            model = InitializeAndGetModel();

            // this is weird, but 360 degrees also = 0 degrees
            model.WindDirectionMagnetic = 360;
            expectedRunwayHeading = 0;

            result = controller.Calculate(model);

            Assert.AreEqual(expectedRunwayHeading, model.ResultsSortedByHeadwind[0].Runway.RunwayHeading.DirectionMagnetic); 

        }

    }
}