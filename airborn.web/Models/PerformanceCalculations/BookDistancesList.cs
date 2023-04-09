using System;
using System.Collections.Generic;

namespace Airborn.web.Models
{
    public class BookDistancesList : List<BookDistances>
    {
        const int pressureAltitudeInterval = 1000; // the interval at which performance data is provided in the POH
        const int temperatureInterval = 10; // the internal at which which temperature data is provided in the POH

        public BookDistancesList()
        {
        }

        public List<BookDistances> TakeoffBookDistances
        {
            get
            {
                return FindAll(x => x.Scenario == Scenario.Takeoff);
            }
        }

        public List<BookDistances> LandingBookDistances
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

                var bookNumbers = new BookDistances(
                    scenario,
                    profile.PressureAltitude,
                    groundRoll.Temperature,
                    groundRoll.Distance,
                    clear50Ft.Distance
                );

                Add(bookNumbers);
            }
        }

        public BookDistances FindBookDistance(Scenario scenario, decimal pressureAltitude, decimal temperature)
        {
            List<BookDistances> bookDistances = FindAll(p => p.Scenario == scenario && p.PressureAltitude == pressureAltitude);

            if (bookDistances.Count == 0)
            {
                throw new Exception("No book distances found for scenario " + scenario + " and pressure altitude " + pressureAltitude);
            }

            return bookDistances[0];
        }

        public BookDistances GetInterpolatedBookDistance(Scenario scenario, decimal pressureAltitude, decimal temperature)
        {
            throw new NotImplementedException();
        }


    }
}