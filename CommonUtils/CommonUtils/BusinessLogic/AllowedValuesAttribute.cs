using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CommonUtils {
    public class AllowedValuesAttribute : ValidationAttribute {
        public string[] ValidValues { get; set; }

        public AllowedValuesAttribute(params string[] _validValues) {
            ValidValues = _validValues;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            return value == null || ValidValues.Contains((string)value) ?
                ValidationResult.Success :
                new ValidationResult(string.Format("Invalid value. Allowed values: {0}", string.Join(",", ValidValues)));
        }
    }
}
