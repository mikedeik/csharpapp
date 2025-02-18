
using System.ComponentModel.DataAnnotations;

namespace CSharpApp.Core.Dtos.Auth {
    public class LoginCredentials {
        [Required]
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }
}
