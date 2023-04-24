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
        public abstract decimal MakeTakeoffAdjustments(PerformanceCalculationResultForRunway result, decimal takeoffDistance);
        public abstract decimal MakeLandingAdjustments(PerformanceCalculationResultForRunway result, decimal landingDistance);

        public abstract string JsonFileName_LowerWeight();
        public abstract string JsonFileName_HigherWeight();

        public abstract int GetLowerWeight();
        public abstract int GetHigherWeight();

        public static AircraftType GetAircraftTypeFromAircraftTypeString(string aircraftType)
        {

            switch (aircraftType)
            {
                case "SR22_G2":
                    return AircraftType.SR22_G2;
                case "SR22T_G5":
                    return AircraftType.SR22T_G5;
                case "C172_SP":
                    return AircraftType.C172_SP;
                default:
                    throw new Exception("Unknown aircraft type");
            }

        }

        public static Aircraft GetAircraftFromAircraftType(AircraftType aircraftType)
        {

            switch (aircraftType)
            {
                case AircraftType.SR22_G2:
                    return new SR22_G2();
                case AircraftType.SR22T_G5:
                    return new SR22T_G5();
                case AircraftType.C172_SP:
                    return new C172_SP();
                default:
                    throw new Exception("Unknown aircraft type");
            }

        }



    }
}