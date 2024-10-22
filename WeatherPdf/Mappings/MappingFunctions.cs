using Mapster;
using WeatherPdf.Dto;
using WeatherPdf.Responses;

namespace WeatherPdf.Mappings;

public static class MappingFunctions
{
    public static WeatherDto CustomMapWeatherToShortInfoDto(WeatherResponse weather)
    {
        var dto = weather.Adapt<WeatherDto>();
        return dto;
    }
}
