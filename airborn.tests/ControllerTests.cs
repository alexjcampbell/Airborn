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

        // Default values for the default runway
        const string _default_airport_Ident = "KJFK";
        const int _default_Runway_Id = 1;
        const string _default_RunwayWidth = "150";
        const string _default_RunwayLength = "10000";
        const string _default_Runway_Name = "22R";

        const int _default_Runway_Heading = 220;

        // Default values for the airport
        const int _default_FieldElevation = 0;

        // Default values for the model
        const AircraftType _default_AircraftType = AircraftType.SR22_G2;
        const AltimeterSettingType _default_AltimeterSettingType = AltimeterSettingType.HPA;
        const string _default_AirportIdentifier = "KJFK";
        const int _default_Temperature = 15;
        const TemperatureType _default_TemperatureType = TemperatureType.C;
        const int _default_WindDirectionMagnetic = 220;
        const int _default_WindStrength = 10;
        const int _default_TemperatureCelcius = 12;
        const decimal _default_QNH = 1013.25m;
        const double _default_AircraftWeight = 3000;

        List<Runway> TestRunways = new List<Runway>();
        List<Airport> TestAirports = new List<Airport>();

        HomeController _controller;
        CalculatePageModel _model;

        private void InitializeController()
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
            SetupTestRunway(2, 230, "23L");
            SetupTestRunway(3, 160, "16");
            SetupTestRunway(4, 360, "36");
            SetupTestRunway(5, 90, "9L");

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


            _controller = new HomeController(_doesntDoMuch, mockEnvironment.Object, airportDbContext.Object);
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

        private void InitializeModel()
        {
            _model = new CalculatePageModel();

            _model.AircraftType = _default_AircraftType;
            _model.AltimeterSetting = _default_QNH;
            _model.AltimeterSettingType = _default_AltimeterSettingType;
            _model.AirportIdentifier = _default_AirportIdentifier;
            _model.Temperature = _default_Temperature;
            _model.TemperatureType = _default_TemperatureType;
            _model.WindDirectionMagnetic = _default_WindDirectionMagnetic;
            _model.WindStrength = _default_WindStrength;
            _model.AircraftWeight = (int)_default_AircraftWeight;
            _model.RootPath = AircraftPerformanceTests.TestJsonPath;
        }

        [TestMethod]
        public void TestDirectHeadwindHasNoCrosswindComponent()
        {
            InitializeController();
            InitializeModel();

            var result = _controller.Calculate(_model);

            _model.WindDirectionMagnetic = 220;
            int expectedCrosswind = 0;

            Assert.AreEqual(_model.Results[0].CrosswindComponent, expectedCrosswind);
        }

        [TestMethod]
        public void TestDirectTailwindHasNoCrosswindComponent()
        {
            InitializeController();
            InitializeModel();

            _model.WindDirectionMagnetic = 40;
            int expectedCrosswind = 0;

            var result = _controller.Calculate(_model);

            Assert.AreEqual(expectedCrosswind, Math.Round(_model.Results[0].CrosswindComponent.Value));
        }

        [TestMethod]
        public void TestCrosswindComponentIsCorrect()
        {
            InitializeController();
            InitializeModel();

            _model.WindDirectionMagnetic = 90;
            decimal expectedCrosswind = -7.6604444311898m;

            var result = _controller.Calculate(_model);

            Assert.AreEqual(expectedCrosswind, _model.Results[0].CrosswindComponent);
        }

        [TestMethod]
        public void TestCrosswindComponentIsCorrectForOppositeWind()
        {
            InitializeController();
            InitializeModel();

            _model.WindDirectionMagnetic = 270;
            decimal expectedCrosswind = 7.66044443118978m;

            var result = _controller.Calculate(_model);

            Assert.AreEqual(expectedCrosswind, _model.Results[0].CrosswindComponent);
        }

        [TestMethod]
        public void TestPressureAltitudeEqualsFieldAltitudeWhenStandardPressureMb()
        {
            InitializeController();
            InitializeModel();

            _model.AltimeterSetting = 1013.25m;
            _model.AltimeterSettingType = AltimeterSettingType.HPA;

            var result = _controller.Calculate(_model);

            // altimeter setting of 1013.25m is standard pressure at sea level,
            // so field elevation should equal pressure altitude
            // but due to rounding errors, pressure altitude doesn't quite equal zero ft (sea level)
            decimal expectedPressureAltitude = 6.825m;
            Assert.AreEqual(expectedPressureAltitude, _model.PressureAltitude);
        }

        [TestMethod]
        public void TestPressureAltitudeEqualsFieldAltitudeWhenStandardPressureHg()
        {
            InitializeController();
            InitializeModel();

            _model.AltimeterSetting = 29.92m;
            _model.AltimeterSettingType = AltimeterSettingType.HG;

            var result = _controller.Calculate(_model);

            decimal expectedPressureAltitude = 0.0000000253344m;

            // Altimeter setting of 29.92 is standard pressure at sea level, so field elevation
            // should equal pressure altitude. However, rounding errors mean pressure altitude doesn't 
            // quite equal zero ft (sea level) as you'd expect
            Assert.AreEqual(expectedPressureAltitude, _model.PressureAltitude);
        }

        [TestMethod]
        public void TestPressureAltitudeIsCorrectWhenAltimeterSettingIsLowerThanStandard()
        {
            InitializeController();
            InitializeModel();

            _model.AltimeterSetting = 29.82m;
            _model.AltimeterSettingType = AltimeterSettingType.HG;

            var result = _controller.Calculate(_model);

            // lower pressure means higher altitude, at a rate of ~90 feet per inch of mercury
            decimal expectedPressureAltitude = _default_FieldElevation + 92.4522894637524m;

            Assert.AreEqual(expectedPressureAltitude, _model.PressureAltitude);
        }

        [TestMethod]
        public void TestPressureAltitudeIsCorrectWhenAltimeterSettingIsHigherThanStandard()
        {
            InitializeController();
            InitializeModel();

            _model.AltimeterSetting = 30.02m;
            _model.AltimeterSettingType = AltimeterSettingType.HG;

            var result = _controller.Calculate(_model);

            // higher pressure means lower altitude, at a rate of ~90 feet per inch of mercury
            decimal expectedPressureAltitude = _default_FieldElevation - 92.4522894130836m;

            Assert.AreEqual(expectedPressureAltitude, _model.PressureAltitude);
        }

        [TestMethod]
        public void TestReturnResultsEvenWhenPressureAltitudeIsNegative()
        {
            InitializeController();
            InitializeModel();

            _model.AltimeterSetting = 1040;
            _model.AltimeterSettingType = AltimeterSettingType.HPA;

            var result = _controller.Calculate(_model);

            Assert.IsTrue(_model.PressureAltitude < 0);

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
        public void TestPressureAltitudeShouldOnlyOverrideToZeroWhenPressureAltitudeIsNegative()
        {
            InitializeController();
            InitializeModel();

            _model.AltimeterSetting = 1000;
            _model.AltimeterSettingType = AltimeterSettingType.HPA;

            decimal expectedPressureAltitude = 361.725m;

            var result = _controller.Calculate(_model);

            Assert.IsTrue(_model.PressureAltitude > 0);

            Assert.AreEqual(expectedPressureAltitude, _model.PressureAltitudeAlwaysPositiveOrZero);
        }

        [TestMethod]
        public void TestReturnResultsEvenWhenTemperatureIsNegative()
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
        public void TestRunwaysWithMostHeadwindAreFirst()
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
        public void TestRunwaysWithMostTailwindAreLast()
        {
            InitializeController();
            InitializeModel();

            _model.WindDirectionMagnetic = 220;

            var result = _controller.Calculate(_model);

            // with a 220 degree wind, runway 36 should have the most tailwind
            // yes, it's expected runway heading is zero
            int expectedRunwayHeading = 0;

            Assert.AreEqual(expectedRunwayHeading, _model.ResultsSortedByHeadwind[4].Runway.RunwayHeading.DirectionMagnetic);
        }

        [TestMethod]
        public void TestRunwaysWithMostCrosswindAreInMiddle()
        {
            InitializeController();
            InitializeModel();

            _model.WindDirectionMagnetic = 220;

            var result = _controller.Calculate(_model);

            // with a 220 degree wind, runway 36 should have the most tailwind
            // yes, it's expected runway heading is zero
            int expectedRunwayHeading = 160;

            Assert.AreEqual(expectedRunwayHeading, _model.ResultsSortedByHeadwind[2].Runway.RunwayHeading.DirectionMagnetic);
        }


        [TestMethod]
        public void TestIsBestWindTrueWhenRunwayIsMostAlignedWithWind()
        {
            InitializeController();
            InitializeModel();

            var result = _controller.Calculate(_model);

            Assert.IsTrue(_model.ResultsSortedByHeadwind.First().IsBestWind);
        }

        [TestMethod]
        public void TestIsBestWindFalseWhenRunwayIsNotMostAlignedWithWind()
        {
            InitializeController();
            InitializeModel();

            var result = _controller.Calculate(_model);

            Assert.IsFalse(_model.ResultsSortedByHeadwind.Last().IsBestWind);
        }

    }
}