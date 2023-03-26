using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;


namespace Airborn.web.Models
{

    public class PerformanceCalculator
    {

        private PerformanceCalculator()
        {
        }


        public PerformanceCalculator(
            AircraftType aircraftType,
            Airport airport,
            Wind wind,
            string path)
        {
            AircraftType = aircraftType;
            Airport = airport;
            _wind = wind;
            JsonPath = path;

        }

        public string JsonPath
        {
            get;
            set;
        }

        public Airport Airport
        {
            get;
        }

        public AircraftType AircraftType
        {
            get;
        }

        private List<Runway> _runways = new List<Runway>();

        public List<Runway> Runways
        {
            get
            {
                return _runways;
            }

        }

        private List<PerformanceCalculationResultForRunway> _results = new List<PerformanceCalculationResultForRunway>();

        public List<PerformanceCalculationResultForRunway> Results
        {
            get
            {
                return _results;
            }
        }

        private Wind _wind;

        public Wind Wind
        {
            get
            {
                return _wind;
            }
        }

        private int _temperatureCelcius;

        public int TemperatureCelcius
        {
            get
            {
                return _temperatureCelcius;
            }
            set
            {
                _temperatureCelcius = value;
            }
        }


        private int _qnh;

        public int QNH
        {
            get
            {
                return _qnh;
            }
            set
            {
                _qnh = value;
            }
        }

        public int PressureAltitude
        {
            get
            {
                return ((1013 - QNH) * 30) + Airport.FieldElevation;
            }
        }

        private double _aircraftWeight;

        public double AircraftWeight
        {
            get
            {
                return _aircraftWeight;
            }
            set
            {
                _aircraftWeight = value;
            }
        }

        public double DensityAltitude
        {
            get
            {
                return ((TemperatureCelcius - ISATemperature) * 120) + PressureAltitude;
            }
        }

        public double ISATemperature
        {
            get
            {
                return 15 - (2 * ((double)PressureAltitude / 1000));
            }
        }


        public void Calculate(string fullPath)
        {


            Aircraft aircraft = GetAircraft();

            fullPath = Path.Combine(JsonPath, aircraft.JsonFileName());

            JsonFile jsonFile = new JsonFile(fullPath);

            List<PerformanceCalculationResultForRunway> results = new List<PerformanceCalculationResultForRunway>();

            using (var db = new AirportDbContext())
            {


                foreach (Runway runway in
                    db.Runways_Flat.Where(runway => runway.Airport_Ident == Airport.Ident.ToUpper()).ToList<Runway>())
                {
                    Runway primaryRunway = runway;

                    primaryRunway.RunwayIdentifier = runway.RunwayIdentifier_Primary;

                    // RunwayIdentifer could be 10R or 28L so we use Substring to get only the first two characters

                    string runwayName = Regex.Replace(runway.RunwayIdentifier_Primary, @"\D+", "");


                    primaryRunway.RunwayHeading =
                        new Direction(
                            int.Parse(runwayName) * 10,
                            0
                            );

                    Runways.Add(primaryRunway);
                }
            }


            foreach (Runway runway in Runways)
            {
                PerformanceCalculationResultForRunway result = CalculatePerformanceForRunway(aircraft, jsonFile, runway);

                Results.Add(result);
            }

        }

        private Aircraft GetAircraft()
        {
            Aircraft aircraft;
            if (AircraftType == AircraftType.C172_SP)
            {
                aircraft = new C172_SP();
            }
            else if (AircraftType == AircraftType.SR22_G2)
            {
                aircraft = new SR22_G2();
            }
            else if (AircraftType == AircraftType.SR22T_G5)
            {
                aircraft = new SR22T_G5();
            }
            else
            {
                throw new ArgumentOutOfRangeException("AircraftType", AircraftType.ToString());
            }

            return aircraft;
        }

