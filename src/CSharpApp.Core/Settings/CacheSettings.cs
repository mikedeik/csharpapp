using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Core.Settings {
    public class CacheSettings {
        [Required]
        public string? ProductsKey { get; set; }
        [Required]
        public int CahceMinutesDurationProducts { get; set; }
        [Required]
        public string? CategoriesKey { get; set; }
        [Required]
        public int CacheMinutesDurationCategories { get; set; }
    }
}
