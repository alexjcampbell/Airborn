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
        public abstract decimal MakeTakeoffAdjustments(PerformanceCalculationResult result, decimal takeoffDistance);
        public abstract decimal MakeLandingAdjustments(PerformanceCalculationResult result, decimal landingDistance);

        public abstract string JsonFileName_LowerWeight();
        public abstract string JsonFileName_HigherWeight();

        public abstract int GetLowerWeight();
        public abstract int GetHigherWeight();

        public static Aircraft GetAircraftFromAircraftType (AircraftType aircraftType)
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

        public decimal GetWeightInterpolationFactor(int weight)
        {
            if(weight <= 0)
            {
                throw new Exception("Weight must be greater than zero");
            }
            else if (weight < GetLowerWeight())
            {
                throw new AircraftWeightOutOfRangeException(GetLowerWeight(), GetHigherWeight());
            }
            else if (weight > GetHigherWeight())
            {
                throw new AircraftWeightOutOfRangeException(GetLowerWeight(), GetHigherWeight());
            }

            int lowerWeight = GetLowerWeight();
            int higherWeight = GetHigherWeight();

            decimal weightInterpolationFactor = (decimal)(weight - lowerWeight) / (decimal)(higherWeight - lowerWeight);

            return weightInterpolationFactor;
        }

    }
}