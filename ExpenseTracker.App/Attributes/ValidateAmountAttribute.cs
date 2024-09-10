using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.App.Attribute
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ValidateAmountAttribute : ValidationAttribute
    {

        private readonly bool _isRequired;

        public ValidateAmountAttribute(bool isRequired = false)
        {
            _isRequired = isRequired;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null && !_isRequired)
            {
                return ValidationResult.Success;
            }

            if (value == null && _isRequired)
            {
                return new ValidationResult("There cannot be an empty value in the amount");
            }

            if (value is decimal decQ)
            {
                if (decQ > 0 && decQ <= decimal.MaxValue)
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult($"Incorrect amount");
            }

            return new ValidationResult("Invalid data type for value amount.");

        }

    }
}
