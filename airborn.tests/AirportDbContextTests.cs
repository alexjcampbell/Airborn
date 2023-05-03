using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Airborn.web.Models;
using System.Collections.Generic;

namespace Airborn.Tests
{
    [TestClass]
    public class AirportDbContextTests
    {
        [TestMethod]
        public void Test_AirportDbContext_CanCreate()
        {
            AirportDbContext context = UtilitiesForTesting.GetMockAirportDbContextForTesting();

            Assert.IsNotNull(context);
            Assert.IsInstanceOfType(context, typeof(AirportDbContext));
        }

        [TestMethod]
        public void Test_AirportDbContext_CanGetAirports()
        {
            AirportDbContext context = UtilitiesForTesting.GetMockAirportDbContextForTesting();

            Assert.IsNotNull(context.Airports);
        }

        [TestMethod]
        public void Test_AirportDbContext_CanGetRunways()
        {
            AirportDbContext context = UtilitiesForTesting.GetMockAirportDbContextForTesting();

            Assert.IsNotNull(context.Runways);
        }

        [TestMethod]
        public void Test_GetAirport_ReturnsAirport()
        {
            AirportDbContext context = UtilitiesForTesting.GetMockAirportDbContextForTesting();
            Airport airport = context.GetAirport("KJFK");

            Assert.IsNotNull(airport);
            Assert.IsInstanceOfType(airport, typeof(Airport));
            Assert.AreEqual("KJFK", airport.Ident);
        }

    }
}