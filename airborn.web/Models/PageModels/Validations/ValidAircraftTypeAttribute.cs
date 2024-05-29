
using System;
using System.ComponentModel.DataAnnotations;
using Airborn.web.Models;

namespace Airborn.web.Models
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidAircraftTypeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            AircraftType aircraftType;

            if (value == null || !Enum.TryParse(value.ToString(), true, out aircraftType))
            {
                return new ValidationResult("Aircraft Type is required.");
            }

            return ValidationResult.Success;
        }
    }
}