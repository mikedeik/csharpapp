using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSharpApp.Core.ValidationRules {
    public class ValidImageUrlsAttribute : ValidationAttribute {

        private static readonly Regex UrlRegex = new(@"^(https?://.*\.(?:png|jpg|jpeg|gif|webp))$", RegexOptions.IgnoreCase);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            if (value is List<string> images) {
                if (!images.Any()) {
                    return new ValidationResult("At least one image URL is required.");
                }

                if (!images.All(url => UrlRegex.IsMatch(url))) {
                    return new ValidationResult("All images must be valid URLs ending in .png, .jpg, .jpeg, .gif, or .webp.");
                }
            }
            return ValidationResult.Success!;
        }
    }
}
