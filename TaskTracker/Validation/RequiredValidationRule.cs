using System;
using System.Globalization;
using System.Windows.Controls;

namespace TaskTracker.Validation
{
    public class RequiredValidationRule : ValidationRule
    {
        public Type ValidationType { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string strValue = Convert.ToString(value, cultureInfo);

            if (string.IsNullOrEmpty(strValue))
                return new ValidationResult(false, "Value is required");
            return new ValidationResult(true, null);
        }
    }
}
