using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AccessControlModels
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FutureDateCorrectRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dateTimeValue = (DateTime)value;
            if(dateTimeValue.Date >= DateTime.Now.Date)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Date must be later");
        }
    }
}
