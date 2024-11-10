using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Net;
using WeatherPdf.Mappings;
using WeatherPdf.Models.ResponseModels;

namespace WeatherPdf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetCityWeatherController : ControllerBase
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _config;

        public GetCityWeatherController(IHttpClientFactory http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }


        [HttpGet]
        [EnableRateLimiting("limit")]
        public async Task<IActionResult> GetWeather(string city)
        {

            try
            {
                var apiKey = _config.GetValue<string>("WeatherApi");
                var client = _http.CreateClient("weather");
                var weatherDto = await client.GetFromJsonAsync<WeatherResponseModel>($"?q={city}&appid={apiKey}&units=metric");
                var mappedWeatherdata = MappingFunctions.CustomMapWeatherToShortInfoDto(weatherDto!);
                return Ok(mappedWeatherdata);
            }

            catch(HttpRequestException  ex)
            {

                var statusCode = (int)ex.StatusCode!.Value;
                var message = statusCode switch
                {
                    401 => "Wrong api key",
                    429 => "Too many requests",
                    404 => "Wrong specified city name",
                    _ => "Something wrong happened. Try again later"
                };
                return Problem(type: "Bad Request", title: message, statusCode: StatusCodes.Status400BadRequest);
            }
        }
    }
}
