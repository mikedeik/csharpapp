using CSharpApp.Core.ValidationRules;
using System.ComponentModel.DataAnnotations;


namespace CSharpApp.Core.Dtos.Products {
    public class ProductCreateDto {
        [JsonPropertyName("title")]
        [Required(ErrorMessage = "Title is required.")]
        [MinLength(1, ErrorMessage = "Title cannot be empty.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        [JsonPropertyName("price")]
        public int? Price { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MinLength(1, ErrorMessage = "Description cannot be empty.")]
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "CategoryId is required.")]
        [JsonPropertyName("categoryId")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Images array is required")]
        [JsonPropertyName("images")]
        [ValidImageUrls(ErrorMessage = "All images must be valid URLs.")]
        public List<string>? Images { get; set; } = [];
    }
}
