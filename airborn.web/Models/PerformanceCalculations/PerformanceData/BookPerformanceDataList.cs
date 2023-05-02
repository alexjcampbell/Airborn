using System;
using System.Collections.Generic;
using System.IO;

namespace Airborn.web.Models
{
    /// <summary>
    /// Represents a list of BookPerformanceData objects for a given aircraft weight
    /// </summary>
    public class BookPerformanceDataList : List<BookPerformanceData>
    {

        public BookPerformanceDataList(int AircraftLowerWeight, int AircraftHigherWeight)
            : base()
        {
            this.AircraftLowerWeight = AircraftLowerWeight;
            this.AircraftHigherWeight = AircraftHigherWeight;
        }

        public int AircraftLowerWeight
        {
            get;
            private set;
        }

        public int AircraftHigherWeight
        {
            get;
            private set;
        }

        public List<BookPerformanceData> TakeoffPerformanceData
        {
            get
            {
                return FindAll(x => x.Scenario == Scenario.Takeoff);
            }
        }

        public List<BookPerformanceData> LandingPerformanceData
        {
            get
            {
                return FindAll(x => x.Scenario == Scenario.Landing);
            }
        }

        public BookPerformanceData FindTakeoffBookDistance(Distance pressureAltitude, decimal temperature, decimal weight)
        {
            return FindBookDistance(Scenario.Takeoff, pressureAltitude, temperature, weight);
        }

        public BookPerformanceData FindLandingBookDistance(Distance pressureAltitude, decimal temperature, decimal weight)
        {
            return FindBookDistance(Scenario.Landing, pressureAltitude, temperature, weight);
        }

        public BookPerformanceData FindBookDistance(Scenario scenario, Distance pressureAltitude, decimal temperature, decimal weight)
        {

            if (temperature < 0)
            {
                // downstream code will add a note to the UI that the temperature is negative and so 
                // we've used the book performance for 0 degress C
                temperature = 0;
            }
            if (pressureAltitude.TotalFeet < 0)
            {
                // downstream code will add a note to the UI that the pressure altitude is negative and so 
                // we've used the book performance for 0 feet
                pressureAltitude = Distance.FromFeet(0);
            }

            if (weight < AircraftLowerWeight)
            {
                throw new Exception("Weight " + weight + " is less than the lower possible weight " + AircraftLowerWeight);
            }
            if (weight > AircraftHigherWeight)
            {
                throw new Exception("Weight " + weight + " is greater than the higher possible weight " + AircraftHigherWeight);
            }

            List<BookPerformanceData> bookDistances = FindAll(
                p => p.Scenario == scenario && p.PressureAltitude.TotalFeet == pressureAltitude.TotalFeet && p.Temperature == temperature && p.AircraftWeight == weight);

            if (bookDistances.Count == 0)
            {
                throw new NoPerformanceDataFoundException(pressureAltitude.TotalFeet, temperature, weight);
            }

            if (bookDistances.Count > 1)
            {
                throw new Exception("More than one book distance found for scenario " + scenario + " and pressure altitude " + pressureAltitude + " temperature " + temperature + " weight " + weight);
            }

            return bookDistances[0];
        }

        public void PopulateFromJsonStringPath(Aircraft aircraft, string jsonPath)
        {
            string fullPathLowerWeight = Path.Combine(jsonPath, aircraft.JsonFileName_LowerWeight());
            string fullPathHigherWeight = Path.Combine(jsonPath, aircraft.JsonFileName_HigherWeight());

            JsonFile jsonFileLowerWeight = new JsonFile(fullPathLowerWeight);
            JsonFile jsonFileHigherWeight = new JsonFile(fullPathHigherWeight);

            PopulateFromJson(aircraft, jsonFileLowerWeight, jsonFileHigherWeight);
        }

        public void PopulateFromJson(Aircraft aircraft, JsonFile jsonFileLowerWeight, JsonFile jsonFileHigherWeight)
        {
            AircraftLowerWeight = aircraft.GetLowerWeight();
            AircraftHigherWeight = aircraft.GetHigherWeight();

            foreach (var profile in jsonFileLowerWeight.TakeoffProfiles)
            {
                PopulatePerformanceDataListFromJsonProfile(Scenario.Takeoff, profile, AircraftLowerWeight);
            }
            foreach (var profile in jsonFileLowerWeight.LandingProfiles)
            {
                PopulatePerformanceDataListFromJsonProfile(Scenario.Landing, profile, AircraftLowerWeight);
            }
            foreach (var profile in jsonFileHigherWeight.TakeoffProfiles)
            {
                PopulatePerformanceDataListFromJsonProfile(Scenario.Takeoff, profile, AircraftHigherWeight);
            }
            foreach (var profile in jsonFileHigherWeight.LandingProfiles)
            {
                PopulatePerformanceDataListFromJsonProfile(Scenario.Landing, profile, AircraftHigherWeight);
            }
        }

        private void PopulatePerformanceDataListFromJsonProfile(
            Scenario scenario,
            JsonPerformanceProfile profile,
            decimal weight)
        {
            for (int i = 0; i < profile.GroundRoll.Count; i++)
            {
                var groundRoll = profile.GroundRoll[i];
                var clear50Ft = profile.Clear50FtObstacle[i];

                var bookNumbers = new BookPerformanceData(
                    scenario,
                    Distance.FromFeet(profile.PressureAltitude),
                    groundRoll.Temperature,
                    weight,
                    Distance.FromFeet(groundRoll.Distance),
                    Distance.FromFeet(clear50Ft.Distance)
                );

                Add(bookNumbers);
            }
        }

    }
}