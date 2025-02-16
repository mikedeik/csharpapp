using CSharpApp.Core.ValidationRules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Core.Dtos.Products {
    public class ProductUpdateDto {
        [JsonPropertyName("title")]
        [MinLength(1, ErrorMessage = "Title cannot be empty.")]
        public string? Title { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        [JsonPropertyName("price")]
        public int? Price { get; set; }

        [MinLength(1, ErrorMessage = "Description cannot be empty.")]
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("images")]
        [ValidImageUrls(ErrorMessage = "All images must be valid URLs.")]
        public List<string>? Images { get; set; }
    }
}
