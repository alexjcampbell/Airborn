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

        private PeformanceDataInterpolator(
            Scenario scenario,
            decimal actualPressureAltitude,
            decimal actualTemperature,
            decimal aircraftActualWeight
        )
        {
            ActualPressureAltitude = actualPressureAltitude;
            ActualTemperature = actualTemperature;
            AircraftActualWeight = aircraftActualWeight;
            Scenario = scenario;

            SetLowerAndUpperPressureAltitudeAndTemperature();
        }

        ///  <summary>
        ///  This constructor is used for testing purposes only - it allows us to pass in a list of
        ///  BookPerformanceData objects instead of a JsonFile object.
        ///  </summary>
        ///  <param name="scenario"></param>
        ///  <param name="actualPressureAltitude">Scenario tells us whether we're in takeoff or landing mode</param>
        ///  <param name="actualTemperature">The actual reported temperature at the airport</param>
        ///  <param name="actualWeight">The actual weight of the </param>
        ///  <param name="bookPerformanceDataList">The BookPerformanceDataList that has been pre-populated with all of the book data</param>
        public PeformanceDataInterpolator(
            Scenario scenario,
            decimal actualPressureAltitude,
            decimal actualTemperature,
            decimal actualWeight,
            BookPerformanceDataList bookPerformanceDataList
        ) : this(scenario, actualPressureAltitude, actualTemperature, actualWeight)
        {
            Scenario = scenario;
            _bookPerformanceDataList = bookPerformanceDataList;
        }

        /// <summary>   
        /// The raw JsonFile with the aircract data for the lower weight
        /// </summary>
        public JsonFile JsonFileLowerWeight
        {
            get;
            private set;
        }

        /// <summary>   
        /// The raw JsonFile with the aircract data for the higher weight
        /// </summary>
        public JsonFile JsonFileHigherWeight
        {
            get;
            private set;
        }

        private BookPerformanceDataList _bookPerformanceDataList;

        /// <summary>
        /// The actual pressure altitude calculated from the airport elevation and the QNH
        /// </summary>
        public decimal ActualPressureAltitude
        {
            get;
            private set;
        }

        /// <summary>
        /// The actual temperature at the airport
        /// </summary>
        public decimal ActualTemperature
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether we're in takeoff or landing scenario
        /// </summary>
        public Scenario Scenario
        {
            get;
            private set;
        }

        private decimal _lowerPressureAltitude;
        private decimal _upperPressureAltitude;

        private decimal _lowerTemperature;
        private decimal _upperTemperature;

        /// <summary>
        /// The lowest weight for which we have performance data for this aircraft
        /// </summary>
        public decimal AircraftLowerWeight
        {
            get;
            private set;
        }

        /// <summary>
        /// The highest weight for which we have performance data for this aircraft
        /// </summary>
        public decimal AircraftHigherWeight
        {
            get;
            private set;
        }


        /// <summary>
        /// The reported weight of the aircraft for which we're calculating performance for
        /// </summary>
        public decimal AircraftActualWeight
        {
            get;
            private set;
        }

        private const int _pressureAltitudeInterval = 1000; // the interval at which performance data is provided in the POH
        private const int _temperatureInterval = 10; // the internal at which which temperature data is provided in the POH

        public InterpolatedPerformanceData GetInterpolatedBookDistance(Aircraft aircraft)
        {
            SetLowerAndUpperPressureAltitudeAndTemperature();

            // get the lower and higher weights for which we have performance data
            AircraftLowerWeight = aircraft.GetLowerWeight();
            AircraftHigherWeight = aircraft.GetHigherWeight();

            // we need eight BookPerformanceDatas to interpolate between
            // first by pressure altitude, then by temperature, then by weight

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
                    _upperTemperature == ActualTemperature ? ActualTemperature : _upperTemperature,
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
                    _upperTemperature == ActualTemperature ? ActualTemperature : _upperTemperature,
                    AircraftHigherWeight);

            List<InterpolatedPerformanceData> interpolatedPerformanceDataList = new List<InterpolatedPerformanceData>();

            InterpolatedPerformanceData lowerWeight = InterpolateByPressureAltitude(d1, d2, d3, d4, AircraftLowerWeight);
            InterpolatedPerformanceData upperWeight = InterpolateByPressureAltitude(d5, d6, d7, d8, AircraftHigherWeight);

            return InterpolateByAircraftWeight(lowerWeight, upperWeight, AircraftActualWeight);

        }

        private void SetLowerAndUpperPressureAltitudeAndTemperature()
        {
            _lowerPressureAltitude = PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(
                (int)ActualPressureAltitude,
                _pressureAltitudeInterval);
            _upperPressureAltitude = _lowerPressureAltitude + _pressureAltitudeInterval;

            _lowerTemperature = PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(
                (int)ActualTemperature,
                _temperatureInterval);
            _upperTemperature = _lowerTemperature + _temperatureInterval;

        }

        public BookPerformanceData FindBookDistance(
            decimal pressureAltitude,
            decimal temperature,
            decimal weight)
        {

            return _bookPerformanceDataList.FindBookDistance(
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
            InterpolatedPerformanceData interpolatedPerformanceDataByPressureAltitude =
                InterpolateByPressureAltitudeOnly(lowerPressureAltitudePerformanceData, upperPresssureAltitudePerformanceData, weight);

            lowerTemperaturePerformanceData.DistanceGroundRoll = interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll;
            lowerTemperaturePerformanceData.DistanceToClear50Ft = interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft;
            upperTemperaturePerformanceData.DistanceGroundRoll = interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll;
            upperTemperaturePerformanceData.DistanceToClear50Ft = interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft;

            return InterpolateByTemperatureOnly(
                lowerTemperaturePerformanceData,
                upperTemperaturePerformanceData,
                weight
                );
        }

        public InterpolatedPerformanceData InterpolateByPressureAltitudeOnly(PerformanceData lowerPressureAltitudePerformanceData, PerformanceData upperPressureAltitudePerformanceData, decimal weight)
        {
            InterpolatedPerformanceData interpolatedPerformanceDataByPressureAltitude = new InterpolatedPerformanceData(
                this.Scenario,
                ActualPressureAltitude,
                ActualTemperature,
                weight
            );

            if (ActualPressureAltitude == lowerPressureAltitudePerformanceData.PressureAltitude)
            {
                // if we're at the lower or upper pressure altitude, then we don't need to interpolate and we use the lower data

                interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll = lowerPressureAltitudePerformanceData.DistanceGroundRoll;
                interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft = lowerPressureAltitudePerformanceData.DistanceToClear50Ft;
                return interpolatedPerformanceDataByPressureAltitude;
            }
            else if (ActualPressureAltitude == upperPressureAltitudePerformanceData.PressureAltitude)
            {
                // if we're at the upper pressure altitude, then we don't need to interpolate and we use the upper data

                interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll = upperPressureAltitudePerformanceData.DistanceGroundRoll;
                interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft = upperPressureAltitudePerformanceData.DistanceToClear50Ft;
                return interpolatedPerformanceDataByPressureAltitude;
            }
            else
            {
                interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll = PerformanceDataInterpolationUtilities.Interpolate(
                    lowerPressureAltitudePerformanceData.DistanceGroundRoll.Value,
                    upperPressureAltitudePerformanceData.DistanceGroundRoll.Value,
                    (int)ActualPressureAltitude,
                    _pressureAltitudeInterval
                    );

                interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft = PerformanceDataInterpolationUtilities.Interpolate(
                    lowerPressureAltitudePerformanceData.DistanceToClear50Ft.Value,
                    upperPressureAltitudePerformanceData.DistanceToClear50Ft.Value,
                    (int)ActualPressureAltitude,
                    _pressureAltitudeInterval
                );


                return interpolatedPerformanceDataByPressureAltitude;
            }
        }

        public InterpolatedPerformanceData InterpolateByTemperatureOnly(
            PerformanceData lowerTemperaturePerformanceData,
            PerformanceData upperTemperaturePerformanceData,
            decimal weight)
        {

            InterpolatedPerformanceData interpolatedPerformanceDataByTemperature = new InterpolatedPerformanceData(
                this.Scenario,
                ActualPressureAltitude,
                ActualTemperature,
                weight
            );

            if (ActualTemperature == lowerTemperaturePerformanceData.Temperature)
            {
                // if we're at the lower or upper temperature, then we don't need to interpolate and we use the lower data

                interpolatedPerformanceDataByTemperature.DistanceGroundRoll = lowerTemperaturePerformanceData.DistanceGroundRoll;
                interpolatedPerformanceDataByTemperature.DistanceToClear50Ft = lowerTemperaturePerformanceData.DistanceToClear50Ft;
                return interpolatedPerformanceDataByTemperature;
            }
            else if (ActualTemperature == upperTemperaturePerformanceData.Temperature)
            {
                // if we're at the upper temperature, then we don't need to interpolate and we use the upper data

                interpolatedPerformanceDataByTemperature.DistanceGroundRoll = upperTemperaturePerformanceData.DistanceGroundRoll;
                interpolatedPerformanceDataByTemperature.DistanceToClear50Ft = upperTemperaturePerformanceData.DistanceToClear50Ft;
                return interpolatedPerformanceDataByTemperature;
            }

            interpolatedPerformanceDataByTemperature.DistanceGroundRoll = PerformanceDataInterpolationUtilities.Interpolate(
                lowerTemperaturePerformanceData.DistanceGroundRoll.Value,
                upperTemperaturePerformanceData.DistanceGroundRoll.Value,
                (int)ActualTemperature,
                _temperatureInterval
                );

            interpolatedPerformanceDataByTemperature.DistanceToClear50Ft = PerformanceDataInterpolationUtilities.Interpolate(
                lowerTemperaturePerformanceData.DistanceToClear50Ft.Value,
                upperTemperaturePerformanceData.DistanceToClear50Ft.Value,
                (int)ActualTemperature,
                _temperatureInterval
            );

            return interpolatedPerformanceDataByTemperature;
        }

        public InterpolatedPerformanceData InterpolateByAircraftWeight(
             PerformanceData lowerWeightPerformanceData,
             PerformanceData upperWeightPerformanceData,
             decimal weight)
        {

            InterpolatedPerformanceData interpolatedPerformanceDataByWeight = new InterpolatedPerformanceData(
                this.Scenario,
                ActualPressureAltitude,
                ActualTemperature,
                weight
            );

            interpolatedPerformanceDataByWeight.DistanceGroundRoll = PerformanceDataInterpolationUtilities.InterpolateDistanceByWeight(
                GetWeightInterpolationFactor(AircraftActualWeight),
                lowerWeightPerformanceData.DistanceGroundRoll.Value,
                upperWeightPerformanceData.DistanceGroundRoll.Value
            );

            interpolatedPerformanceDataByWeight.DistanceToClear50Ft = PerformanceDataInterpolationUtilities.InterpolateDistanceByWeight(
                GetWeightInterpolationFactor(AircraftActualWeight),
                lowerWeightPerformanceData.DistanceToClear50Ft.Value,
                upperWeightPerformanceData.DistanceToClear50Ft.Value
            );

            return interpolatedPerformanceDataByWeight;
        }

        public decimal GetWeightInterpolationFactor(decimal weight)
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

            decimal lowerWeight = AircraftLowerWeight;
            decimal higherWeight = AircraftHigherWeight;

            decimal weightInterpolationFactor = (decimal)(weight - lowerWeight) / (decimal)(higherWeight - lowerWeight);

            return weightInterpolationFactor;
        }



    }
}