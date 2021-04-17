using System;
using System.Collections.Generic;
using System.Linq;

namespace Airborn.web.Models
{
          public class AircraftPerformanceProfileList : List<AircraftPerformanceProfile>
        {
            public AircraftPerformanceProfile FindByPressureAltitude(ScenarioMode scenarioMode, int pressureAltitude)
            {
                string type = "";

                switch (scenarioMode) {
                    case ScenarioMode.Takeoff_50FtClearance:
                    case ScenarioMode.Takeoff_GroundRoll:
                        type = "Takeoff";
                        break;
                    case ScenarioMode.Landing_50FtClearance:
                    case ScenarioMode.Landing_GroundRoll:
                        type = "Landing";
                        break;                        
                }

                return this.Find (
                    p => 
                    p.PressureAltitude == pressureAltitude
                    &
                    p.Type == type
                    );
            }


            public double GetPerformanceData(Scenario scenario, ScenarioMode scenarioMode, int pressureAltitude, int temperatureCelcius)
            {
                // check to make sure that we actually have data for this Pressure Altitude and Temperature
                int maxPressureAltitudeAvailable = this.Max(p => p.PressureAltitude);
                if(pressureAltitude > maxPressureAltitudeAvailable)
                {
                    throw new PressureAltitudePerformanceProfileNotFoundException(scenario.PressureAltitude);
                }

                switch(scenarioMode)
                {
                    case ScenarioMode.Takeoff_50FtClearance:
                        return FindByPressureAltitude(ScenarioMode.Takeoff_50FtClearance, pressureAltitude).Clear50FtObstacle.FindByTemperature(temperatureCelcius);
                    case ScenarioMode.Takeoff_GroundRoll:
                        return FindByPressureAltitude(ScenarioMode.Takeoff_GroundRoll, pressureAltitude).GroundRoll.FindByTemperature(temperatureCelcius);                     
                    case ScenarioMode.Landing_50FtClearance:
                        return FindByPressureAltitude(ScenarioMode.Landing_50FtClearance, pressureAltitude).Clear50FtObstacle.FindByTemperature(temperatureCelcius);
                    case ScenarioMode.Landing_GroundRoll:
                        return FindByPressureAltitude(ScenarioMode.Landing_GroundRoll, pressureAltitude).GroundRoll.FindByTemperature(temperatureCelcius);                     


                }

                throw new ArgumentException("No performance data found for pressure altitude: " + pressureAltitude + " and temperature: " + temperatureCelcius);
            }
        }
}