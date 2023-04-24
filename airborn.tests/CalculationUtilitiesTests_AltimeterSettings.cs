using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class CalculationUtilitiesTests_AltimeterSettings
    {

        [TestMethod]
        public void TestInchesToMillibars_29Point92InchesOfMercuryEquals1013Millibars()
        {
            decimal inchesOfMercury = 29.92m;
            decimal expectedMillibars = 1013.25m;

            Assert.AreEqual(
                Math.Round(expectedMillibars, 2),
                Math.Round(CalculationUtilities.InchesOfMercuryToMillibars(inchesOfMercury), 2)
                );
        }

        [TestMethod]
        public void TestInchesToMillibars_30Point00InchesOfMercuryEquals1015Millibars()
        {
            decimal inchesOfMercury = 30m;
            decimal expectedMillibars = 1015.9592245980m;

            Assert.AreEqual(
                expectedMillibars,
                CalculationUtilities.InchesOfMercuryToMillibars(inchesOfMercury)
                );
        }

        [TestMethod]
        public void TestInchesToMillibars_30Point92InchesOfMercuryEquals1047Millibars()
        {
            decimal inchesOfMercury = 30.92m;
            decimal expectedMillibars = 1047.115307485672m;

            Assert.AreEqual(
                expectedMillibars,
                CalculationUtilities.InchesOfMercuryToMillibars(inchesOfMercury)
                );
        }
    }

}