        private PerformanceCalculationResultForRunway CalculatePerformanceForRunway(Aircraft aircraft, JsonFile jsonFile, Runway runway)
        {
            PerformanceCalculationResultForRunway result =
                new PerformanceCalculationResultForRunway(runway, Wind);

            result.JsonFile = jsonFile;

            result.Takeoff_GroundRoll =
                aircraft.MakeTakeoffAdjustments
                (
                    result,
                    GetInterpolatedDistanceFromJson(ScenarioMode.Takeoff_GroundRoll, jsonFile, jsonFile.TakeoffProfiles)
                );

            result.Takeoff_50FtClearance =
                aircraft.MakeTakeoffAdjustments
                (
                    result,
                    GetInterpolatedDistanceFromJson(ScenarioMode.Takeoff_50FtClearance, jsonFile, jsonFile.TakeoffProfiles)
                );

            result.Landing_GroundRoll =
                aircraft.MakeLandingAdjustments
                (
                    result,
                    GetInterpolatedDistanceFromJson(ScenarioMode.Landing_GroundRoll, jsonFile, jsonFile.LandingProfiles)
                );

            result.Landing_50FtClearance =
                aircraft.MakeLandingAdjustments(
                    result,
                    GetInterpolatedDistanceFromJson(ScenarioMode.Landing_50FtClearance, jsonFile, jsonFile.LandingProfiles)
                );
            return result;
        }

        public double GetInterpolatedDistanceFromJson(ScenarioMode scenarioMode, JsonFile jsonFile, JsonPerformanceProfileList profiles)
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
            int lowerPressureAltidude = GetLowerBoundForInterpolation(PressureAltitude, 1000);
            int upperPressureAltidude = GetUpperBoundForInterpolation(PressureAltitude, 1000);
            int lowerTemperature = GetLowerBoundForInterpolation(TemperatureCelcius, 10);
            int upperTemperature = GetUpperBoundForInterpolation(TemperatureCelcius, 10);

            // then get the performance data for the lower temperature and the lower and higher
            // pressure altitude
            double distanceForLowerPressureAltitudeLowerTemp = jsonFile.GetPerformanceDataValueForConditions(profiles, this, scenarioMode, lowerPressureAltidude, lowerTemperature);
            double distanceForUpperPressureAltitudeLowerTemp = jsonFile.GetPerformanceDataValueForConditions(profiles, this, scenarioMode, upperPressureAltidude, lowerTemperature);

            // interpolate between the lower and higher pressure altitude for the lower temperature
            double distanceLowerTempInterpolated = Interpolate(
                distanceForLowerPressureAltitudeLowerTemp,
                distanceForUpperPressureAltitudeLowerTemp,
                PressureAltitude,
                1000 // pressure altitudes in the json comes in intervals of 1000
            );

            // then get the performance data for the higher temperature and the lower and higher
            // pressure altitude
            double distanceForLowerPressureAltitudeUpperTemp = jsonFile.GetPerformanceDataValueForConditions(profiles, this, scenarioMode, lowerPressureAltidude, upperTemperature);
            double distanceForUpperPressureAltitudeUpperTemp = jsonFile.GetPerformanceDataValueForConditions(profiles, this, scenarioMode, upperPressureAltidude, upperTemperature);

            // interpolate between the lower and higher pressure altitude for the higher temperature
            double distanceUpperTempInterpolated = Interpolate(
                distanceForLowerPressureAltitudeUpperTemp,
                distanceForUpperPressureAltitudeUpperTemp,
                PressureAltitude,
                1000 // pressure altitudes in the json comes in intervals of 1000
            );

            // interpolate between the lower and higher temperature
            double distanceInterpolated = Interpolate(
                (int)distanceLowerTempInterpolated,
                (int)distanceUpperTempInterpolated,
                TemperatureCelcius,
                10 // temperatures in the json comes in intervals of 10
            );

            System.Diagnostics.Debug.Write(
                $"Distance interpolated: {distanceInterpolated} for pressure altitude {PressureAltitude} and temperature {TemperatureCelcius} ");

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