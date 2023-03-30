using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Airborn.web.Models;
using Moq;

namespace Airborn.Controllers.Tests
{
    [TestClass]
    public class ControllerTests
    {
        [TestMethod]
        public void TestName()
        {


            ILogger<HomeController> doesntDoMuch = new Microsoft.Extensions.Logging.Abstractions.NullLogger<HomeController>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            //...Setup the mock as needed
            mockEnvironment
                .Setup(m => m.EnvironmentName)
                .Returns("Hosting:UnitTestEnvironment");

            var controller = new HomeController(doesntDoMuch, mockEnvironment.Object);
            var model = new CalculatePageModel();

            model.AircraftType = AircraftType.C172_SP;
            model.AltimeterSetting = 29.92m;
            model.AltimeterSettingType = AltimeterSettingType.HG;
            model.AirportIdentifier = "KJFK";
            model.Temperature = 15;
            model.TemperatureType = TemperatureType.C;
            model.WindDirectionMagnetic = 240;
            model.WindStrength = 10;

            //var result = controller.Calculate(model);

            //Assert.AreEqual(model.Results[0].CrosswindComponent, 19);

            // When

            // Then
        }
    }
}