using System;
using System.Collections.Generic;

namespace Airborn.web.Models
{
    public static class CalculationUtilities
    {

        /// <summary>
        /// Returns the smallest angular difference between two angles
        /// If you give it 0 and 90, it will return you 90
        /// If you give it 0 and 270, it will return you -90
        /// </summary>
        public static decimal AngularDifference(int angle1, int angle2)
        {
            decimal difference = (angle2 - angle1 + 180) % 360 - 180;

            difference = difference < -180 ? difference + 360 : difference;

            if(difference > 360){
                throw new ArgumentException("The difference between the two angles cannot be more than 360 degrees. (Distance was " + difference + ")");
            }

            return difference;
            
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


        public static decimal CalculateISATemperatureForPressureAltitude(decimal pressureAltitude)
        {
                // ISA temperature decreases by 2 degrees Celsius for every 1000 feet
                // of altitude

                int isaTemperatureAtSeaLevel = 15;
                int temperatureLapseRate = 2;
                int altitudeInterval = 1000;

                return isaTemperatureAtSeaLevel - (temperatureLapseRate * ((decimal)pressureAltitude / altitudeInterval));
        }

        public static decimal CalculateHeadwindComponent(int windStrength, decimal windRunwayAngularDifferenceMagnetic)
        {
            return (decimal)(
                windStrength * Math.Cos((double)ConvertDegreesToRadians(windRunwayAngularDifferenceMagnetic))
            );            
        }


        public static decimal CalculateCrosswindComponent(int windStrength, decimal windRunwayAngularDifferenceMagnetic)
        {
            return Convert.ToDecimal(
                 windStrength * Math.Sin(
                    (double)CalculationUtilities.ConvertDegreesToRadians(windRunwayAngularDifferenceMagnetic)
                    )
            );
        }        

        public static decimal CalculateAngularDifferenceBetweenRunwayAndWind(int runwayHeading, int windDirection)
        {
            return AngularDifference(runwayHeading, windDirection);
        }

        public static decimal CalculatePressureAltitudeAtAirport(int QNH, int fieldElevation)
        {
            int standardPressure = 1013;
            int lapseRate = 30;

            return ((standardPressure - QNH) * lapseRate) + fieldElevation;
        }

        public static decimal CalculateDensityAltitudeAtAirport(int temperatureCelcius, decimal isaTemperature, decimal pressureAltitude)
        {
            return ((temperatureCelcius - isaTemperature) * 120) + pressureAltitude;
        }

    }

}