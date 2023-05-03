using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class DistanceTests
    {
        [TestMethod]
        public void Test_MetresPerKilometer_Is1000To1()
        {
            Distance distance = Distance.FromMeters(1000);

            Assert.AreEqual(1, distance.TotalKilometers);
        }

        [TestMethod]
        public void Test_FeetPerKilometer_Is3280()
        {
            Distance distance = Distance.FromKilometers(1);
            Assert.AreEqual(3280.8398950131233595800524934m, distance.TotalFeet);
        }

        [TestMethod]
        public void Test_FormatToStringAsFt_ReturnsFormattedInFeet()
        {
            Distance distance = Distance.FromFeet(1000);
            Assert.AreEqual("1,000 ft", distance.ToString());
        }

    }
}
