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


            var testRunways = new List<Runway>(){
                new Runway(Direction.FromMagnetic(10, 0))
            };

            testRunways[0].Airport_Ident = "KJFK";
            testRunways[0].Runway_Id = 1;
            testRunways[0].RunwayWidth = "150";
            testRunways[0].RunwayLength = "10000";
            testRunways[0].Runway_Name = "22R";

            // Set up the DbSet as an IQueryable so it can be enumerated.
            var queryableRunways = testRunways.AsQueryable();
            runwayDbSet.As<IQueryable<Runway>>().Setup(m => m.Provider).Returns(queryableRunways.Provider);
            runwayDbSet.As<IQueryable<Runway>>().Setup(m => m.Expression).Returns(queryableRunways.Expression);
            runwayDbSet.As<IQueryable<Runway>>().Setup(m => m.ElementType).Returns(queryableRunways.ElementType);
            runwayDbSet.As<IQueryable<Runway>>().Setup(m => m.GetEnumerator()).Returns(() => queryableRunways.GetEnumerator());

            var airportDbSet = new Mock<DbSet<Airport>>();
            airportDbContext.Setup(o => o.Airports).Returns(() => airportDbSet.Object);

            var testAirports = new List<Airport>(){
                new Airport()
            };

            testAirports[0].Ident = "KJFK";
            testAirports[0].FieldElevation = 9;

            // Set up the DbSet as an IQueryable so it can be enumerated.
            var queryableAirports = testAirports.AsQueryable();
            airportDbSet.As<IQueryable<Airport>>().Setup(m => m.Provider).Returns(queryableAirports.Provider);
            airportDbSet.As<IQueryable<Airport>>().Setup(m => m.Expression).Returns(queryableAirports.Expression);
            airportDbSet.As<IQueryable<Airport>>().Setup(m => m.ElementType).Returns(queryableAirports.ElementType);
            airportDbSet.As<IQueryable<Airport>>().Setup(m => m.GetEnumerator()).Returns(() => queryableAirports.GetEnumerator());


            var controller = new HomeController(_doesntDoMuch, mockEnvironment.Object, airportDbContext.Object);

            return controller;
        }

        private CalculatePageModel InitializeAndGetModel()
        {
            CalculatePageModel model = new CalculatePageModel();
            
            model.AircraftType = AircraftType.C172_SP;
            model.AltimeterSetting = 29.92m;
            model.AltimeterSettingType = AltimeterSettingType.HG;
            model.AirportIdentifier = "KJFK";
            model.Temperature = 15;
            model.TemperatureType = TemperatureType.C;
            model.WindDirectionMagnetic = 220;
            model.WindStrength = 10;
            model.AircraftWeight = 2500;
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


    }
}