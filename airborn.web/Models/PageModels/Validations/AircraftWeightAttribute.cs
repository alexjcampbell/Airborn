
using System;
using System.ComponentModel.DataAnnotations;
using Airborn.web.Models;

namespace Airborn.web.Models
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidAircraftWeightAttribute : ValidationAttribute
    {
        public ValidAircraftWeightAttribute()
        {

        }

        public double AircraftWeight { get; }

        public string GetErrorMessage(double minimum, double maximum, string type) =>
            $"{AircraftWeight} lbs is not a valid weight. It must be between {minimum} lbs and {maximum} lbs for {type}.";

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            var pageModel = (CalculatePageModel)validationContext.ObjectInstance;
            if (value == null)
            {
                return new ValidationResult("Aircraft weight is required.");
            }
            if (pageModel.AircraftType == null || pageModel.AircraftType == "")
            {
                return ValidationResult.Success;
            }   

            var aircraftWeight = ((int)value!);

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(pageModel.AircraftType);

            double minimum = aircraft.LowestPossibleWeight;
            double maximum = aircraft.HighestPossibleWeight;

            if ((aircraftWeight < minimum || aircraftWeight > maximum))
            {
                return new ValidationResult(GetErrorMessage(minimum, maximum, aircraft.AircraftTypeString));
            }

            return ValidationResult.Success;
        }
    }
}