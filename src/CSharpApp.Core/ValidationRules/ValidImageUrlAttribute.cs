using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSharpApp.Core.ValidationRules {
    internal class ValidImageUrlAttribute: ValidationAttribute {
        private static readonly Regex UrlRegex = new(@"^(https?://.*)$", RegexOptions.IgnoreCase);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            if (value is string url && !UrlRegex.IsMatch(url)) {
                return new ValidationResult("The URL is not valid.");
            }
            return ValidationResult.Success!;
        }
    }
}
