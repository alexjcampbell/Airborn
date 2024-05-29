using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Airborn.web.Models
{
    public abstract class Aircraft
    {

        public Aircraft()
        {
        }



        // the implementing aircraft class must implement the POH rules for adjusting
        // the performance data for wind, slope, surface etc
        public abstract Distance MakeTakeoffAdjustments(
            CalculationResultForRunway result,
            Distance unadjustedTakeoffDistance,
            PerformanceCalculationLog.LogItem logItem,
            AirconOptions airconOptions
            );
        public abstract Distance MakeLandingAdjustments(
            CalculationResultForRunway result,
            Distance unadjustedlandingDistance,
            PerformanceCalculationLog.LogItem logItem,
            AirconOptions airconOptions
            );

        public abstract string JsonFileName_LowestWeight
        {
            get;
        }

        public abstract string JsonFileName_HighestWeight
        {
            get;
        }

        public abstract int LowestPossibleWeight
        {
            get;
        }
        public abstract int HighestPossibleWeight
        {
            get;
        }

        public abstract int LowestPossibleTemperature
        {
            get;
        }

        public abstract int HighestPossibleTemperature
        {
            get;
        }

        public abstract int LowestPossiblePressureAltitude
        {
            get;
        }

        public abstract int HighestPossiblePressureAltitude
        {
            get;
        }

        public abstract bool HasAirconOption
        {
            get;
        }

        public abstract string AircraftTypeString
        {
            get;
        }

        public static AircraftType? GetAircraftTypeFromAircraftTypeString(string aircraftType)
        {

            switch (aircraftType)
            {
                case "SR22_G2":
                    return AircraftType.SR22_G2;
                case "SR22T_G5":
                    return AircraftType.SR22T_G5;
                case "SR22T_G6":
                    return AircraftType.SR22T_G6;
                case "C172_SP":
                    return AircraftType.C172_SP;
                case "C182_T":
                    return AircraftType.C182_T;
                default:
                    return null;
            }

        }

        public static Aircraft GetAircraftFromAircraftType(string aircraftType)
        {
            return GetAircraftFromAircraftType(GetAircraftTypeFromAircraftTypeString(aircraftType).Value);
        }

        public static Aircraft GetAircraftFromAircraftType(AircraftType aircraftType)
        {

            switch (aircraftType)
            {
                case AircraftType.SR22_G2:
                    return new SR22_G2();
                case AircraftType.SR22T_G5:
                    return new SR22T_G5();
                case AircraftType.SR22T_G6:
                    return new SR22T_G6();
                case AircraftType.C172_SP:
                    return new C172_SP();
                case AircraftType.C182_T:
                    return new C182_T();
                default:
                    throw new Exception("Unknown aircraft type");
            }

        }
    }
}