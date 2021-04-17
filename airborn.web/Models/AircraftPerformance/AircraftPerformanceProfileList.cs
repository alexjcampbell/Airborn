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

                AircraftPerformanceProfile resultList = null;

                switch(scenarioMode)
                {
                    case ScenarioMode.Takeoff_50FtClearance:
                        resultList = FindByPressureAltitude(ScenarioMode.Takeoff_50FtClearance, pressureAltitude);

                        if(resultList == null)
                        {
                            throw new PressureAltitudePerformanceProfileNotFoundException(scenario.PressureAltitude);
                        }

                        return resultList.Clear50FtObstacle.FindByTemperature(temperatureCelcius);

                    case ScenarioMode.Takeoff_GroundRoll:
                        resultList = FindByPressureAltitude(ScenarioMode.Takeoff_GroundRoll, pressureAltitude);

                        if(resultList == null)
                        {
                            throw new PressureAltitudePerformanceProfileNotFoundException(scenario.PressureAltitude);
                        }

                        return resultList.GroundRoll.FindByTemperature(temperatureCelcius);                     

                    case ScenarioMode.Landing_50FtClearance:
                        resultList = FindByPressureAltitude(ScenarioMode.Landing_50FtClearance, pressureAltitude);

                        if(resultList == null)
                        {
                            throw new PressureAltitudePerformanceProfileNotFoundException(scenario.PressureAltitude);
                        }

                        return resultList.Clear50FtObstacle.FindByTemperature(temperatureCelcius);

                    case ScenarioMode.Landing_GroundRoll:

                        if(resultList == null)
                        {
                            throw new PressureAltitudePerformanceProfileNotFoundException(scenario.PressureAltitude);
                        }

                        resultList =  FindByPressureAltitude(ScenarioMode.Landing_GroundRoll, pressureAltitude);

                        return resultList.GroundRoll.FindByTemperature(temperatureCelcius);                     
                }

                throw new ArgumentException("No performance data found for pressure altitude: " + pressureAltitude + " and temperature: " + temperatureCelcius);
            }
        }
}