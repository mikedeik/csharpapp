using CSharpApp.Core.ValidationRules;
using System.ComponentModel.DataAnnotations;


namespace CSharpApp.Core.Dtos.Categories {
    public class CategoryMutateDto {
        [JsonPropertyName("name")]
        [Required(ErrorMessage = "Name property is required")]
        [MinLength(1, ErrorMessage = "Name cannot be empty")]
        public string? Name { get; set; }

        [JsonPropertyName("image")]
        [Required(ErrorMessage = "Image property is required")]
        [ValidImageUrl(ErrorMessage = "The image URL is not valid.")]
        public string? Image { get; set; }
    }
}
