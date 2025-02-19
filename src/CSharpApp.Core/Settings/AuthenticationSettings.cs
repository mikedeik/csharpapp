using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Core.Settings {
    public class AuthenticationSettings {
        [Required]
        public string? Key { get; set; }
        [Required]
        public string? Issuer { get; set; }
        [Required]
        public string? Audience { get; set; }
        [Required]
        public int AccessTokenExpirationMinutes { get; set; }
        [Required]
        public int RefreshTokenExpirationDays { get; set; }

    }
}
