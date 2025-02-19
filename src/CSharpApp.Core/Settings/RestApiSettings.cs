using System.ComponentModel.DataAnnotations;

namespace CSharpApp.Core.Settings;

public sealed class RestApiSettings
{
    [Required]
    public string? BaseUrl { get; set; }
    [Required]
    public string? Products { get; set; }
    [Required]
    public string? Categories { get; set; }
    [Required]
    public string? Auth { get; set; }
    [Required]
    public string? UserAvailable { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    [Required]
    public string? APIName { get; set; }
}