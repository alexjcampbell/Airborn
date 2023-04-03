using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;

namespace Airborn.Tests
{
    [TestClass]
    public class RunwayTests
    {
        [TestMethod]
        public void LandingDistanceIncludesDisplacedThreshold()
        {
            int runwayHeading = 0;

            Runway runway = new Runway(Direction.FromMagnetic(runwayHeading));

            // regrettably, yes, these are stored as text in the database
            runway.RunwayLength = "5000";
            runway.DisplacedThresholdFt = "1000";

            Assert.AreEqual(4000, runway.LandingAvailableLength.TotalFeet);
        }
    }
}
