
using System;
using System.ComponentModel.DataAnnotations;
using Airborn.web.Models;

namespace Airborn.web.Models
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AircraftWeightAttribute : ValidationAttribute
    {
        public AircraftWeightAttribute()
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
            var aircraftWeight = ((int)value!);

            Aircraft aircraft = Aircraft.GetAircraftFromAircraftType(pageModel.AircraftType);

            double minimum = aircraft.GetLowerWeight();
            double maximum = aircraft.GetHigherWeight();

            if ((aircraftWeight < minimum || aircraftWeight > maximum))
            {
                return new ValidationResult(GetErrorMessage(minimum, maximum, aircraft.GetAircraftTypeString()));
            }

            return ValidationResult.Success;
        }
    }
}