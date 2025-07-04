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
            double actualTemperature,
            double aircraftActualWeight,
            PerformanceCalculationLog.LogItem logger
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
            double actualTemperature,
            double actualWeight,
            BookPerformanceDataList bookPerformanceDataList,
            PerformanceCalculationLog.LogItem logger
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
        public double ActualTemperature
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

        private double _lowerTemperature;
        private double _upperTemperature;

        /// <summary>
        /// The lowest weight for which we have performance data for this aircraft
        /// </summary>
        public double AircraftLowerWeight
        {
            get;
            private set;
        }

        /// <summary>
        /// The highest weight for which we have performance data for this aircraft
        /// </summary>
        public double AircraftHigherWeight
        {
            get;
            private set;
        }


        /// <summary>
        /// The reported weight of the aircraft for which we're calculating performance for
        /// </summary>
        public double AircraftActualWeight
        {
            get;
            private set;
        }

        private const int _pressureAltitudeInterval = 1000; // the interval at which performance data is provided in the POH

        private const int _temperatureInterval = 10; // the internal at which which temperature data is provided in the POH

        private PerformanceCalculationLog.LogItem _logger;

        /// <summary>
        /// Get the interpolated performance data from the POH for a given aircraft
        /// </summary>
        /// <param name="aircraft">The aircraft that we're calculating performance for</param>
        public InterpolatedPerformanceData GetInterpolatedBookDistances(Aircraft aircraft)
        {

            // get the lower and higher weights for which we have performance data
            AircraftLowerWeight = aircraft.LowestPossibleWeight;
            AircraftHigherWeight = aircraft.HighestPossibleWeight;

            PerformanceCalculationLog.LogItem foundBookDataLogItem = _logger.NewItem("Looking for book data:");

            foundBookDataLogItem.AddSubItem($"Scenario: {Scenario}");
            foundBookDataLogItem.AddSubItem($"Aircraft: {aircraft.AircraftTypeString}");
            foundBookDataLogItem.AddSubItem($"Actual aircraft weight: {AircraftActualWeight}" + " lbs");
            foundBookDataLogItem.AddSubItem($"Actual pressure altitude: {ActualPressureAltitude}");
            foundBookDataLogItem.AddSubItem($"Actual temperature: {ActualTemperature}" + " °C");

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
                    _pressureAltitudeInterval)
                );

            // if the lower pressure altitude is the same as the actual pressure altitude, 
            // then we don't need to interpolate
            if (_lowerPressureAltitude.TotalFeet == ActualPressureAltitude.TotalFeet)
            {
                _upperPressureAltitude = ActualPressureAltitude;
            }
            else
            {
                _upperPressureAltitude = Distance.FromFeet(
                    _lowerPressureAltitude.TotalFeet + _pressureAltitudeInterval
                    );
            }

            // if the lower temperature is the same as the actual temperature,
            // then we don't need to interpolate
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
            double temperature,
            double weight,
            PerformanceCalculationLog.LogItem foundBookDataLogItem
            )
        {
            BookPerformanceData bookData = _bookPerformanceDataList.FindBookDistance(
                Scenario,
                pressureAltitude,
                temperature,
                weight);

            PerformanceCalculationLog.LogItem item = foundBookDataLogItem.NewItem($"Found book data for: Aircraft weight {weight} lbs, pressure altitude {pressureAltitude} ft, temperature {temperature} °C");

            item.AddSubItem($"Ground roll: {bookData.DistanceGroundRoll}");
            item.AddSubItem($"Distance to clear 50ft: {bookData.DistanceToClear50Ft}");

            return bookData;
        }


        public InterpolatedPerformanceData InterpolateByPressureAltitude(
            PerformanceDataBase lowerPressureAltitudePerformanceData,
            PerformanceDataBase upperPresssureAltitudePerformanceData,
            PerformanceDataBase lowerTemperaturePerformanceData,
            PerformanceDataBase upperTemperaturePerformanceData,
            double weight)
        {
            // first interpolate by pressure altitude
            InterpolatedPerformanceData interpolatedPerformanceDataByPressureAltitude =
                InterpolateByPressureAltitudeOnly(
                    lowerPressureAltitudePerformanceData,
                    upperPresssureAltitudePerformanceData,
                    weight);

            lowerTemperaturePerformanceData.DistanceGroundRoll =
                interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll;

            lowerTemperaturePerformanceData.DistanceToClear50Ft =
                interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft;

            upperTemperaturePerformanceData.DistanceGroundRoll =
                interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll;

            upperTemperaturePerformanceData.DistanceToClear50Ft =
                interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft;

            return InterpolateByTemperatureOnly(
                lowerTemperaturePerformanceData,
                upperTemperaturePerformanceData,
                weight
                );
        }

        public InterpolatedPerformanceData InterpolateByPressureAltitudeOnly(PerformanceDataBase lowerPressureAltitudePerformanceData, PerformanceDataBase upperPressureAltitudePerformanceData, double weight)
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

                PerformanceCalculationLog.LogItem subItem = _logger.NewItem($"No interpolation required for pressure altitude {ActualPressureAltitude} ft");
                subItem.AddSubItem($"Ground roll: {interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll}");
                subItem.AddSubItem($"Distance to clear 50ft: {interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft}");

                return interpolatedPerformanceDataByPressureAltitude;
            }
            if (ActualPressureAltitude.TotalFeet == upperPressureAltitudePerformanceData.PressureAltitude.TotalFeet)
            {
                // if we're at the upper pressure altitude, then we don't need to interpolate and we use the upper data

                interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll = upperPressureAltitudePerformanceData.DistanceGroundRoll;
                interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft = upperPressureAltitudePerformanceData.DistanceToClear50Ft;

                PerformanceCalculationLog.LogItem subItem = _logger.NewItem($"No interpolation required for pressure altitude {ActualPressureAltitude} ft");
                subItem.AddSubItem($"Ground roll: {interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll}");
                subItem.AddSubItem($"Distance to clear 50ft: {interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft}");

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
            double pressureAltitudeInterpolationFactor = PerformanceDataInterpolationUtilities.CalculateInterpolationFactor(
                ActualPressureAltitude.TotalFeet,
                _lowerPressureAltitude.TotalFeet,
                _upperPressureAltitude.TotalFeet
            );

            PerformanceCalculationLog.LogItem item = _logger.NewItem(
                $"Interpolated performance data by pressure altitude for aircraft weight {weight} lbs, temperature {lowerPressureAltitudePerformanceData.Temperature} °C");

            PerformanceCalculationLog.LogItem interpolatingLogItem = item.NewItem($"Interpolating actual pressure altitude {ActualPressureAltitude.ToString()} ft, between {lowerPressureAltitudePerformanceData.PressureAltitude} ft and {upperPressureAltitudePerformanceData.PressureAltitude.ToString()} ft, with interpolation factor {pressureAltitudeInterpolationFactor.ToString("0.00")}");
            interpolatingLogItem.AddSubItem($"Lower temperature is {lowerPressureAltitudePerformanceData.Temperature} °C,, upper temperature is {upperPressureAltitudePerformanceData.Temperature} °C");
            interpolatingLogItem.AddSubItem($"Aircraft weight is {weight} lbs");
            interpolatingLogItem.AddSubItem($"Lower pressure altitude performance data at {lowerPressureAltitudePerformanceData.PressureAltitude.ToString()} ft, ground roll is: {lowerPressureAltitudePerformanceData.DistanceGroundRoll}, distance to clear 50' obstacle is: {lowerPressureAltitudePerformanceData.DistanceToClear50Ft}");
            interpolatingLogItem.AddSubItem($"Upper pressure altitude performance data at {upperPressureAltitudePerformanceData.PressureAltitude.ToString()} ft, ground roll is: {upperPressureAltitudePerformanceData.DistanceGroundRoll}, distance to clear 50' obstacle is: {upperPressureAltitudePerformanceData.DistanceToClear50Ft}");
            item.AddSubItem($"Interpolated by pressure altitude, ground roll is: {interpolatedPerformanceDataByPressureAltitude.DistanceGroundRoll}, distance to clear 50' obstacle is: {interpolatedPerformanceDataByPressureAltitude.DistanceToClear50Ft}");

            return interpolatedPerformanceDataByPressureAltitude;

        }

        public InterpolatedPerformanceData InterpolateByTemperatureOnly(
            PerformanceDataBase lowerTemperaturePerformanceData,
            PerformanceDataBase upperTemperaturePerformanceData,
            double weight)
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

                PerformanceCalculationLog.LogItem logItem = _logger.NewItem($"No interpolation required for temperature {ActualTemperature}");
                logItem.AddSubItem($"Ground roll: {interpolatedPerformanceDataByTemperature.DistanceGroundRoll}");
                logItem.AddSubItem($"Distance to clear 50' obstacle: {interpolatedPerformanceDataByTemperature.DistanceToClear50Ft}");

                return interpolatedPerformanceDataByTemperature;
            }
            else if (ActualTemperature == upperTemperaturePerformanceData.Temperature)
            {
                // if we're at the upper temperature, then we don't need to interpolate and we use the upper data

                interpolatedPerformanceDataByTemperature.DistanceGroundRoll = upperTemperaturePerformanceData.DistanceGroundRoll;
                interpolatedPerformanceDataByTemperature.DistanceToClear50Ft = upperTemperaturePerformanceData.DistanceToClear50Ft;

                PerformanceCalculationLog.LogItem logItem = _logger.NewItem($"No interpolation required for temperature {ActualTemperature}");
                logItem.AddSubItem($"Ground roll: {interpolatedPerformanceDataByTemperature.DistanceGroundRoll}");
                logItem.AddSubItem($"Distance to clear 50' obstacle: {interpolatedPerformanceDataByTemperature.DistanceToClear50Ft}");

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
            double temperatureInterpolationFactor = PerformanceDataInterpolationUtilities.CalculateInterpolationFactor(
                ActualTemperature,
                _lowerTemperature,
                _upperTemperature
            );

            PerformanceCalculationLog.LogItem item = _logger.NewItem($"Interpolated performance data by temperature for aircraft weight {weight}, pressure altitude {lowerTemperaturePerformanceData.PressureAltitude}");

            PerformanceCalculationLog.LogItem interpolatingLogItem = item.NewItem($"Interpolating actual temperature {ActualTemperature} °C, between {lowerTemperaturePerformanceData.Temperature} °C and {upperTemperaturePerformanceData.Temperature} °C, with interpolation factor {temperatureInterpolationFactor}");
            interpolatingLogItem.AddSubItem($"Lower pressure altitude is {lowerTemperaturePerformanceData.PressureAltitude} ft, upper pressure altitude is {upperTemperaturePerformanceData.PressureAltitude} ft");
            interpolatingLogItem.AddSubItem($"Aircraft weight is {weight} lbs");
            interpolatingLogItem.AddSubItem($"Lower temperature performance data at {lowerTemperaturePerformanceData.Temperature} °C: ground roll {lowerTemperaturePerformanceData.DistanceGroundRoll}, distance to clear 50' obstacle {lowerTemperaturePerformanceData.DistanceToClear50Ft}");
            interpolatingLogItem.AddSubItem($"Upper temperature performance data at {upperTemperaturePerformanceData.Temperature} °C: ground roll {upperTemperaturePerformanceData.DistanceGroundRoll}, distance to clear 50' obstacle {upperTemperaturePerformanceData.DistanceToClear50Ft}");
            item.AddSubItem($"Interpolated by temperature, ground roll is: {interpolatedPerformanceDataByTemperature.DistanceGroundRoll}, distance to clear 50' obstacle is: {interpolatedPerformanceDataByTemperature.DistanceToClear50Ft}");

            return interpolatedPerformanceDataByTemperature;
        }

        public InterpolatedPerformanceData InterpolateByAircraftWeight(
             PerformanceDataBase lowerWeightPerformanceData,
             PerformanceDataBase upperWeightPerformanceData,
             double weight)
        {

            InterpolatedPerformanceData interpolatedPerformanceDataByWeight = new InterpolatedPerformanceData(
                this.Scenario,
                ActualPressureAltitude,
                ActualTemperature,
                weight
            );

            double weightInterpolationFactor = GetWeightInterpolationFactor(AircraftActualWeight);

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

            PerformanceCalculationLog.LogItem item = _logger.NewItem("Interpolated weight performance data between:");

            PerformanceCalculationLog.LogItem interpolatingLogItem = item.NewItem($"Interpolating weight {AircraftActualWeight} lbs, between {lowerWeightPerformanceData.AircraftWeight} lbs and {upperWeightPerformanceData.AircraftWeight} lbs, with interpolation factor {weightInterpolationFactor.ToString("0.00")}");
            interpolatingLogItem.AddSubItem($"Lower weight performance data at {lowerWeightPerformanceData.AircraftWeight} lbs: ground roll {lowerWeightPerformanceData.DistanceGroundRoll}, distance to clear 50' obstacle {lowerWeightPerformanceData.DistanceToClear50Ft}");
            interpolatingLogItem.AddSubItem($"Upper weight performance data at {upperWeightPerformanceData.AircraftWeight} lbs: ground roll {upperWeightPerformanceData.DistanceGroundRoll}, distance to clear 50' obstacle {upperWeightPerformanceData.DistanceToClear50Ft}");
            item.AddSubItem($"Interpolated by weight, ground roll is: {interpolatedPerformanceDataByWeight.DistanceGroundRoll}, distance to clear 50' obstacle is: {interpolatedPerformanceDataByWeight.DistanceToClear50Ft}");

            return interpolatedPerformanceDataByWeight;
        }

        /// <summary>
        /// Returns the interpolation factor for the given weight.  The interpolation factor is the percentage distance that the given weight is between the lowest
        /// and highest possible weight for the aircraft
        /// </summary>
        public double GetWeightInterpolationFactor(double weight)
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

            double lowerWeight = AircraftLowerWeight;
            double higherWeight = AircraftHigherWeight;

            // interpolaton factor is the percentage distance that the given weight is between the lowest
            // and highest possible weight for the aircraft
            double weightInterpolationFactor = (weight - lowerWeight) / (higherWeight - lowerWeight);

            return weightInterpolationFactor;
        }



    }
}