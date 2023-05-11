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
    public class ControllerTests_Wind
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
        public void TestDirectHeadwind_HasNoCrosswindComponent()
        {

            var result = _controller.Calculate(_model);

            _model.WindDirectionMagnetic = 220;
            double expectedCrosswind = 0;

            Assert.AreEqual(
                expectedCrosswind,
                _model.Results[0].CrosswindComponent.Value,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_DirectTailwind_HasNoCrosswindComponent()
        {

            _model.WindDirectionMagnetic = 40;
            int expectedCrosswind = 0;

            var result = _controller.Calculate(_model);

            Assert.AreEqual(
                expectedCrosswind,
                _model.Results[0].CrosswindComponent.Value,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CrosswindComponent_IsCorrect()
        {

            _model.WindDirectionMagnetic = 90;
            double expectedCrosswind = -7.66f;

            var result = _controller.Calculate(_model);

            Assert.AreEqual(
                expectedCrosswind,
                _model.Results[0].CrosswindComponent.Value,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_CrosswindComponent_IsCorrectForOppositeWind()
        {
            _model.WindDirectionMagnetic = 270;
            double expectedCrosswind = 7.66f;

            var result = _controller.Calculate(_model);

            Assert.AreEqual(
                expectedCrosswind,
                _model.Results[0].CrosswindComponent.Value,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_RunwaysWithMostTailwind_AreLast()
        {
            _model.WindDirectionMagnetic = 220;

            var result = _controller.Calculate(_model);

            // with a 220 degree wind, runway 4 should have the most tailwind
            // yes, it's expected runway heading is zero
            int expectedRunwayHeading = 40;

            Assert.AreEqual(
                expectedRunwayHeading,
                _model.ResultsSortedByHeadwind[_model.Results.Count - 1].Runway.Runway_Heading_Magnetic.Value.DirectionMagnetic
                );
        }

        [TestMethod]
        public void Test_RunwaysWithMostCrosswind_AreInMiddle()
        {

            _model.WindDirectionMagnetic = 220;

            var result = _controller.Calculate(_model);

            // with a 220 degree wind, runway 36 should have the most tailwind
            // yes, it's expected runway heading is zero
            int expectedRunwayHeading = 180;

            Assert.AreEqual(
                expectedRunwayHeading,
                _model.ResultsSortedByHeadwind[2].Runway.Runway_Heading_Magnetic.Value.DirectionMagnetic
                );
        }


        [TestMethod]
        public void Test_IsBestWindTrueWhenRunway_IsMostAlignedWithWind()
        {

            var result = _controller.Calculate(_model);

            Assert.IsTrue(_model.ResultsSortedByHeadwind.First().IsBestWind);
        }

        [TestMethod]
        public void Test_IsBestWindFalseWhenRunway_IsNotMostAlignedWithWind()
        {

            var result = _controller.Calculate(_model);

            Assert.IsFalse(_model.ResultsSortedByHeadwind.Last().IsBestWind);
        }
    }
}