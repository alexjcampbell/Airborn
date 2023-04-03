using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Airborn.web.Models
{
    public class JsonFile
    {

        private JsonFile()
        {
        }

        public JsonFile(string path)
        {
            LoadJson(path);
        }

        protected JsonPerformanceProfileList _takeoffProfiles;

        public JsonPerformanceProfileList TakeoffProfiles
        {
            get
            {
                return _takeoffProfiles;
            }
            set
            {
                _takeoffProfiles = value;
            }
        }

        protected JsonPerformanceProfileList _landingProfiles;

        public JsonPerformanceProfileList LandingProfiles
        {
            get
            {
                return _landingProfiles;
            }
            set
            {
                _landingProfiles = value;
            }
        }

        public void LoadJson(string path)
        {


            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore
            };

            StreamReader sr = new StreamReader(path);

            var json = sr.ReadToEnd();

            JsonConvert.PopulateObject(json, this); ;


        }

        public decimal GetPerformanceDataValueForConditions(
            JsonPerformanceProfileList profiles,
            PerformanceCalculator calculator,
            ScenarioMode scenarioMode,
            int pressureAltitude,
            int temperatureCelcius
        )
        {
            System.Diagnostics.Debug.WriteLine("profiles null " + profiles is null);
            System.Diagnostics.Debug.WriteLine("profiles length is " + profiles.Count.ToString());

            // check to make sure that we actually have data for this Pressure Altitude and Temperature
            int maxPressureAltitudeAvailable = profiles.Max(p => p.PressureAltitude);
            if (pressureAltitude > maxPressureAltitudeAvailable)
            {
                throw new PressureAltitudePerformanceProfileNotFoundException(calculator.PressureAltitude);
            }

            JsonPerformanceProfile profile =
                FindByPressureAltitude(profiles, scenarioMode, pressureAltitude);

            if (profile == null)
            {
                throw new PressureAltitudePerformanceProfileNotFoundException(calculator.PressureAltitude);
            }

            switch (scenarioMode)
            {
                case ScenarioMode.Takeoff_50FtClearance:
                case ScenarioMode.Landing_50FtClearance:

                    return profile.Clear50FtObstacle.FindByTemperature(temperatureCelcius);

                case ScenarioMode.Takeoff_GroundRoll:
                case ScenarioMode.Landing_GroundRoll:
                    return profile.GroundRoll.FindByTemperature(temperatureCelcius);

            }

            throw new ArgumentException($"No performance data found for pressure altitude: {pressureAltitude} and temperature: {temperatureCelcius}");
        }

        public JsonPerformanceProfile FindByPressureAltitude(JsonPerformanceProfileList profiles, ScenarioMode scenarioMode, int pressureAltitude)
        {
            string type = "";

            switch (scenarioMode)
            {
                case ScenarioMode.Takeoff_50FtClearance:
                case ScenarioMode.Takeoff_GroundRoll:
                    type = "Takeoff";
                    break;
                case ScenarioMode.Landing_50FtClearance:
                case ScenarioMode.Landing_GroundRoll:
                    type = "Landing";
                    break;
            }

            return profiles.Find(
                p =>
                p.PressureAltitude == pressureAltitude
                &
                p.Type == type
                );
        }

    }
}