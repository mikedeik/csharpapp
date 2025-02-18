using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Core.Dtos.Auth {
    public class RefreshDto {

        [Required]
        [JsonPropertyName("refreshToken")]
        public string? RefreshToken { get; set; }
    }
}
