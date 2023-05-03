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
using Airborn.Controllers;

namespace Airborn.web.Models
{
    public static class UtilitiesForTesting
    {

        public const string Default_AirportIdentifier = "KJFK";
        public const int Default_Runway_Id = 1;
        public const int Default_RunwayWidth = 150;
        public const int Default_RunwayLength = 10000;
        public const string Default_Runway_Name = "22R";

        public const int Default_Runway_Heading = 220;

        public const int Default_FieldElevation = 0;

        public const AircraftType Default_AircraftType = AircraftType.SR22_G2;
        public const AltimeterSettingType Default_AltimeterSettingType = AltimeterSettingType.MB;

        public const double Default_QNH = 1013.25f;

        public const int Default_Temperature = 15;
        public const TemperatureType Default_TemperatureType = TemperatureType.C;
        public const int Default_WindDirectionMagnetic = 220;
        public const int Default_WindStrength = 10;
        public const int Default_TemperatureCelcius = 12;
        public const double Default_AircraftWeight = 3000;

        public const double Default_RunwayElevation = 0;

        public const double MinimumPrecisisionForDoubleComparison = 0.01d;

        public static AirportDbContext GetMockAirportDbContextForTesting()
        {


            List<Runway> TestRunways = new List<Runway>();
            List<Airport> TestAirports = new List<Airport>();

            var airportDbContext = new Mock<AirportDbContext>();

            var runwayDbSet = new Mock<DbSet<Runway>>();
            airportDbContext.Setup(o => o.Runways).Returns(() => runwayDbSet.Object);

            SetupTestRunway(Default_Runway_Id, Default_Runway_Heading, Default_Runway_Name, TestRunways);
            SetupTestRunway(2, 230, "23L", TestRunways);
            SetupTestRunway(3, 50, "5R", TestRunways);
            SetupTestRunway(4, 160, "16", TestRunways);
            SetupTestRunway(5, 340, "34", TestRunways);
            SetupTestRunway(6, 180, "18", TestRunways);
            SetupTestRunway(7, 360, "36", TestRunways);
            SetupTestRunway(8, 90, "9L", TestRunways);
            SetupTestRunway(9, 270, "27R", TestRunways);
            SetupTestRunway(10, 40, "4L", TestRunways);

            // Set up the DbSet as an IQueryable so it can be enumerated.
            var queryableRunways = TestRunways.AsQueryable();
            runwayDbSet.As<IQueryable<Runway>>().Setup(m => m.Provider).Returns(queryableRunways.Provider);
            runwayDbSet.As<IQueryable<Runway>>().Setup(m => m.Expression).Returns(queryableRunways.Expression);
            runwayDbSet.As<IQueryable<Runway>>().Setup(m => m.ElementType).Returns(queryableRunways.ElementType);
            runwayDbSet.As<IQueryable<Runway>>().Setup(m => m.GetEnumerator()).Returns(() => queryableRunways.GetEnumerator());

            var airportDbSet = new Mock<DbSet<Airport>>();
            airportDbContext.Setup(o => o.Airports).Returns(() => airportDbSet.Object);

            var testAirports = new List<Airport>(){
                new Airport()
            };

            testAirports[0].Ident = Default_AirportIdentifier;
            testAirports[0].FieldElevation = Default_FieldElevation;

            // Set up the DbSet as an IQueryable so it can be enumerated.
            var queryableAirports = testAirports.AsQueryable();
            airportDbSet.As<IQueryable<Airport>>().Setup(m => m.Provider).Returns(queryableAirports.Provider);
            airportDbSet.As<IQueryable<Airport>>().Setup(m => m.Expression).Returns(queryableAirports.Expression);
            airportDbSet.As<IQueryable<Airport>>().Setup(m => m.ElementType).Returns(queryableAirports.ElementType);
            airportDbSet.As<IQueryable<Airport>>().Setup(m => m.GetEnumerator()).Returns(() => queryableAirports.GetEnumerator());

            return airportDbContext.Object;

        }

        private static void SetupTestRunway(int runwayId, int runwayHeading, string runwayName, List<Runway> TestRunways)
        {
            Runway runway = new Runway(Default_Runway_Name);

            runway.Airport_Ident = Default_AirportIdentifier;
            runway.Runway_Id = runwayId;
            runway.RunwayWidth = Default_RunwayWidth;
            runway.RunwayLength = Default_RunwayLength;
            runway.Runway_Name = runwayName;
            runway.ElevationFt = (int)Default_RunwayElevation;

            TestRunways.Add(runway);

        }

