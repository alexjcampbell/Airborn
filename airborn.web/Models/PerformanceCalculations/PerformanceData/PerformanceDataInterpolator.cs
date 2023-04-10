using System;
using System.IO;
using System.Collections.Generic;

namespace Airborn.web.Models
{


    /// <summary>
    /// This class is used to interpolate performance data from the POH, interpolating between
    /// pressure altitude, temperature, and aircraft weight.
    /// </summary>
    public class PeformanceDataInterpolator
    {

        private PeformanceDataInterpolator()
        {

        }

        public PeformanceDataInterpolator(
            Scenario scenario,
            decimal actualPressureAltitude,
            decimal actualTemperature,
            decimal aircraftActualWeigth,
            JsonFile jsonFileLowerWeight,
            JsonFile jsonFileHigherWeight
        )
        {
            ActualPressureAltitude = actualPressureAltitude;
            ActualTemperature = actualTemperature;
            AircraftActualWeigth = aircraftActualWeigth;
            Scenario = scenario;
            JsonFileLowerWeight = jsonFileLowerWeight;
            JsonFileHigherWeight = jsonFileHigherWeight;
            _performanceDataList = new BookPerformanceDataList(jsonFileLowerWeight.AircraftWeight, jsonFileHigherWeight.AircraftWeight);

        }

        ///  <summary>
        ///  This constructor is used for testing purposes only - it allows us to pass in a list of
        ///  BookPerformanceData objects instead of a JsonFile object.
        ///  </summary>
        public PeformanceDataInterpolator(
            Scenario scenario,
            decimal actualPressureAltitude,
            decimal actualTemperature,
            decimal actualWeight,
            BookPerformanceDataList performanceDataList
        )
        {
            ActualPressureAltitude = actualPressureAltitude;
            ActualTemperature = actualTemperature;
            AircraftActualWeigth = actualWeight;
            Scenario = scenario;
            _performanceDataList = performanceDataList;
        }

        public JsonFile JsonFileLowerWeight
        {
            get;
            private set;
        }

        public JsonFile JsonFileHigherWeight
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

        public decimal AircraftLowerWeight
        {
            get;
            private set;
        }

        public decimal AircraftHigherWeight
        {
            get;
            private set;
        }

        public decimal AircraftActualWeigth
        {
            get;
            private set;
        }

        private const int _pressureAltitudeInterval = 1000; // the interval at which performance data is provided in the POH
        private const int _temperatureInterval = 10; // the internal at which which temperature data is provided in the POH

        public InterpolatedPerformanceData GetInterpolatedBookDistance(Aircraft aircraft, Scenario scenario)
        {

            _lowerPressureAltitude = PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(
                (int)ActualPressureAltitude,
                _pressureAltitudeInterval);
            _upperPressureAltitude = _lowerPressureAltitude + _pressureAltitudeInterval;

            _lowerTemperature = PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation((int)ActualTemperature, _temperatureInterval);
            _upperTemperature = _lowerTemperature + _temperatureInterval;

            AircraftLowerWeight = aircraft.GetLowerWeight();
            AircraftHigherWeight = aircraft.GetHigherWeight();


            BookPerformanceData d1 = FindBookDistance(
                    _lowerPressureAltitude,
                    _lowerTemperature,
                    AircraftLowerWeight);

            BookPerformanceData d2 = FindBookDistance(
                    _upperPressureAltitude,
                    _lowerTemperature,
                    AircraftLowerWeight);

            BookPerformanceData d3 = FindBookDistance(
                    _upperPressureAltitude,
                    _lowerTemperature,
                    AircraftLowerWeight);

            BookPerformanceData d4 = FindBookDistance(
                    _upperPressureAltitude,
                    _upperTemperature,
                    AircraftLowerWeight);

            BookPerformanceData d5 = FindBookDistance(
                    _lowerPressureAltitude,
                    _lowerTemperature,
                    AircraftHigherWeight);

            BookPerformanceData d6 = FindBookDistance(
                    _upperPressureAltitude,
                    _lowerTemperature,
                    AircraftHigherWeight);

            BookPerformanceData d7 = FindBookDistance(
                    _upperPressureAltitude,
                    _lowerTemperature,
                    AircraftHigherWeight);

            BookPerformanceData d8 = FindBookDistance(
                    _upperPressureAltitude,
                    _upperTemperature,
                    AircraftHigherWeight);

            List<InterpolatedPerformanceData> interpolatedPerformanceDataList = new List<InterpolatedPerformanceData>();

            InterpolatedPerformanceData lowerWeight = InterpolateByPressureAltitude(d1, d2, d3, d4, AircraftLowerWeight);
            InterpolatedPerformanceData upperWeight = InterpolateByPressureAltitude(d5, d6, d7, d8, AircraftHigherWeight);

            return InterpolateByAircraftWeight(lowerWeight, upperWeight, AircraftActualWeigth);

        }

        public BookPerformanceData FindBookDistance(
            decimal pressureAltitude,
            decimal temperature,
            decimal weight)
        {
            return _performanceDataList.FindBookDistance(
                Scenario,
                pressureAltitude,
                temperature,
                weight);
        }


