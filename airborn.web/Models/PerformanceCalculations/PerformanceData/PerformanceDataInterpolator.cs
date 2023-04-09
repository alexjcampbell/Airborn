using System;

namespace Airborn.web.Models
{

    public class PeformanceDataInterpolator
    {

        private PeformanceDataInterpolator()
        {

        }

        public PeformanceDataInterpolator(
            Scenario scenario,
            decimal actualPressureAltitude,
            decimal actualTemperature,
            JsonFile jsonFile
        )
        {
            ActualPressureAltitude = actualPressureAltitude;
            ActualTemperature = actualTemperature;
            Scenario = scenario;
            JsonFile = jsonFile;
            _performanceDataList = new BookPerformanceDataList();

        }

        ///  <summary>
        ///  This constructor is used for testing purposes only - it allows us to pass in a list of
        ///  BookPerformanceData objects instead of a JsonFile object.
        ///  </summary>
        public PeformanceDataInterpolator(
            Scenario scenario,
            decimal actualPressureAltitude,
            decimal actualTemperature,
            BookPerformanceDataList performanceDataList
        )
        {
            ActualPressureAltitude = actualPressureAltitude;
            ActualTemperature = actualTemperature;
            Scenario = scenario;
            _performanceDataList = performanceDataList;
        }

        public JsonFile JsonFile
        {
            get;
            private set;
        }

        private BookPerformanceDataList _performanceDataList;

        public decimal ActualPressureAltitude
        {
            get;
            private set;
        }

        public decimal ActualTemperature
        {
            get;
            private set;
        }

        public Scenario Scenario
        {
            get;
            private set;
        }

        private decimal _lowerPressureAltitude;
        private decimal _upperPressureAltitude;

        private decimal _lowerTemperature;
        private decimal _upperTemperature;


        private const int _pressureAltitudeInterval = 1000; // the interval at which performance data is provided in the POH
        private const int _temperatureInterval = 10; // the internal at which which temperature data is provided in the POH




        public InterpolatedPerformanceData GetInterpolatedBookDistance(Scenario scenario)
        {
            // in tests we prepolate data here without using JsonFile, so we only want to do this
            // when JsonFile is not null
            if (JsonFile != null)
            {
                _performanceDataList.PopulateFromJson(JsonFile);
            }

            _lowerPressureAltitude = PerformanceCalculator.GetLowerBoundForInterpolation(
                (int)ActualPressureAltitude,
                _pressureAltitudeInterval);
            _upperPressureAltitude = _lowerPressureAltitude + _pressureAltitudeInterval;

            _lowerTemperature = PerformanceCalculator.GetLowerBoundForInterpolation((int)ActualTemperature, _temperatureInterval);
            _upperTemperature = _lowerTemperature + _temperatureInterval;

            BookPerformanceData lower =
                _performanceDataList.FindBookDistance(Scenario, _lowerPressureAltitude, _lowerTemperature);
            BookPerformanceData upper =
                _performanceDataList.FindBookDistance(Scenario, _upperPressureAltitude, _lowerTemperature);


            return InterpolateBetweenPerformanceData(
                lower,
                upper
                );

        }

        public InterpolatedPerformanceData InterpolateBetweenPerformanceData(
            BookPerformanceData lower,
            BookPerformanceData upper)
        {
            // lowerPressureAltitudePerformanceData will be like: pressure altitude 1000, temperature 10, groundroll 100
            // upperPressureAltitudePerformanceData will be like: pressure altitude 1000, temperature 20, groundroll 120
            // lowerTemperaturePerformanceData will be like: pressure altitude 2000, temperature 10, groundroll 130
            // upperTemperaturePerformanceData will be like: pressure altitude 2000, temperature 20, groundroll 140
            // actualPressureAltitude will be like 1500
            // actualTemperature will be like 15
            // we need to do a four-way interpolation here


            return Interpolate(lower, upper, ActualPressureAltitude, ActualTemperature); ;

        }


        private static void ValidatePressureAlitudeAndTemperatureDifferences(
            BookPerformanceData upper,
            BookPerformanceData lower)
        {

        }

        public InterpolatedPerformanceData InterpolateByPressureAltitude(
            BookPerformanceData lowerPressureAltitudePerformanceData,
            BookPerformanceData upperPresssureAltitudePerformanceData)
        {
            // bookData1 will be like: pressure altitude 1000, temperature 10
            // bookData2 will be like: pressure altitude 2000, temperature 10
            // actualPressureAltitude will be like 1500
            // we need to do a two-way interpolation here

            decimal interpolatedGroundRoll = PerformanceCalculator.Interpolate(
                lowerPressureAltitudePerformanceData.GroundRoll,
                upperPresssureAltitudePerformanceData.GroundRoll,
                (int)ActualPressureAltitude,
                _pressureAltitudeInterval
                );

            decimal interpolatedDistanceToClear50Ft = PerformanceCalculator.Interpolate(
                lowerPressureAltitudePerformanceData.DistanceToClear50Ft,
                upperPresssureAltitudePerformanceData.DistanceToClear50Ft,
                (int)ActualPressureAltitude,
                _pressureAltitudeInterval
            );

            return new InterpolatedPerformanceData(
                lowerPressureAltitudePerformanceData.Scenario,
                ActualPressureAltitude,
                ActualTemperature,
                interpolatedGroundRoll,
                interpolatedDistanceToClear50Ft
                );
        }

        public InterpolatedPerformanceData InterpolateByTemperature(
            BookPerformanceData lowerTemperaturePerformanceData,
            BookPerformanceData upperTemperaturePerformanceData)
        {
            // bookData1 will be like: pressure altitude 1000, temperature 10
            // bookData2 will be like: pressure altitude 1000, temperature 20
            // actualTemperature will be like 15
            // we need to do a two-way interpolation here

            decimal interpolatedGroundRoll = PerformanceCalculator.Interpolate(
                lowerTemperaturePerformanceData.GroundRoll,
                upperTemperaturePerformanceData.GroundRoll,
                (int)ActualTemperature,
                _temperatureInterval
                );

            decimal interpolatedDistanceToClear50Ft = PerformanceCalculator.Interpolate(
                lowerTemperaturePerformanceData.DistanceToClear50Ft,
                upperTemperaturePerformanceData.DistanceToClear50Ft,
                (int)ActualTemperature,
                _temperatureInterval
            );

            return new InterpolatedPerformanceData(
                lowerTemperaturePerformanceData.Scenario,
                ActualPressureAltitude,
                ActualTemperature,
                interpolatedGroundRoll,
                interpolatedDistanceToClear50Ft
                );
        }

        public static InterpolatedPerformanceData Interpolate(PerformanceData p1, PerformanceData p2, decimal pa, decimal temp)
        {
            decimal pressureAltitudeDiff = 1000;
            decimal tempDiff = 10;
            decimal paRatio = (pa - p1.PressureAltitude) / pressureAltitudeDiff;
            decimal tempRatio = (temp - p1.Temperature) / tempDiff;
            decimal groundRoll = p1.GroundRoll + (p2.GroundRoll - p1.GroundRoll) * paRatio * tempRatio;
            decimal distanceToClear50Ft = p1.DistanceToClear50Ft + (p2.DistanceToClear50Ft - p1.DistanceToClear50Ft) * paRatio * tempRatio;
            return new InterpolatedPerformanceData(p1.Scenario, pa, temp, groundRoll, distanceToClear50Ft);
        }
    }
}