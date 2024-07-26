using System;
using System.Globalization;
using System.Windows.Controls;

namespace TaskTracker.Validation
{
    public class DateValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string strValue = Convert.ToString(value, cultureInfo);

            if (string.IsNullOrEmpty(strValue))
                return new ValidationResult(true, null);
            
            if (!DateTime.TryParse(strValue, out DateTime _))
                return new ValidationResult(false, "Value is not a valid date");
            return new ValidationResult(true, null);
        }
    }
}
