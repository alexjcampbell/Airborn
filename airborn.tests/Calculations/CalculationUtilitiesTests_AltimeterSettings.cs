using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class CalculationUtilitiesTests_AltimeterSettings
    {

        [TestMethod]
        public void Test_InchesToMillibars_29Point92InchesOfMercury_Equals1013Millibars()
        {
            double inchesOfMercury = 29.92f;
            double expectedMillibars = 1013.25f;

            Assert.AreEqual(
                expectedMillibars,
                CalculationUtilities.InchesOfMercuryToMillibars(inchesOfMercury),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_InchesToMillibars_30Point00InchesOfMercury_Equals1015Millibars()
        {
            double inchesOfMercury = 30f;
            double expectedMillibars = 1015.95f;

            Assert.AreEqual(
                expectedMillibars,
                CalculationUtilities.InchesOfMercuryToMillibars(inchesOfMercury),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }

        [TestMethod]
        public void Test_InchesToMillibars_30Point92InchesOfMercury_Equals1047Millibars()
        {
            double inchesOfMercury = 30.92f;
            double expectedMillibars = 1047.11f;

            Assert.AreEqual(
                expectedMillibars,
                CalculationUtilities.InchesOfMercuryToMillibars(inchesOfMercury),
                UtilitiesForTesting.MinimumPrecisisionForDoubleComparison
                );
        }
    }

}