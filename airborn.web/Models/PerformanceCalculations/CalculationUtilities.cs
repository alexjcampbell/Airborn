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

            if (difference > 360)
            {
                throw new ArgumentException("The difference between the two angles cannot be more than 360 degrees. (Distance was " + difference + ")");
            }

            return difference;

        }

        public static decimal ConvertDegreesToRadians(decimal degrees)
        {
            decimal radians = ((decimal)Math.PI / 180) * degrees;
            return radians;
        }

        public static decimal ConvertFahrenheitToCelcius(decimal fahrenheitTemperature)
        {
            return ((fahrenheitTemperature - 32) * 5 / 9);
        }

        public static decimal ConvertInchesOfMercuryToMillibars(decimal inchesOfMercury)
        {
            // once we've converted inches of mercury to hectopascals we can safely treat this
            // as an int (29.92 inches of mercury will end up being 1013 at which point the 
            // decimals don't matter)
            return inchesOfMercury * 33.8653074866m;
        }


        public static decimal CalculateISATemperatureForPressureAltitude(decimal pressureAltitude)
        {
            // ISA temperature decreases by 2 degrees Celsius for every 1000 feet
            // of altitude

            decimal isaTemperatureAtSeaLevel = 15;
            decimal temperatureLapseRate = 2;
            decimal altitudeInterval = 1000;

            return isaTemperatureAtSeaLevel - (temperatureLapseRate * (pressureAltitude / altitudeInterval));
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

        public static decimal CalculatePressureAltitudeAtFieldElevation(decimal QNH, int fieldElevation)
        {
            decimal standardPressure = 1013.25m;

            // folks usually round this to 30, but, fun fact, the formula is actually
            // 96 × ( T in kelvin) / QNH (in hPa)
            // at T = ISA (15C) and QNH = 1013.25 hPa, this is 27.3
            decimal lapseRate = 27.3m;

            return ((standardPressure - QNH) * lapseRate) + fieldElevation;
        }

        public static decimal CalculateDensityAltitudeAtAirport(decimal temperatureCelcius, decimal isaTemperature, decimal pressureAltitude)
        {
            return ((temperatureCelcius - isaTemperature) * 120) + pressureAltitude;
        }

    }

}