using System;

namespace Airborn.web.Models
{
    public class BookDistances
    {
        private BookDistances()
        {
        }

        public BookDistances(Scenario scenario, decimal pressureAltitude, decimal temperature, decimal groundRoll, decimal distanceToClear50Ft)
        {
            Scenario = scenario;
            PressureAltitude = pressureAltitude;
            Temperature = temperature;
            GroundRoll = groundRoll;
            DistanceToClear50Ft = distanceToClear50Ft;
        }

        public Scenario Scenario;

        const int pressureAltitudeInterval = 1000; // the interval at which performance data is provided in the POH
        const int temperatureInterval = 10; // the internal at which which temperature data is provided in the POH

        public decimal PressureAltitude
        {
            get;
            private set;
        }

        public decimal Temperature
        {
            get;
            private set;
        }

        public decimal GroundRoll
        {
            get;
            private set;
        }

        public decimal DistanceToClear50Ft
        {
            get;
            private set;
        }
    }
}