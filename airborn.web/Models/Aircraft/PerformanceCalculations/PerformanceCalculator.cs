using System;
using System.IO;

namespace Airborn.web.Models
{

    
    public static class PerformanceCalculator {
        public static PerformanceCalculation Calculate(Scenario scenario, AircraftType aircraftType, string path)
        {
            PerformanceCalculation calculation = new PerformanceCalculation();

            calculation.JsonFile = new JsonFile(path);

            Aircraft aircraft;

            if (aircraftType == AircraftType.C172_SP)
            {

                aircraft = new C172_SP(scenario, Path.Combine(path, "../C172_SP.json"));
            }
            else if (aircraftType == AircraftType.SR22_G2)
            {
                aircraft = new SR22_G2(scenario, Path.Combine(path, "../SR22_G2.json"));
            }
            else if (aircraftType == AircraftType.SR22T_G5)
            {
                aircraft = new SR22T_G5(scenario, Path.Combine(path, "../SR22T_G5.json"));
            }
            else
            {
                throw new ArgumentOutOfRangeException("AircraftType", aircraftType.ToString());
            }

            calculation.Takeoff_GroundRoll =
                aircraft.MakeTakeoffAdjustments
                (
                    GetInterpolatedDistanceFromJson(ScenarioMode.Takeoff_GroundRoll, scenario, calculation.JsonFile, calculation.JsonFile.TakeoffProfiles)
                );

            calculation.Takeoff_50FtClearance =
                aircraft.MakeTakeoffAdjustments
                (
                    GetInterpolatedDistanceFromJson(ScenarioMode.Takeoff_50FtClearance, scenario, calculation.JsonFile, calculation.JsonFile.TakeoffProfiles)
                );

            calculation.Landing_GroundRoll =
                aircraft.MakeLandingAdjustments
                (
                    GetInterpolatedDistanceFromJson(ScenarioMode.Landing_GroundRoll, scenario, calculation.JsonFile, calculation.JsonFile.LandingProfiles)
                );

            calculation.Landing_50FtClearance =
                aircraft.MakeLandingAdjustments(
                    GetInterpolatedDistanceFromJson(ScenarioMode.Landing_50FtClearance, scenario, calculation.JsonFile, calculation.JsonFile.LandingProfiles)
                );

            return calculation;
        }

        public static double GetInterpolatedDistanceFromJson(ScenarioMode scenarioMode, Scenario scenario, JsonFile jsonFile, JsonPerformanceProfileList profiles)
        {
            /*  The performance data in the JSON is provided by the manufacturer in 
                pressure altitude intervals of 1000 ft and temperature intervals of
                10 degrees celcius. This function looks bit complicated but all it 
                is doing is interpolating between the two

                i.e. if the scenario's temperature is 12 degrees and pressure altitude
                is 1500 ft, it will find the performance data for 10 and 20 degrees C
                and pressure altitude of 1000 and 2000 ft, and interpolate between them
            */

            // get the upper and lower bounds for pressure altitude and temperature
            // ie using the example below this will return 1000 ft and 2000 ft
            // and 10 and 20 degreses
            int lowerPressureAltidude = GetLowerBoundForInterpolation(scenario.PressureAltitude, 1000);
            int upperPressureAltidude = GetUpperBoundForInterpolation(scenario.PressureAltitude, 1000);
            int lowerTemperature = GetLowerBoundForInterpolation(scenario.TemperatureCelcius, 10);
            int upperTemperature = GetUpperBoundForInterpolation(scenario.TemperatureCelcius, 10);

            // then get the performance data for the lower temperature and the lower and higher
            // pressure altitude
            double distanceForLowerPressureAltitudeLowerTemp = jsonFile.GetPerformanceDataValueForConditions(profiles, scenario, scenarioMode, lowerPressureAltidude, lowerTemperature);
            double distanceForUpperPressureAltitudeLowerTemp = jsonFile.GetPerformanceDataValueForConditions(profiles, scenario, scenarioMode, upperPressureAltidude, lowerTemperature);

            // interpolate between the lower and higher pressure altitude for the lower temperature
            double distanceLowerTempInterpolated = Interpolate(
                distanceForLowerPressureAltitudeLowerTemp,
                distanceForUpperPressureAltitudeLowerTemp,
                scenario.PressureAltitude,
                1000 // pressure altitudes in the json comes in intervals of 1000
            );

            // then get the performance data for the higher temperature and the lower and higher
            // pressure altitude
            double distanceForLowerPressureAltitudeUpperTemp = jsonFile.GetPerformanceDataValueForConditions(profiles, scenario, scenarioMode, lowerPressureAltidude, upperTemperature);
            double distanceForUpperPressureAltitudeUpperTemp = jsonFile.GetPerformanceDataValueForConditions(profiles, scenario, scenarioMode, upperPressureAltidude, upperTemperature);

            // interpolate between the lower and higher pressure altitude for the higher temperature
            double distanceUpperTempInterpolated = Interpolate(
                distanceForLowerPressureAltitudeUpperTemp,
                distanceForUpperPressureAltitudeUpperTemp,
                scenario.PressureAltitude,
                1000 // pressure altitudes in the json comes in intervals of 1000
            );

            // interpolate between the lower and higher temperature
            double distanceInterpolated = Interpolate(
                (int)distanceLowerTempInterpolated,
                (int)distanceUpperTempInterpolated,
                scenario.TemperatureCelcius,
                10 // temperatures in the json comes in intervals of 10
            );

            System.Diagnostics.Debug.Write(
                $"Distance interpolated: {distanceInterpolated} for pressure altitude {scenario.PressureAltitude} and temperature {scenario.TemperatureCelcius} ");

            return distanceInterpolated;


        }


        public static double CalculateInterpolationFactor(int value, int x1, int x2)
        {
            if (value < 0)
            {
                throw new PressureAltitudePerformanceProfileNotFoundException(value);
            }

            if (value < x1) { throw new ArgumentOutOfRangeException(); }
            if (value > x2) { throw new ArgumentOutOfRangeException(); }

            double interpolationFactor = (double)(value - x1) / (double)(x2 - x1);

            return interpolationFactor;
        }

        public static int GetLowerBoundForInterpolation(int value, int desiredInterval)
        {
            int lowerBound = value - (value % desiredInterval);

            return lowerBound;
        }

        public static int GetUpperBoundForInterpolation(int value, int desiredInterval)
        {
            int lowerBound = value - (value % desiredInterval);
            int upperBound = lowerBound + desiredInterval;

            return upperBound;
        }

        public static (int, int) GetUpperAndLowBoundsForInterpolation(int value, int desiredInterval)
        {
            return (GetLowerBoundForInterpolation(value, desiredInterval), GetUpperBoundForInterpolation(value, desiredInterval));
        }

        public static double Interpolate(double lowerValue, double upperValue, int valueForInterpolation, int desiredInterval)
        {
            int lowerInterpolation = GetLowerBoundForInterpolation(valueForInterpolation, desiredInterval);
            int upperInterpolation = GetUpperBoundForInterpolation(valueForInterpolation, desiredInterval);

            double interpolationFactor = CalculateInterpolationFactor(valueForInterpolation, lowerInterpolation, upperInterpolation);

            double interpolatedValue =
                (upperValue - lowerValue)
                *
                interpolationFactor
                + lowerValue;

            return interpolatedValue;

        }

    }
}