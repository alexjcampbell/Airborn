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
            get;
            set;
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
}