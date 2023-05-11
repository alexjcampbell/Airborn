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
    public class ControllerTests_Altitude
    {

        private ILogger<HomeController> _doesntDoMuch =
            new Microsoft.Extensions.Logging.Abstractions.NullLogger<HomeController>();


        HomeController _controller;
        CalculatePageModel _model;

        [TestInitialize]
        public void Initialize()
        {
            _controller = ControllerTests.InitializeController(_doesntDoMuch);
            _model = ControllerTests.InitializeModel();
        }

        [TestMethod]
        public void Test_PressureAltitude_EqualsFieldAltitudeWhenStandardPressureMb()
        {
            _model.AltimeterSetting = 1013.25f;
            _model.AltimeterSettingType = AltimeterSettingType.MB;

            var result = _controller.Calculate(_model);

            // altimeter setting of 1013.25m is standard pressure at sea level,
            // so field elevation should equal pressure altitude
            // but due to rounding errors, pressure altitude doesn't quite equal zero ft (sea level)
            double expectedPressureAltitude = 6.825f;

            Assert.AreEqual(
                expectedPressureAltitude,
                _model.PressureAltitude.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_PressureAltitude_EqualsFieldAltitudeWhenStandardPressureHg()
        {

            _model.AltimeterSetting = 29.92f;
            _model.AltimeterSettingType = AltimeterSettingType.HG;

            var result = _controller.Calculate(_model);

            double expectedPressureAltitude = 0;

            // Altimeter setting of 29.92 is standard pressure at sea level, so field elevation
            // should equal pressure altitude. However, rounding errors mean pressure altitude doesn't 
            // quite equal zero ft (sea level) as you'd expect
            Assert.AreEqual(
                expectedPressureAltitude,
                _model.PressureAltitude.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_PressureAltitude_IsCorrectWhenAltimeterSettingIsLowerThanStandard()
        {

            _model.AltimeterSetting = 29.82f;
            _model.AltimeterSettingType = AltimeterSettingType.HG;

            var result = _controller.Calculate(_model);

            // lower pressure means higher altitude, at a rate of ~90 feet per inch of mercury
            double expectedPressureAltitude = UtilitiesForTesting.Default_FieldElevation + 92.4522894637524f;

            Assert.AreEqual(
                expectedPressureAltitude,
                _model.PressureAltitude.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_PressureAltitude_IsCorrectWhenAltimeterSettingIsHigherThanStandard()
        {

            _model.AltimeterSetting = 30.02f;
            _model.AltimeterSettingType = AltimeterSettingType.HG;

            var result = _controller.Calculate(_model);

            // higher pressure means lower altitude, at a rate of ~90 feet per inch of mercury
            double expectedPressureAltitude = UtilitiesForTesting.Default_FieldElevation - 92.4522894130836f;

            Assert.AreEqual(
                expectedPressureAltitude,
                _model.PressureAltitude.Value.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_ReturnResults_EvenWhenPressureAltitudeIsNegative()
        {

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

            _model.AltimeterSetting = 1000;
            _model.AltimeterSettingType = AltimeterSettingType.MB;

            double expectedPressureAltitude = 361.72f;

            var result = _controller.Calculate(_model);

            Assert.IsTrue(_model.PressureAltitude.Value.TotalFeet > 0);

            Assert.AreEqual(
                expectedPressureAltitude,
                _model.PressureAltitudeAlwaysPositiveOrZero.Value,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

    }
}