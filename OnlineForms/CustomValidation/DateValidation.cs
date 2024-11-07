using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Diagnostics;

namespace OnlineForms.CustomValidation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class DateOrderAttributes : ValidationAttribute, IClientValidatable
    {
        private readonly string propertyName;
        private readonly bool allowEqualDates;

        public DateOrderAttributes(string propertyName, bool allowEqualDates = false)
        {
            this.propertyName = propertyName;
            this.allowEqualDates = allowEqualDates;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyInfo = validationContext.ObjectType.GetProperty(this.propertyName);
            if (propertyInfo == null)
            {
                return new ValidationResult( string.Format("unknown property {0}", this.propertyName));
            }

            var propertyValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);

            if((DateTime)value >= (DateTime)propertyValue)
            {
                if(this.allowEqualDates && value.Equals(propertyValue))
                {
                    return ValidationResult.Success;
                }else if((DateTime)value > (DateTime)propertyValue)
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule mvr = new ModelClientValidationRule();
            mvr.ErrorMessage = this.ErrorMessageString;
            mvr.ValidationType = "dateorder";

            mvr.ValidationParameters["propertytested"] = this.propertyName;
            mvr.ValidationParameters["allowequaldates"] = this.allowEqualDates;

            return new[] { mvr };
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class BeforeTodayAttributes : ValidationAttribute, IClientValidatable
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dt = (DateTime)value;
            if(dt >= DateTime.UtcNow)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? "Selected date must be greater than today");
        }
            public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule mvr = new ModelClientValidationRule();
            mvr.ErrorMessage = this.ErrorMessageString;
            mvr.ValidationType = "beforetoday";

            return new[] { mvr };
        }
    }
}