        public static List<JsonFile> GetMockJsonPerformanceFilesForTesting()
        {
            List<JsonFile> jsonFiles = new List<JsonFile>();

            Mock<JsonFile> jsonFileLowerWeight = new Mock<JsonFile>();
            Mock<JsonFile> jsonFileHigherWeight = new Mock<JsonFile>();

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(Default_AircraftType);

            int _aircraftLowerWeight = Aircraft.GetAircraftFromAircraftType(Default_AircraftType).GetLowerWeight();
            int _aircraftHigherWeight = Aircraft.GetAircraftFromAircraftType(Default_AircraftType).GetHigherWeight();

            jsonFileLowerWeight.Setup(x => x.TakeoffProfiles).Returns(new JsonPerformanceProfileList());
            jsonFileLowerWeight.Setup(x => x.LandingProfiles).Returns(new JsonPerformanceProfileList());

            jsonFileLowerWeight.Object.AircraftWeight = _aircraftLowerWeight;

            jsonFileLowerWeight.Object.TakeoffProfiles.Add(PopulateAndReturnJsonProfile(0, "Takeoff"));
            jsonFileLowerWeight.Object.TakeoffProfiles.Add(PopulateAndReturnJsonProfile(1000, "Takeoff"));
            jsonFileLowerWeight.Object.TakeoffProfiles.Add(PopulateAndReturnJsonProfile(2000, "Takeoff"));
            jsonFileLowerWeight.Object.TakeoffProfiles.Add(PopulateAndReturnJsonProfile(3000, "Takeoff"));

            jsonFileLowerWeight.Object.LandingProfiles.Add(PopulateAndReturnJsonProfile(0, "Landing"));
            jsonFileLowerWeight.Object.LandingProfiles.Add(PopulateAndReturnJsonProfile(1000, "Landing"));
            jsonFileLowerWeight.Object.LandingProfiles.Add(PopulateAndReturnJsonProfile(2000, "Landing"));
            jsonFileLowerWeight.Object.LandingProfiles.Add(PopulateAndReturnJsonProfile(3000, "Landing"));

            jsonFileHigherWeight.Setup(x => x.TakeoffProfiles).Returns(new JsonPerformanceProfileList());
            jsonFileHigherWeight.Setup(x => x.LandingProfiles).Returns(new JsonPerformanceProfileList());

            jsonFileHigherWeight.Object.AircraftWeight = _aircraftHigherWeight;

            jsonFileHigherWeight.Object.TakeoffProfiles.Add(PopulateAndReturnJsonProfile(0, "Takeoff"));
            jsonFileHigherWeight.Object.TakeoffProfiles.Add(PopulateAndReturnJsonProfile(1000, "Takeoff"));
            jsonFileHigherWeight.Object.TakeoffProfiles.Add(PopulateAndReturnJsonProfile(2000, "Takeoff"));
            jsonFileHigherWeight.Object.TakeoffProfiles.Add(PopulateAndReturnJsonProfile(3000, "Takeoff"));

            jsonFileHigherWeight.Object.LandingProfiles.Add(PopulateAndReturnJsonProfile(0, "Landing"));
            jsonFileHigherWeight.Object.LandingProfiles.Add(PopulateAndReturnJsonProfile(1000, "Landing"));
            jsonFileHigherWeight.Object.LandingProfiles.Add(PopulateAndReturnJsonProfile(2000, "Landing"));
            jsonFileHigherWeight.Object.LandingProfiles.Add(PopulateAndReturnJsonProfile(3000, "Landing"));

            jsonFiles.Add(jsonFileLowerWeight.Object);
            jsonFiles.Add(jsonFileHigherWeight.Object);

            return jsonFiles;
        }

        private static JsonPerformanceProfile PopulateAndReturnJsonProfile(int pressureAltitude, string type)
        {
            JsonPerformanceProfile profile = new JsonPerformanceProfile();
            profile.PressureAltitude = pressureAltitude;
            profile.Type = type;
            profile.GroundRoll = new JsonPerformanceProfileResultList();
            profile.Clear50FtObstacle = new JsonPerformanceProfileResultList();

            for (int i = 0; i < 6; i++)
            {
                JsonPerformanceProfileResult groundRoll = new JsonPerformanceProfileResult();
                groundRoll.Temperature = i * 10;

                // use 1/10th of the pressure altitude to add some upward slope as pressure altitude increasess
                groundRoll.Distance = (int)(((i + 1) * 100) * (0.1m * (pressureAltitude + 1))); // +1 to avoid multipling by zero and getting zero
                profile.GroundRoll.Add(groundRoll);

                JsonPerformanceProfileResult clear50Ft = new JsonPerformanceProfileResult();
                clear50Ft.Temperature = i * 10;

                // use 1/10th of the pressure altitude to add some upward slope as pressure altitude increases
                clear50Ft.Distance = (int)(((i + 10) * 120) * (0.1m * (pressureAltitude + 1))); // +1 to avoid multipling by zero and getting zero
                profile.Clear50FtObstacle.Add(clear50Ft);

            }

            return profile;
        }

    }
}