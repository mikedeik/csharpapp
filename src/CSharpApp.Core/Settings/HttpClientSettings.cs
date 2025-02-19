using System.ComponentModel.DataAnnotations;

namespace CSharpApp.Core.Settings;

public sealed class HttpClientSettings
{
    [Required]
    public int LifeTime { get; set; }
    [Required]
    public int RetryCount { get; set; }
    [Required]
    public int SleepDuration { get; set; }
}