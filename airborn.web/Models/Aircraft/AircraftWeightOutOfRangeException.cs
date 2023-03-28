using System;

namespace Airborn.web.Models
{
    public class AircraftWeightOutOfRangeException : JsonPerformanceProfileNotFoundExceptionBase
    {

        public AircraftWeightOutOfRangeException(int minWeight, int maxWeight) :
            base(String.Format($"Aircraft weight must be less than : {minWeight} lbs and more than {maxWeight} lbs"))
        {
            MinWeight = minWeight;
            MaxWeight = maxWeight;
        }

        public int MinWeight
        {
            get; set;
        }

        public int MaxWeight
        {
            get; set;
        }

    }
}