using System;

namespace Airborn.web.Models
{
    public class CalculationInterpolation
    {
        public ScenarioMode ScenarioMode;

        public decimal PressureAltitude;
        public decimal Weight;
        public decimal Temperature;



        public CalculationInterpolation(ScenarioMode scenarioMode, decimal pressureAltitude, decimal weight, decimal temperature)
        {
            ScenarioMode = scenarioMode;
            PressureAltitude = pressureAltitude;
            Weight = weight;
            Temperature = temperature;
        }

        private decimal CalculateInterpolatedDistanceForPressureAltitudeAndTemperature()
        {
            return 0;
        }

        private decimal CalculateInterpolatedDistanceForWeight()
        {
            return 0;
        }

        public static int GetLowerBoundForInterpolation(int value, int desiredInterval)
        {
            int lowerBound = value - (value % desiredInterval);

            return lowerBound;
        }
    }

}