        public InterpolatedPerformanceData InterpolateByPressureAltitude(
            PerformanceData lowerPressureAltitudePerformanceData,
            PerformanceData upperPresssureAltitudePerformanceData,
            PerformanceData lowerTemperaturePerformanceData,
            PerformanceData upperTemperaturePerformanceData,
            decimal weight)
        {
            // bookData1 will be like: pressure altitude 1000, temperature 10
            // bookData2 will be like: pressure altitude 2000, temperature 10
            // actualPressureAltitude will be like 1500
            // we need to do a two-way interpolation here

            decimal interpolatedGroundRoll = PerformanceDataInterpolationUtilities.Interpolate(
                lowerPressureAltitudePerformanceData.GroundRoll,
                upperPresssureAltitudePerformanceData.GroundRoll,
                (int)ActualPressureAltitude,
                _pressureAltitudeInterval
                );

            decimal interpolatedDistanceToClear50Ft = PerformanceDataInterpolationUtilities.Interpolate(
                lowerPressureAltitudePerformanceData.DistanceToClear50Ft,
                upperPresssureAltitudePerformanceData.DistanceToClear50Ft,
                (int)ActualPressureAltitude,
                _pressureAltitudeInterval
            );

            lowerTemperaturePerformanceData.GroundRoll = interpolatedGroundRoll;
            lowerTemperaturePerformanceData.DistanceToClear50Ft = interpolatedDistanceToClear50Ft;
            lowerTemperaturePerformanceData.AircraftWeight = weight;
            upperTemperaturePerformanceData.GroundRoll = interpolatedGroundRoll;
            upperTemperaturePerformanceData.DistanceToClear50Ft = interpolatedDistanceToClear50Ft;
            upperTemperaturePerformanceData.AircraftWeight = weight;

            lowerPressureAltitudePerformanceData.AircraftWeight = weight;
            upperPresssureAltitudePerformanceData.AircraftWeight = weight;

            return InterpolateByTemperature(
                lowerTemperaturePerformanceData,
                upperTemperaturePerformanceData,
                weight
                );
        }

        public InterpolatedPerformanceData InterpolateByTemperature(
            PerformanceData lowerTemperaturePerformanceData,
            PerformanceData upperTemperaturePerformanceData,
            decimal weight)
        {
            // bookData1 will be like: pressure altitude 1000, temperature 10
            // bookData2 will be like: pressure altitude 1000, temperature 20
            // actualTemperature will be like 15
            // we need to do a two-way interpolation here

            decimal interpolatedGroundRoll = PerformanceDataInterpolationUtilities.Interpolate(
                lowerTemperaturePerformanceData.GroundRoll,
                upperTemperaturePerformanceData.GroundRoll,
                (int)ActualTemperature,
                _temperatureInterval
                );

            decimal interpolatedDistanceToClear50Ft = PerformanceDataInterpolationUtilities.Interpolate(
                lowerTemperaturePerformanceData.DistanceToClear50Ft,
                upperTemperaturePerformanceData.DistanceToClear50Ft,
                (int)ActualTemperature,
                _temperatureInterval
            );

            return new InterpolatedPerformanceData(
                lowerTemperaturePerformanceData.Scenario,
                ActualPressureAltitude,
                ActualTemperature,
                weight,
                interpolatedGroundRoll,
                interpolatedDistanceToClear50Ft
                );
        }

        public InterpolatedPerformanceData InterpolateByAircraftWeight(
             PerformanceData lowerWeightPerformanceData,
             PerformanceData upperWeightPerformanceData,
             decimal weight)
        {
            // bookData1 will be like: pressure altitude 1000, temperature 10
            // bookData2 will be like: pressure altitude 1000, temperature 20
            // actualTemperature will be like 15
            // we need to do a two-way interpolation here

            decimal interpolatedGroundRoll = InterpolateDistanceByWeight(
                GetWeightInterpolationFactor((int)AircraftActualWeigth),
                lowerWeightPerformanceData.GroundRoll,
                upperWeightPerformanceData.GroundRoll
            );

            decimal interpolatedDistanceToClear50Ft = InterpolateDistanceByWeight(
                GetWeightInterpolationFactor((int)AircraftActualWeigth),
                lowerWeightPerformanceData.DistanceToClear50Ft,
                upperWeightPerformanceData.DistanceToClear50Ft
            );

            return new InterpolatedPerformanceData(
                lowerWeightPerformanceData.Scenario,
                ActualPressureAltitude,
                ActualTemperature,
                weight,
                interpolatedGroundRoll,
                interpolatedDistanceToClear50Ft
                );
        }

        public decimal GetWeightInterpolationFactor(int weight)
        {
            if (weight <= 0)
            {
                throw new Exception("Weight must be greater than zero");
            }
            else if (weight < AircraftLowerWeight)
            {
                throw new AircraftWeightOutOfRangeException((int)AircraftLowerWeight, (int)AircraftHigherWeight);
            }
            else if (weight > AircraftHigherWeight)
            {
                throw new AircraftWeightOutOfRangeException((int)AircraftLowerWeight, (int)AircraftHigherWeight);
            }

            int lowerWeight = (int)AircraftLowerWeight;
            int higherWeight = (int)AircraftHigherWeight;

            decimal weightInterpolationFactor = (decimal)(weight - lowerWeight) / (decimal)(higherWeight - lowerWeight);

            return weightInterpolationFactor;
        }

        /// <summary>
        /// Interpolates between two distances based on the weight interpolation factor
        /// i.e. if the weight interpolation factor is 0.5, it will return the average of the two distances
        /// </summary>
        private static decimal InterpolateDistanceByWeight(decimal weightInterpolationFactor, decimal distance_LowerWeight, decimal distance_HigherWeight)
        {
            return distance_LowerWeight +
                (weightInterpolationFactor * (distance_HigherWeight - distance_LowerWeight));
        }

    }
}