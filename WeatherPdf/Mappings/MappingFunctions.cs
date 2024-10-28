using Mapster;
using WeatherPdf.Models.Dtos;
using WeatherPdf.Models.ResponseModels;

namespace WeatherPdf.Mappings;

public static class MappingFunctions
{
    public static WeatherDto CustomMapWeatherToShortInfoDto(WeatherResponseModel weather)
    {
        var dto = weather.Adapt<WeatherDto>();
        return dto;
    }
}
