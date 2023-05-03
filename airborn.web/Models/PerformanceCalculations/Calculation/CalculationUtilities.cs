using System;
using System.Collections.Generic;

namespace Airborn.web.Models
{
    /// <summary>
    /// This class contains a number of utility methods for performing calculations on wind, temperature, pressure, etc.
    /// </summary>
    public static class CalculationUtilities
    {

        /// <summary>
        /// Returns the smallest angular difference between two angles
        /// If you give it 0 and 90, it will return you 90
        /// If you give it 0 and 270, it will return you -90
        /// </summary>
        public static double SmallestAngularDifference(int angle1, int angle2)
        {
            // the smallest angular difference between two angles is the difference between the two angles
            // modulo 360, minus 180

            double difference = (angle2 - angle1 + 180) % 360 - 180;

            // if the difference is greater than 180, subtract 360 from it so that we never return a value greater than 180
            difference = difference < -180 ? difference + 360 : difference;

            if (difference > 360)
            {
                throw new ArgumentException("The difference between the two angles cannot be more than 360 degrees. (Distance was " + difference + ")");
            }

            return difference;

        }

        /// <summary>
        /// Returns the angular difference between the runway and the wind
        /// </summary>
        /// <param name="runwayHeading">The heading of the runway</param>
        /// <param name="windDirection">The direction of the wind</param>
        public static double AngularDifferenceBetweenRunwayAndWind(int runwayHeading, int windDirection)
        {
            // the angular difference between the runway and the wind is the smallest angular difference between the two
            return SmallestAngularDifference(runwayHeading, windDirection);
        }

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        /// <param name="degrees">The number of degrees to convert</param>
        public static double DegreesToRadians(double degrees)
        {
            // 180 degrees = pi radians
            // 1 degree = pi / 180 radians
            // so, x degrees = (pi / 180) * x radians
            // or, in this case , x radians = (180 / pi) * x degrees

            double radians = (Math.PI / 180) * degrees;
            return radians;
        }

        /// <summary>
        /// Converts a temperature in fahrenheit to celcius
        /// </summary>
        /// <param name="fahrenheitTemperature">The temperature in fahrenheit to convert</param>
        public static double FahrenheitToCelcius(double fahrenheitTemperature)
        {
            return ((fahrenheitTemperature - 32) * 5 / 9);
        }

        /// <summary>
        /// Converts inches of mercury to millibars
        /// </summary>
        /// <param name="inchesOfMercury">The pressure in inches of mercury to convert</param>
        public static double InchesOfMercuryToMillibars(double inchesOfMercury)
        {
            // 1 inch of mercury = 33.8653074866 millibars
            return inchesOfMercury * 33.8653074866f;
        }


        /// <summary>
        /// Returns the ISA temperature at a given pressure altitude
        /// </summary>
        /// <param name="pressureAltitude">The pressure altitude to calculate the ISA temperature at</param>
        public static double ISATemperatureForPressureAltitude(double pressureAltitude)
        {

            double isaTemperatureAtSeaLevel = 15; // 15 degrees Celsius
            double temperatureLapseRate = 2; // 2 degrees Celsius per 1000 feet
            double altitudeInterval = 1000;

            // ISA temperature starts at 15 degrees Celsius at sea level, 
            // and decreases by 2 degrees Celsius for every 1000 feet of altitude
            return isaTemperatureAtSeaLevel - (temperatureLapseRate * (pressureAltitude / altitudeInterval));
        }

        /// <summary>
        /// Returns the headwind component for a given wind strength and angular difference between the runway and the wind
        /// </summary>
        /// <param name="windStrength">The strength of the wind</param>
        /// <param name="windRunwayAngularDifferenceMagnetic">The angular difference between the runway and the wind</param>
        public static double HeadwindComponent(int windStrength, double windRunwayAngularDifferenceMagnetic)
        {
            // headwind component = wind strength * cos(the angular difference between the runway and the wind)

            return (
                windStrength * Math.Cos(DegreesToRadians(windRunwayAngularDifferenceMagnetic))
            );
        }


        /// <summary>
        /// Returns the crosswind component for a given wind strength and angular difference between the runway and the wind
        /// </summary>
        /// <param name="windStrength">The strength of the wind</param>
        /// <param name="windRunwayAngularDifferenceMagnetic">The angular difference between the runway and the wind</param>
        public static double CrosswindComponent(int windStrength, double windRunwayAngularDifferenceMagnetic)
        {
            // crosswind component = wind strength * sin(the angular difference between the runway and the wind)

            return Convert.ToDouble(
                 windStrength * Math.Sin(
                    CalculationUtilities.DegreesToRadians(windRunwayAngularDifferenceMagnetic)
                    )
            );
        }

        /// <summary>
        /// Returns the pressure altitude at a given field elevation
        /// </summary>
        /// <param name="QNH">The QNH at the airport</param>
        /// <param name="fieldElevation">The elevation of the airport</param>
        public static double PressureAltitudeAtFieldElevation(double QNH, int fieldElevation)
        {
            double standardPressure = 1013.25f;

            // folks usually round this to 30, but, fun fact, the formula is actually
            // 96 × ( T in kelvin) / QNH (in hPa)
            // at T = ISA (15C) and QNH = 1013.25 hPa, this is 27.3
            double lapseRate = 27.3f;

            return ((standardPressure - QNH) * lapseRate) + fieldElevation;
        }

        /// <summary>
        /// Returns the density altitude at a given airport
        /// </summary>
        /// <param name="temperatureCelcius">The temperature at the airport in celcius</param>
        /// <param name="isaTemperature">The ISA temperature at the airport</param>
        /// <param name="pressureAltitude">The pressure altitude at the airport</param>
        public static double DensityAltitudeAtAirport(double temperatureCelcius, double isaTemperature, double pressureAltitude)
        {
            // density altitude is the pressure altitude adjusted for temperature
            // the formula is:
            // density altitude = pressure altitude + (120 * (temperature - ISA temperature))

            double densityAltitudeInterval = 120;

            return ((temperatureCelcius - isaTemperature) * densityAltitudeInterval) + pressureAltitude;
        }

    }

}