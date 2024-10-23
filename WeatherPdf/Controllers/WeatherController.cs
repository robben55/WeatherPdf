using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherPdf.Dto;
using WeatherPdf.Mappings;
using WeatherPdf.Models;
using WeatherPdf.Services.Caching;

namespace WeatherPdf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _config;

        public WeatherController(IHttpClientFactory http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }


        [HttpGet]
        public async Task<IActionResult> GetWeather(string city)
        {
            var apiKey = _config.GetValue<string>("WeatherApi");
            var client = _http.CreateClient("weather");
            var weatherDto = await client.GetFromJsonAsync<WeatherResponseModel>($"?q={city}&appid={apiKey}&units=metric");
            var mappedWeatherdata = MappingFunctions.CustomMapWeatherToShortInfoDto(weatherDto!);
            return Ok(mappedWeatherdata);   
        }
    }
}
