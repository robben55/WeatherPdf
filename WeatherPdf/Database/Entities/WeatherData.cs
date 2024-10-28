namespace WeatherPdf.Database.Entities;

public class WeatherData
{
    public int Id { get; set; }
    public string Country { get; set; } = string.Empty;
    public string Temperature { get; set; } = string.Empty;
    public string Pressure { get; set; } = string.Empty;
    public string Humidity { get; set; } = string.Empty;
    public string SunriseTime { get; set; } = string.Empty;
    public string SunsetTime { get; set; } = string.Empty;
    public DateTime SearchedTime { get; set; }
}
