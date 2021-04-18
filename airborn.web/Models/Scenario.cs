using System;

namespace Airborn.web.Models
{
    public class Scenario
    {

        private Scenario()
        {
        }

        public Scenario(Runway runway, Wind wind)
        {
            _runway = runway;
            _wind = wind;
        }

        private Runway _runway;
        public Runway Runway
        {
            get
            {
                return _runway;
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

        private int _fieldElevation;

        public int FieldElevation
        {
            get
            {
                return _fieldElevation;
            }
            set
            {
                _fieldElevation = value;
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
                return ((1013 - QNH) * 30) + FieldElevation;
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

        private int _runwayLength;

        public int RunwayLength
        {
            get
            {
                return _runwayLength;
            }
            set
            {
                _runwayLength = value;
            }
        }

        public double HeadwindComponent
        {
            get
            {
                return Wind.StrengthKts * Math.Cos(ConvertDegreesToRadians(WindRunwayAngularDifferenceMagnetic));
            }
        }

        public double CrosswindComponent
        {
            get
            {
                return Wind.StrengthKts * Math.Sin(ConvertDegreesToRadians(WindRunwayAngularDifferenceMagnetic));
            }
        }

        public int WindRunwayAngularDifferenceMagnetic
        {
            get
            {
                int difference = (int)AngularDifference(Runway.RunwayHeading.DirectionMagnetic, Wind.Direction.DirectionMagnetic);

                return difference;
            }
        }

        private static double AngularDifference(int angle1, int angle2)
        {
            double difference = (angle2 - angle1 + 180) % 360 - 180;
            return difference < -180 ? difference + 360 : difference;
        }

        private static double ConvertDegreesToRadians(int degrees)
        {
            double radians = (Math.PI / 180) * (double)degrees;
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

    public enum ScenarioMode
    {
        Takeoff_GroundRoll,
        Takeoff_50FtClearance,
        Landing_GroundRoll,
        Landing_50FtClearance
    }
}