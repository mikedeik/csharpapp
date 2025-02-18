using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Core.Dtos.Auth {
    public class UserAvailableDto {

        [JsonPropertyName("isAvailable")]
        public bool IsAvailable { get; set; }
    }
}
