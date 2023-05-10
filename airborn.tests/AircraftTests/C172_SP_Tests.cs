using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class C172_SP_Tests
    {
        private C172_SP _c172_sp;
        private Airport _airport;
        private Runway _runway;
        private Wind _wind;
        private CalculationResultForRunway _result;
        private PerformanceCalculationLogger _logger;
        private PerformanceCalculationLogItem _logItem;
        private AirconOptions _airconOption;

        Distance _unadjustedTakeoffDistance;

        [TestInitialize]
        public void TestInitialize()
        {
            _c172_sp = new C172_SP();
            _airport = new Airport();
            _runway = new Runway(_airport, Direction.FromTrue(0, 0), "36");
            _logger = new PerformanceCalculationLogger();
            _logItem = new PerformanceCalculationLogItem("");
            _airconOption = AirconOptions.NoAircon;
            _unadjustedTakeoffDistance = Distance.FromFeet(1000);

        }

        [TestMethod]
        public void TestMakeTakeoffAdjustments_WithHeadwind_Subtracts10PercentForEveryNineKnots()
        {

            _wind = Wind.FromMagnetic(0, 0, 10); // 10 knots headwind

            _result = new CalculationResultForRunway(_runway, _wind, _logItem, Distance.FromFeet(0));

            var adjustedTakeoffDistance = _c172_sp.MakeTakeoffAdjustments(
                _result,
                _unadjustedTakeoffDistance,
                _logItem,
                _airconOption);

            Assert.AreEqual(
                Distance.FromFeet(888.88).TotalFeet,
                adjustedTakeoffDistance.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void TestMakeTakeoffAdjustments_WithTailwind_Adds10PercentForEverTwoKnots()
        {

            var unadjustedTakeoffDistance = Distance.FromFeet(1000);

            _wind = Wind.FromMagnetic(180, 0, 10); // 10 knots tailwind

            _result = new CalculationResultForRunway(_runway, _wind, _logItem, Distance.FromFeet(0));

            var adjustedTakeoffDistance = _c172_sp.MakeTakeoffAdjustments(
                _result,
                _unadjustedTakeoffDistance,
                _logItem,
                _airconOption);

            Assert.AreEqual(
                Distance.FromFeet(1500).TotalFeet,
                adjustedTakeoffDistance.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void TestMakeTakeoffAdjustments_WithTailwind_GrassAdds15Percent()
        {

            var unadjustedTakeoffDistance = Distance.FromFeet(1000);

            _wind = Wind.FromMagnetic(180, 0, 0); // no wind
            _runway.SurfaceText = "GRASS";

            _result = new CalculationResultForRunway(_runway, _wind, _logItem, Distance.FromFeet(0));

            var adjustedTakeoffDistance = _c172_sp.MakeTakeoffAdjustments(
                _result,
                _unadjustedTakeoffDistance,
                _logItem,
                _airconOption);

            Assert.AreEqual(
                Distance.FromFeet(1150).TotalFeet,
                adjustedTakeoffDistance.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }




        [TestMethod]
        public void TestMakeLandingAdjustments_WithHeadwind_Subtracts10PercentForEveryNineKnots()
        {

            _wind = Wind.FromMagnetic(0, 0, 10); // 10 knots headwind

            _result = new CalculationResultForRunway(_runway, _wind, _logItem, Distance.FromFeet(0));

            var adjustedTakeoffDistance = _c172_sp.MakeLandingAdjustments(
                _result,
                _unadjustedTakeoffDistance,
                _logItem,
                _airconOption);

            Assert.AreEqual(
                Distance.FromFeet(888.88).TotalFeet,
                adjustedTakeoffDistance.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void TestMakeLandingAdjustments_WithTailwind_Adds10PercentForEverTwoKnots()
        {

            var unadjustedTakeoffDistance = Distance.FromFeet(1000);

            _wind = Wind.FromMagnetic(180, 0, 10); // 10 knots tailwind

            _result = new CalculationResultForRunway(_runway, _wind, _logItem, Distance.FromFeet(0));

            var adjustedTakeoffDistance = _c172_sp.MakeLandingAdjustments(
                _result,
                _unadjustedTakeoffDistance,
                _logItem,
                _airconOption);

            Assert.AreEqual(
                Distance.FromFeet(1500).TotalFeet,
                adjustedTakeoffDistance.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void TestMakeLandingAdjustments_WithTailwind_GrassAdds45Percent()
        {

            var unadjustedTakeoffDistance = Distance.FromFeet(1000);

            _wind = Wind.FromMagnetic(180, 0, 0); // no wind
            _runway.SurfaceText = "GRASS";

            _result = new CalculationResultForRunway(_runway, _wind, _logItem, Distance.FromFeet(0));

            var adjustedTakeoffDistance = _c172_sp.MakeLandingAdjustments(
                _result,
                _unadjustedTakeoffDistance,
                _logItem,
                _airconOption);

            Assert.AreEqual(
                Distance.FromFeet(1450).TotalFeet,
                adjustedTakeoffDistance.TotalFeet,
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_HasAirconOption_IsFalse()
        {
            Assert.IsFalse(
                _c172_sp.HasAirconOption
                );
        }

    }
}