using System.ComponentModel.DataAnnotations;

namespace WeatherPdf.Settings;

public sealed class CosmosSettings
{
    public const string ConfigurationSection = "CosmosSection";
    [Required]
    public string EndPoint { get; init; } = string.Empty;
    [Required]
    public string SecurityKey { get;init; } = string.Empty;
    [Required]
    public string Name { get; init ; } = string.Empty;
}
