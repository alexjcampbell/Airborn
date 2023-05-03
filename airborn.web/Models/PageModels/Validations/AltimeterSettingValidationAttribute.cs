
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

        public double AltimeterSetting { get; }

        public string GetErrorMessage(double minimum, double maximum, AltimeterSettingType type) =>
            $"{AltimeterSetting} is not a valid altimeter setting. It must be between {minimum} and {maximum} for {type}.";

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            var pageModel = (CalculatePageModel)validationContext.ObjectInstance;
            if (value == null)
            {
                return new ValidationResult("Altimeter setting is required.");
            }
            var altimeterSetting = ((double)value!);

            double minimumHG = 28.0f;
            double maximumHG = 31.0f;

            double minimumMB = 950f;
            double maximumMB = 1050f;

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