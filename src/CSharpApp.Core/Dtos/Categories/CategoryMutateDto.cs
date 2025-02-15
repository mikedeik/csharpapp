using CSharpApp.Core.ValidationRules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Core.Dtos.Categories {
    public class CategoryMutateDto {
        [JsonPropertyName("name")]
        [Required(ErrorMessage = "Name property is required")]
        [MinLength(1, ErrorMessage = "Name cannot be empty")]
        public string? Name { get; set; }

        [JsonPropertyName("image")]
        [ValidImageUrl(ErrorMessage = "The image URL is not valid.")]
        public string? Image { get; set; }
    }
}
