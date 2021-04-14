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

        public Scenario(int runwayMagneticHeading, int magneticVariation, int windDirectionMagnetic, int windStrength)
        {
            _runway = new Runway(Direction.FromMagnetic(runwayMagneticHeading, magneticVariation));
            _wind = new Wind(Direction.FromMagnetic(windDirectionMagnetic, magneticVariation), windStrength);

        }


        private Runway _runway;
        public Runway Runway {
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
            set{
                _temperatureCelcius = value;
            }
         }
         
         private int _pressureAltitude;

         public int PressureAltitude
         {
            get
            {
                return _pressureAltitude;
            }
            set
            {
                _pressureAltitude = value;
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

        private static double AngularDifference( int angle1, int angle2 )
        {
            double difference = ( angle2 - angle1 + 180 ) % 360 - 180;
            return difference < -180 ? difference + 360 : difference;
        }       

        private static double ConvertDegreesToRadians(int degrees)
        {
            double radians = (Math.PI / 180) * (double)degrees;
            return radians;
        }

    }

    public enum ScenarioMode {
        Takeoff_GroundRoll,
        Takeoff_50FtClearance,
        Landing_GroundRoll,
        Landing_50FtClearance
    }
}