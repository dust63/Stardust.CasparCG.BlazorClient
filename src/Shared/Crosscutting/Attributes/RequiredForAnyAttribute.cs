using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stardust.Flux.Crosscutting.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RequiredForAnyAttribute : ValidationAttribute
    {
        /// <summary>
        /// Values of the <see cref="PropertyName"/> that will trigger the validation
        /// </summary>
        public string[] Values { get; set; }

        /// <summary>
        /// Independent property name
        /// </summary>
        public string PropertyName { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance;
            if (model == null || Values == null)
            {
                return ValidationResult.Success;
            }

            var currentValue = model.GetType().GetProperty(PropertyName)?.GetValue(model, null)?.ToString();
            if (Values.Contains(currentValue) && value == null)
            {
                var propertyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);
                return new ValidationResult($"{propertyInfo.Name} is required for the current {PropertyName} value {currentValue}");
            }
            return ValidationResult.Success;
        }
    }
}
