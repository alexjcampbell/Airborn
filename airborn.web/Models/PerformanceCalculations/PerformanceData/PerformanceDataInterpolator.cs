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
            decimal aircraftActualWeight,
            PerformanceCalculationLogItem logger
        )
        {
            ActualPressureAltitude = actualPressureAltitude;
            ActualTemperature = actualTemperature;
            AircraftActualWeight = aircraftActualWeight;
            Scenario = scenario;
            _logger = logger;

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
        ///  <param name="logger">The logger that will be used to log how we calculate takeoff and landing distances</param>
        public PeformanceDataInterpolator(
            Scenario scenario,
            decimal actualPressureAltitude,
            decimal actualTemperature,
            decimal actualWeight,
            BookPerformanceDataList bookPerformanceDataList,
            PerformanceCalculationLogItem logger
        ) : this(scenario, actualPressureAltitude, actualTemperature, actualWeight, logger)
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

        private PerformanceCalculationLogItem _logger;

        /// <summary>
        /// Get the interpolated performance data from the POH for a given aircraft
        /// </summary>
        /// <param name="aircraft">The aircraft that we're calculating performance for</param>
        public InterpolatedPerformanceData GetInterpolatedBookDistances(Aircraft aircraft)
        {

            _logger.Add($"Getting book perfomance data for {Scenario}, for aircraft {aircraft.GetType()}. Actual aircraft weight: {AircraftActualWeight} Actual pressure altitude: {ActualPressureAltitude}, actual temperature: {ActualTemperature}");

            // get the lower and higher weights for which we have performance data
            AircraftLowerWeight = aircraft.GetLowerWeight();
            AircraftHigherWeight = aircraft.GetHigherWeight();

            PerformanceCalculationLogItem foundBookDataLogItem = new PerformanceCalculationLogItem("Found book data");
            _logger.SubItems.Add(foundBookDataLogItem);

            // we need eight BookPerformanceDatas to interpolate between
            // first by pressure altitude, then by temperature, then by weight

            BookPerformanceData d1 = FindBookDistance(
                    _lowerPressureAltitude,
                    _lowerTemperature,
                    AircraftLowerWeight,
                    foundBookDataLogItem);

            BookPerformanceData d2 = FindBookDistance(
                    _upperPressureAltitude,
                    _lowerTemperature,
                    AircraftLowerWeight,
                    foundBookDataLogItem
                    );

            BookPerformanceData d3 = FindBookDistance(
                    _lowerPressureAltitude,
                    _upperTemperature,
                    AircraftLowerWeight,
                    foundBookDataLogItem
                    );

            BookPerformanceData d4 = FindBookDistance(
                    _upperPressureAltitude,
                    _upperTemperature == ActualTemperature ? ActualTemperature : _upperTemperature,
                    AircraftLowerWeight,
                    foundBookDataLogItem);

            BookPerformanceData d5 = FindBookDistance(
                    _lowerPressureAltitude,
                    _lowerTemperature,
                    AircraftHigherWeight,
                    foundBookDataLogItem);

            BookPerformanceData d6 = FindBookDistance(
                    _upperPressureAltitude,
                    _lowerTemperature,
                    AircraftHigherWeight,
                    foundBookDataLogItem);

            BookPerformanceData d7 = FindBookDistance(
                    _lowerPressureAltitude,
                    _lowerTemperature,
                    AircraftHigherWeight,
                    foundBookDataLogItem);

            BookPerformanceData d8 = FindBookDistance(
                    _upperPressureAltitude,
                    _upperTemperature == ActualTemperature ? ActualTemperature : _upperTemperature,
                    AircraftHigherWeight,
                    foundBookDataLogItem);

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
            decimal weight,
            PerformanceCalculationLogItem foundBookDataLogItem
            )
        {



            BookPerformanceData bookData = _bookPerformanceDataList.FindBookDistance(
                Scenario,
                pressureAltitude,
                temperature,
                weight);

            PerformanceCalculationLogItem item = new PerformanceCalculationLogItem($"Aircraft weight {weight}, pressure altitude {pressureAltitude}, temperature {temperature}");
            foundBookDataLogItem.SubItems.Add(item);

            item.Add($"Ground roll {bookData.DistanceGroundRoll}, distance to clear 50' obstacle {bookData.DistanceToClear50Ft}");

            return bookData;
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

                _logger.Add($"No interpolation required for pressure altitude {ActualPressureAltitude}, ground roll is: {interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll}, distance to clear 50' obstacle is: {interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft}");

                return interpolatedPerformanceDataByPressureAltitude;
            }
            else if (ActualPressureAltitude == upperPressureAltitudePerformanceData.PressureAltitude)
            {
                // if we're at the upper pressure altitude, then we don't need to interpolate and we use the upper data

                interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll = upperPressureAltitudePerformanceData.DistanceGroundRoll;
                interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft = upperPressureAltitudePerformanceData.DistanceToClear50Ft;

                _logger.Add($"No interpolation required for pressure altitude {ActualPressureAltitude}, ground roll is: {interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll}, distance to clear 50' obstacle is: {interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft}");

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

                // we need the pressure altitude interpolation factor for logging purposes
                // TODO: make the Interpolate function return this value
                decimal pressureAltitudeInterpolationFactor = PerformanceDataInterpolationUtilities.CalculateInterpolationFactor(
                    (int)ActualPressureAltitude,
                    _lowerPressureAltitude,
                    _upperPressureAltitude
                );

                PerformanceCalculationLogItem logItem = new PerformanceCalculationLogItem($"Interpolated performance data by pressure altitude for aircraft weight {weight}, temperature {lowerPressureAltitudePerformanceData.Temperature}");

                logItem.Add($"Interpolating actual pressure altitude {ActualPressureAltitude}, between {lowerPressureAltitudePerformanceData.PressureAltitude} and {upperPressureAltitudePerformanceData.PressureAltitude}, with interpolation factor {pressureAltitudeInterpolationFactor}");
                logItem.Add($"Lower pressure altitude performance data at {lowerPressureAltitudePerformanceData.PressureAltitude}, ground roll is: {lowerPressureAltitudePerformanceData.DistanceGroundRoll}, distance to clear 50' obstacle is: {lowerPressureAltitudePerformanceData.DistanceToClear50Ft}");
                logItem.Add($"Upper pressure altitude performance data at {upperPressureAltitudePerformanceData.PressureAltitude}, ground roll is: {upperPressureAltitudePerformanceData.DistanceGroundRoll}, distance to clear 50' obstacle is: {upperPressureAltitudePerformanceData.DistanceToClear50Ft}");
                logItem.Add($"Interpolated ground roll is: {interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll}, distance to clear 50' obstacle is: {interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft}");
                _logger.SubItems.Add(logItem);

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

                _logger.Add($"No interpolation required for temperature {ActualTemperature}, ground roll is: {interpolatedPerformanceDataByTemperature.DistanceGroundRoll}, distance to clear 50' obstacle is: {interpolatedPerformanceDataByTemperature.DistanceToClear50Ft}");

                return interpolatedPerformanceDataByTemperature;
            }
            else if (ActualTemperature == upperTemperaturePerformanceData.Temperature)
            {
                // if we're at the upper temperature, then we don't need to interpolate and we use the upper data

                interpolatedPerformanceDataByTemperature.DistanceGroundRoll = upperTemperaturePerformanceData.DistanceGroundRoll;
                interpolatedPerformanceDataByTemperature.DistanceToClear50Ft = upperTemperaturePerformanceData.DistanceToClear50Ft;

                _logger.Add($"No interpolation required for temperature {ActualTemperature}, ground roll is: {interpolatedPerformanceDataByTemperature.DistanceGroundRoll}, distance to clear 50' obstacle is: {interpolatedPerformanceDataByTemperature.DistanceToClear50Ft}");

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

            decimal temperatureInterpolationFactor = PerformanceDataInterpolationUtilities.CalculateInterpolationFactor(
                (int)ActualTemperature,
                _lowerTemperature,
                _upperTemperature
            );

            PerformanceCalculationLogItem logItem = new PerformanceCalculationLogItem($"Interpolated performance data by temperature for aircraft weight {weight}, pressure altitude {lowerTemperaturePerformanceData.PressureAltitude}");

            logItem.Add($"Interpolating actual temperature {ActualTemperature}, between {lowerTemperaturePerformanceData.Temperature} and {upperTemperaturePerformanceData.Temperature}, with interpolation factor {temperatureInterpolationFactor}");
            logItem.Add($"Lower temperature performance data at {lowerTemperaturePerformanceData.Temperature}: ground roll {lowerTemperaturePerformanceData.DistanceGroundRoll}, distance to clear 50' obstacle {lowerTemperaturePerformanceData.DistanceToClear50Ft}");
            logItem.Add($"Upper temperature performance data at {upperTemperaturePerformanceData.Temperature}: ground roll {upperTemperaturePerformanceData.DistanceGroundRoll}, distance to clear 50' obstacle {upperTemperaturePerformanceData.DistanceToClear50Ft}");
            logItem.Add($"Interpolated ground roll is: {interpolatedPerformanceDataByTemperature.DistanceGroundRoll}, distance to clear 50' obstacle is: {interpolatedPerformanceDataByTemperature.DistanceToClear50Ft}");
            _logger.SubItems.Add(logItem);

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

            decimal weightInterpolationFactor = GetWeightInterpolationFactor(AircraftActualWeight);

            PerformanceCalculationLogItem logItem = new PerformanceCalculationLogItem("Interpolated weight performance data between:");

            logItem.Add($"Interpolated weight {AircraftActualWeight}, between {lowerWeightPerformanceData.AircraftWeight} and {upperWeightPerformanceData.AircraftWeight}, with interpolation factor {weightInterpolationFactor}");
            logItem.Add($"Lower weight performance data at {lowerWeightPerformanceData.AircraftWeight}: ground roll {lowerWeightPerformanceData.DistanceGroundRoll}, distance to clear 50' obstacle {lowerWeightPerformanceData.DistanceToClear50Ft}");
            logItem.Add($"Upper weight performance data at {upperWeightPerformanceData.AircraftWeight}: ground roll {upperWeightPerformanceData.DistanceGroundRoll}, distance to clear 50' obstacle {upperWeightPerformanceData.DistanceToClear50Ft}");
            logItem.Add($"Ground roll is: {interpolatedPerformanceDataByWeight.DistanceGroundRoll}, distance to clear 50' obstacle is: {interpolatedPerformanceDataByWeight.DistanceToClear50Ft}");

            _logger.SubItems.Add(logItem);

            return interpolatedPerformanceDataByWeight;
        }

        /// <summary>
        /// Returns the interpolation factor for the given weight.  The interpolation factor is the percentage distance that the given weight is between the lowest
        /// and highest possible weight for the aircraft
        /// </summary>
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

            // interpolaton factor is the percentage distance that the given weight is between the lowest
            // and highest possible weight for the aircraft
            decimal weightInterpolationFactor = (weight - lowerWeight) / (decimal)(higherWeight - lowerWeight);

            return weightInterpolationFactor;
        }



    }
}