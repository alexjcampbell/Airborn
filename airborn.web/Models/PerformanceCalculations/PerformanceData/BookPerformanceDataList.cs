using System;
using System.Collections.Generic;

namespace Airborn.web.Models
{
    public class BookPerformanceDataList : List<BookPerformanceData>
    {

        public BookPerformanceDataList()
        {
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

        public void PopulateFromJson(JsonFile jsonFile)
        {
            foreach (var profile in jsonFile.TakeoffProfiles)
            {
                PopulateProfilesFromJson(Scenario.Takeoff, profile);
            }
            foreach (var profile in jsonFile.LandingProfiles)
            {
                PopulateProfilesFromJson(Scenario.Landing, profile);
            }
        }

        private void PopulateProfilesFromJson(
            Scenario scenario,
            JsonPerformanceProfile profile)
        {
            for (int i = 0; i < profile.GroundRoll.Count; i++)
            {
                var groundRoll = profile.GroundRoll[i];
                var clear50Ft = profile.Clear50FtObstacle[i];

                var bookNumbers = new BookPerformanceData(
                    scenario,
                    profile.PressureAltitude,
                    groundRoll.Temperature,
                    groundRoll.Distance,
                    clear50Ft.Distance
                );

                Add(bookNumbers);
            }
        }

        public BookPerformanceData FindTakeoffBookDistance(decimal pressureAltitude, decimal temperature)
        {
            return FindBookDistance(Scenario.Takeoff, pressureAltitude, temperature);
        }

        public BookPerformanceData FindLandingBookDistance(decimal pressureAltitude, decimal temperature)
        {
            return FindBookDistance(Scenario.Landing, pressureAltitude, temperature);
        }

        public BookPerformanceData FindBookDistance(Scenario scenario, decimal pressureAltitude, decimal temperature)
        {

            if (temperature < 0)
            {
                // downstream code will add a note to the UI that the temperature is negative and so 
                // we've used the book performance for 0 degress C
                temperature = 0;
            }
            if (pressureAltitude < 0)
            {
                // downstream code will add a note to the UI that the pressure altitude is negative and so 
                // we've used the book performance for 0 feet
                pressureAltitude = 0;
            }

            List<BookPerformanceData> bookDistances = FindAll(p => p.Scenario == scenario && p.PressureAltitude == pressureAltitude && p.Temperature == temperature);

            if (bookDistances.Count == 0)
            {
                throw new Exception("No book distances found for scenario " + scenario + " and pressure altitude " + pressureAltitude);
            }

            if (bookDistances.Count > 1)
            {
                throw new Exception("More than one book distance found for scenario " + scenario + " and pressure altitude " + pressureAltitude);
            }

            return bookDistances[0];
        }




    }
}