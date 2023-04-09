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

        public JsonFile()
        {
        }

        public JsonFile(string path)
        {
            LoadJson(path);
        }

        protected JsonPerformanceProfileList _takeoffProfiles;

        public virtual JsonPerformanceProfileList TakeoffProfiles
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

        public virtual JsonPerformanceProfileList LandingProfiles
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
            ScenarioMode scenarioMode,
            decimal actualPressureAltitude,
            int pressureAltitudeToFind,
            int temperatureCelcius
        )
        {

            // check to make sure that we actually have data for this Pressure Altitude and Temperature
            int maxPressureAltitudeAvailable = profiles.Max(p => p.PressureAltitude);
            if (pressureAltitudeToFind > maxPressureAltitudeAvailable)
            {
                throw new PressureAltitudePerformanceProfileNotFoundException(actualPressureAltitude);
            }

            JsonPerformanceProfile profile =
                FindByPressureAltitude(profiles, scenarioMode, pressureAltitudeToFind);

            if (profile == null)
            {
                throw new PressureAltitudePerformanceProfileNotFoundException(actualPressureAltitude);
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

            throw new ArgumentException($"No performance data found for pressure altitude: {pressureAltitudeToFind} and temperature: {temperatureCelcius}");
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