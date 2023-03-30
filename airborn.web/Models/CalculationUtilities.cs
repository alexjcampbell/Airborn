using System;
using System.Collections.Generic;

namespace Airborn.web.Models
{
    public static class CalculationUtilities
    {

        public static decimal AngularDifference(int angle1, int angle2)
        {
            decimal difference = (angle2 - angle1 + 180) % 360 - 180;
            return difference < -180 ? difference + 360 : difference;
        }

        public static decimal ConvertDegreesToRadians(decimal degrees)
        {
            decimal radians = ((decimal)Math.PI / 180) * (decimal)degrees;
            return radians;
        }

        public static int ConvertFahrenheitToCelcius(decimal? fahrenheitTemperature)
        {
            return (int)((fahrenheitTemperature - 32) * 5 / 9);
        }

        public static int ConvertInchesOfMercuryToMillibars(decimal? inchesOfMercury)
        {
            // once we've converted inches of mercury to hectopascals we can safely treat this
            // as an int (29.92 inches of mercury will end up being 1013 at which point the 
            // decimals don't matter)
            return (int)(inchesOfMercury * 33.8639M);
        }

    }

}