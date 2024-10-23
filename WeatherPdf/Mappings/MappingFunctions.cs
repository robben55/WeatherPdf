using Mapster;
using WeatherPdf.Models;
using WeatherPdf.Models.Dtos;

namespace WeatherPdf.Mappings;

public static class MappingFunctions
{
    public static WeatherDto CustomMapWeatherToShortInfoDto(WeatherResponseModel weather)
    {
        var dto = weather.Adapt<WeatherDto>();
        return dto;
    }
}
