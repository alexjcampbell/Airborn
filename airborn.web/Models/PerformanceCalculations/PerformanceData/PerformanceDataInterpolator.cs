using System;
using System.IO;
using System.Collections.Generic;

namespace Airborn.web.Models.PerformanceData
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
            Distance actualPressureAltitude,
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
            Distance actualPressureAltitude,
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
        public Distance ActualPressureAltitude
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

        private Distance _lowerPressureAltitude;
        private Distance _upperPressureAltitude;

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

            // get the lower and higher weights for which we have performance data
            AircraftLowerWeight = aircraft.GetLowerWeight();
            AircraftHigherWeight = aircraft.GetHigherWeight();

            PerformanceCalculationLogItem foundBookDataLogItem = new PerformanceCalculationLogItem("Looking for book data:");

            foundBookDataLogItem.Add($"Scenario: {Scenario}");
            foundBookDataLogItem.Add($"Aircraft: {aircraft.GetAircraftTypeString()}");
            foundBookDataLogItem.Add($"Actual aircraft weight: {AircraftActualWeight}" + " lbs");
            foundBookDataLogItem.Add($"Actual pressure altitude: {ActualPressureAltitude}");
            foundBookDataLogItem.Add($"Actual temperature: {ActualTemperature}" + " °C");

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
                    _upperTemperature,
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
                    _upperTemperature,
                    AircraftHigherWeight,
                    foundBookDataLogItem);

            List<InterpolatedPerformanceData> interpolatedPerformanceDataList = new List<InterpolatedPerformanceData>();

            InterpolatedPerformanceData lowerWeight = InterpolateByPressureAltitude(d1, d2, d3, d4, AircraftLowerWeight);
            InterpolatedPerformanceData upperWeight = InterpolateByPressureAltitude(d5, d6, d7, d8, AircraftHigherWeight);

            return InterpolateByAircraftWeight(lowerWeight, upperWeight, AircraftActualWeight);

        }

        private void SetLowerAndUpperPressureAltitudeAndTemperature()
        {
            _lowerPressureAltitude = Distance.FromFeet(
            PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(
                (int)ActualPressureAltitude.TotalFeet,
                _pressureAltitudeInterval));

            _upperPressureAltitude = Distance.FromFeet(_lowerPressureAltitude.TotalFeet + _pressureAltitudeInterval);

            _lowerTemperature = PerformanceDataInterpolationUtilities.GetLowerBoundForInterpolation(
                (int)ActualTemperature,
                _temperatureInterval);

            if (_lowerTemperature == ActualTemperature)
            {
                _upperTemperature = ActualTemperature;
            }
            else
            {
                _upperTemperature = _lowerTemperature + _temperatureInterval;
            }


        }

        public BookPerformanceData FindBookDistance(
            Distance pressureAltitude,
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

            PerformanceCalculationLogItem item = new PerformanceCalculationLogItem($"Found book data for: Aircraft weight {weight} lbs, pressure altitude {pressureAltitude} ft, temperature {temperature} °C");
            foundBookDataLogItem.SubItems.Add(item);

            item.Add($"Ground roll: {bookData.DistanceGroundRoll} ft");
            item.Add($"Distance to clear 50ft: {bookData.DistanceToClear50Ft} ft");

            return bookData;
        }


        public InterpolatedPerformanceData InterpolateByPressureAltitude(
            PerformanceDataBase lowerPressureAltitudePerformanceData,
            PerformanceDataBase upperPresssureAltitudePerformanceData,
            PerformanceDataBase lowerTemperaturePerformanceData,
            PerformanceDataBase upperTemperaturePerformanceData,
            decimal weight)
        {
            // first interpolate by pressure altitude
            InterpolatedPerformanceData interpolatedPerformanceDataByPressureAltitude =
                InterpolateByPressureAltitudeOnly(
                    lowerPressureAltitudePerformanceData,
                    upperPresssureAltitudePerformanceData,
                    weight);

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

        public InterpolatedPerformanceData InterpolateByPressureAltitudeOnly(PerformanceDataBase lowerPressureAltitudePerformanceData, PerformanceDataBase upperPressureAltitudePerformanceData, decimal weight)
        {
            InterpolatedPerformanceData interpolatedPerformanceDataByPressureAltitude = new InterpolatedPerformanceData(
                this.Scenario,
                ActualPressureAltitude,
                ActualTemperature,
                weight
            );

            if (ActualPressureAltitude.TotalFeet == lowerPressureAltitudePerformanceData.PressureAltitude.TotalFeet)
            {
                // if we're at the lower pressure altitude, then we don't need to interpolate and we use the lower data

                interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll = lowerPressureAltitudePerformanceData.DistanceGroundRoll;
                interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft = lowerPressureAltitudePerformanceData.DistanceToClear50Ft;

                PerformanceCalculationLogItem item = new PerformanceCalculationLogItem($"No interpolation required for pressure altitude {ActualPressureAltitude} ft");
                item.Add($"Ground roll: {interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll} ft");
                item.Add($"Distance to clear 50ft: {interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft} ft");
                _logger.SubItems.Add(item);

                return interpolatedPerformanceDataByPressureAltitude;
            }
            if (ActualPressureAltitude.TotalFeet == upperPressureAltitudePerformanceData.PressureAltitude.TotalFeet)
            {
                // if we're at the upper pressure altitude, then we don't need to interpolate and we use the upper data

                interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll = upperPressureAltitudePerformanceData.DistanceGroundRoll;
                interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft = upperPressureAltitudePerformanceData.DistanceToClear50Ft;

                PerformanceCalculationLogItem item = new PerformanceCalculationLogItem($"No interpolation required for pressure altitude {ActualPressureAltitude} ft");
                item.Add($"Ground roll: {interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll} ft");
                item.Add($"Distance to clear 50ft: {interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft} ft");
                _logger.SubItems.Add(item);

                return interpolatedPerformanceDataByPressureAltitude;
            }

            interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll =
                Distance.FromFeet(PerformanceDataInterpolationUtilities.Interpolate(
                    lowerPressureAltitudePerformanceData.DistanceGroundRoll.Value.TotalFeet,
                    upperPressureAltitudePerformanceData.DistanceGroundRoll.Value.TotalFeet,
                    ActualPressureAltitude.TotalFeet,
                    _pressureAltitudeInterval
                ));

            interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft =
                Distance.FromFeet(PerformanceDataInterpolationUtilities.Interpolate(
                    lowerPressureAltitudePerformanceData.DistanceToClear50Ft.Value.TotalFeet,
                    upperPressureAltitudePerformanceData.DistanceToClear50Ft.Value.TotalFeet,
                    ActualPressureAltitude.TotalFeet,
                    _pressureAltitudeInterval
                ));

            // we need the pressure altitude interpolation factor for logging purposes
            // TODO: make the Interpolate function return this value
            decimal pressureAltitudeInterpolationFactor = PerformanceDataInterpolationUtilities.CalculateInterpolationFactor(
                ActualPressureAltitude.TotalFeet,
                _lowerPressureAltitude.TotalFeet,
                _upperPressureAltitude.TotalFeet
            );

            PerformanceCalculationLogItem logItem = new PerformanceCalculationLogItem(
                $"Interpolated performance data by pressure altitude for aircraft weight {weight} lbs, temperature {lowerPressureAltitudePerformanceData.Temperature} °C");

            logItem.Add($"Interpolating actual pressure altitude {ActualPressureAltitude.ToString()} ft, between {lowerPressureAltitudePerformanceData.PressureAltitude} ft and {upperPressureAltitudePerformanceData.PressureAltitude.ToString()} ft, with interpolation factor {pressureAltitudeInterpolationFactor.ToString("0.00")}");
            logItem.SubItems[0].Add($"Lower temperature is {lowerPressureAltitudePerformanceData.Temperature} °C,, upper temperature is {upperPressureAltitudePerformanceData.Temperature} °C");
            logItem.SubItems[0].Add($"Aircraft weight is {weight} lbs");
            logItem.SubItems[0].Add($"Lower pressure altitude performance data at {lowerPressureAltitudePerformanceData.PressureAltitude.ToString()} ft, ground roll is: {lowerPressureAltitudePerformanceData.DistanceGroundRoll} ft, distance to clear 50' obstacle is: {lowerPressureAltitudePerformanceData.DistanceToClear50Ft} ft");
            logItem.SubItems[0].Add($"Upper pressure altitude performance data at {upperPressureAltitudePerformanceData.PressureAltitude.ToString()} ft, ground roll is: {upperPressureAltitudePerformanceData.DistanceGroundRoll} ft, distance to clear 50' obstacle is: {upperPressureAltitudePerformanceData.DistanceToClear50Ft} ft");
            logItem.Add($"Interpolated by pressure altitude, ground roll is: {interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll} ft, distance to clear 50' obstacle is: {interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft} ft");
            _logger.SubItems.Add(logItem);

            return interpolatedPerformanceDataByPressureAltitude;

        }

        public InterpolatedPerformanceData InterpolateByTemperatureOnly(
            PerformanceDataBase lowerTemperaturePerformanceData,
            PerformanceDataBase upperTemperaturePerformanceData,
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
                // if we're at the lower, then we don't need to interpolate and we use the lower data

                interpolatedPerformanceDataByTemperature.DistanceGroundRoll = lowerTemperaturePerformanceData.DistanceGroundRoll;
                interpolatedPerformanceDataByTemperature.DistanceToClear50Ft = lowerTemperaturePerformanceData.DistanceToClear50Ft;

                PerformanceCalculationLogItem item = new PerformanceCalculationLogItem($"No interpolation required for temperature {ActualTemperature}");
                item.Add($"Ground roll: {interpolatedPerformanceDataByTemperature.DistanceGroundRoll} ft");
                item.Add($"Distance to clear 50' obstacle: {interpolatedPerformanceDataByTemperature.DistanceToClear50Ft} ft");

                return interpolatedPerformanceDataByTemperature;
            }
            else if (ActualTemperature == upperTemperaturePerformanceData.Temperature)
            {
                // if we're at the upper temperature, then we don't need to interpolate and we use the upper data

                interpolatedPerformanceDataByTemperature.DistanceGroundRoll = upperTemperaturePerformanceData.DistanceGroundRoll;
                interpolatedPerformanceDataByTemperature.DistanceToClear50Ft = upperTemperaturePerformanceData.DistanceToClear50Ft;

                PerformanceCalculationLogItem item = new PerformanceCalculationLogItem($"No interpolation required for temperature {ActualTemperature}");
                item.Add($"Ground roll: {interpolatedPerformanceDataByTemperature.DistanceGroundRoll} ft");
                item.Add($"Distance to clear 50' obstacle: {interpolatedPerformanceDataByTemperature.DistanceToClear50Ft} ft");

                return interpolatedPerformanceDataByTemperature;
            }

            // interpolate the ground roll performance data by temperature
            interpolatedPerformanceDataByTemperature.DistanceGroundRoll =
                Distance.FromFeet(PerformanceDataInterpolationUtilities.Interpolate(
                    lowerTemperaturePerformanceData.DistanceGroundRoll.Value.TotalFeet,
                    upperTemperaturePerformanceData.DistanceGroundRoll.Value.TotalFeet,
                    ActualTemperature,
                    _temperatureInterval
                    )
                );

            // interpolate the distance to clear 50' obstacle performance data by temperature
            interpolatedPerformanceDataByTemperature.DistanceToClear50Ft = Distance.FromFeet(PerformanceDataInterpolationUtilities.Interpolate(
                    lowerTemperaturePerformanceData.DistanceToClear50Ft.Value.TotalFeet,
                    upperTemperaturePerformanceData.DistanceToClear50Ft.Value.TotalFeet,
                    ActualTemperature,
                    _temperatureInterval
                )
            );

            // we need the temperature interpolation factor for logging purposes
            decimal temperatureInterpolationFactor = PerformanceDataInterpolationUtilities.CalculateInterpolationFactor(
                ActualTemperature,
                _lowerTemperature,
                _upperTemperature
            );

            PerformanceCalculationLogItem logItem = new PerformanceCalculationLogItem($"Interpolated performance data by temperature for aircraft weight {weight}, pressure altitude {lowerTemperaturePerformanceData.PressureAltitude}");

            logItem.Add($"Interpolating actual temperature {ActualTemperature} °C, between {lowerTemperaturePerformanceData.Temperature} °C and {upperTemperaturePerformanceData.Temperature} °C, with interpolation factor {temperatureInterpolationFactor}");
            logItem.SubItems[0].Add($"Lower pressure altitude is {lowerTemperaturePerformanceData.PressureAltitude} ft, upper pressure altitude is {upperTemperaturePerformanceData.PressureAltitude} ft");
            logItem.SubItems[0].Add($"Aircraft weight is {weight} lbs");
            logItem.SubItems[0].Add($"Lower temperature performance data at {lowerTemperaturePerformanceData.Temperature} °C: ground roll {lowerTemperaturePerformanceData.DistanceGroundRoll} ft, distance to clear 50' obstacle {lowerTemperaturePerformanceData.DistanceToClear50Ft} ft");
            logItem.SubItems[0].Add($"Upper temperature performance data at {upperTemperaturePerformanceData.Temperature} °C: ground roll {upperTemperaturePerformanceData.DistanceGroundRoll} ft, distance to clear 50' obstacle {upperTemperaturePerformanceData.DistanceToClear50Ft} ft");
            logItem.Add($"Interpolated by temperature, ground roll is: {interpolatedPerformanceDataByTemperature.DistanceGroundRoll}, distance to clear 50' obstacle is: {interpolatedPerformanceDataByTemperature.DistanceToClear50Ft}");
            _logger.SubItems.Add(logItem);

            return interpolatedPerformanceDataByTemperature;
        }

        public InterpolatedPerformanceData InterpolateByAircraftWeight(
             PerformanceDataBase lowerWeightPerformanceData,
             PerformanceDataBase upperWeightPerformanceData,
             decimal weight)
        {

            InterpolatedPerformanceData interpolatedPerformanceDataByWeight = new InterpolatedPerformanceData(
                this.Scenario,
                ActualPressureAltitude,
                ActualTemperature,
                weight
            );

            decimal weightInterpolationFactor = GetWeightInterpolationFactor(AircraftActualWeight);

            interpolatedPerformanceDataByWeight.DistanceGroundRoll =
                Distance.FromFeet(PerformanceDataInterpolationUtilities.InterpolateDistanceByWeight(
                    weightInterpolationFactor,
                    lowerWeightPerformanceData.DistanceGroundRoll.Value.TotalFeet,
                    upperWeightPerformanceData.DistanceGroundRoll.Value.TotalFeet)
            );

            interpolatedPerformanceDataByWeight.DistanceToClear50Ft =
                Distance.FromFeet(PerformanceDataInterpolationUtilities.InterpolateDistanceByWeight(
                    weightInterpolationFactor,
                    lowerWeightPerformanceData.DistanceToClear50Ft.Value.TotalFeet,
                    upperWeightPerformanceData.DistanceToClear50Ft.Value.TotalFeet)
            );

            PerformanceCalculationLogItem logItem =
                new PerformanceCalculationLogItem("Interpolated weight performance data between:");

            logItem.Add($"Interpolated weight {AircraftActualWeight} lbs, between {lowerWeightPerformanceData.AircraftWeight} lbs and {upperWeightPerformanceData.AircraftWeight} lbs, with interpolation factor {weightInterpolationFactor.ToString("0.00")}");
            logItem.SubItems[0].Add($"Lower weight performance data at {lowerWeightPerformanceData.AircraftWeight} lbs: ground roll {lowerWeightPerformanceData.DistanceGroundRoll} ft, distance to clear 50' obstacle {lowerWeightPerformanceData.DistanceToClear50Ft} ft");
            logItem.SubItems[0].Add($"Upper weight performance data at {upperWeightPerformanceData.AircraftWeight} lbs: ground roll {upperWeightPerformanceData.DistanceGroundRoll} ft, distance to clear 50' obstacle {upperWeightPerformanceData.DistanceToClear50Ft} ft");
            logItem.Add($"Interpolated by weight, ground roll is: {interpolatedPerformanceDataByWeight.DistanceGroundRoll} ft, distance to clear 50' obstacle is: {interpolatedPerformanceDataByWeight.DistanceToClear50Ft} ft");

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