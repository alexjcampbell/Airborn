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
                Math.Round(CalculationUtilities.ConvertInchesOfMercuryToMillibars(inchesOfMercury), 2)
                );
        }

        [TestMethod]
        public void TestInchesToMillibars_30Point00InchesOfMercuryEquals1013Millibars()
        {
            decimal inchesOfMercury = 30m;
            decimal expectedMillibars = 1015.9592245980m;

            Assert.AreEqual(
                expectedMillibars,
                CalculationUtilities.ConvertInchesOfMercuryToMillibars(inchesOfMercury)
                );
        }

    }

}