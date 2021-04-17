using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class DistanceTests
    {
        [TestMethod]
        public void TestMetresPerKilometer()
        {
            Distance distance = Distance.FromMeters(1000);

            Assert.AreEqual(1, distance.TotalKilometers);
        }

        [TestMethod]
        public void TestFeetPerKilometer()
        {
            Distance distance = Distance.FromKilometers(1);
            Assert.AreEqual(3280.8398950131236, distance.TotalFeet);
        }
    }
}
