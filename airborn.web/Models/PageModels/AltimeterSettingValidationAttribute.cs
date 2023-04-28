
using System;
using System.ComponentModel.DataAnnotations;
using Airborn.web.Models;

namespace Airborn.web.Models
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AltimeterSettingAttribute : ValidationAttribute
    {
        public AltimeterSettingAttribute()
        {

        }

        public decimal AltimeterSetting { get; }

        public string GetErrorMessage(decimal minimum, decimal maximum, AltimeterSettingType type) =>
            $"{AltimeterSetting} is not a valid altimeter setting. It must be between {minimum} and {maximum} for {type}.";

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            var pageModel = (CalculatePageModel)validationContext.ObjectInstance;
            if (value == null)
            {
                return new ValidationResult("Altimeter setting is required.");
            }
            var altimeterSetting = ((decimal)value!);

            decimal minimumHG = 28.0m;
            decimal maximumHG = 31.0m;

            decimal minimumMB = 950m;
            decimal maximumMB = 1050m;

            if (pageModel.AltimeterSettingType.Value == AltimeterSettingType.HG &&
                (altimeterSetting < minimumHG || altimeterSetting > maximumHG))
            {
                return new ValidationResult(GetErrorMessage(minimumHG, maximumHG, pageModel.AltimeterSettingType.Value));
            }
            else if (pageModel.AltimeterSettingType.Value == AltimeterSettingType.MB &&
                (altimeterSetting < minimumMB || altimeterSetting > maximumMB))
            {
                return new ValidationResult(GetErrorMessage(minimumMB, maximumMB, pageModel.AltimeterSettingType.Value));
            }

            return ValidationResult.Success;
        }
    }
}