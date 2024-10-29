using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Globalization;
using WeatherPdf.Database.Context;
using WeatherPdf.Models.ResponseModels;
using WeatherPdf.Pdf.Weather.ContentModels;
using WeatherPdf.Services.Pf;
using WeatherPdf.Utils;

namespace WeatherPdf.Routes;

public static class RoutesCollection
{
    public static void MapReportEndPoints(this RouteGroupBuilder group)
    {
        group.MapGet("/monthly", async (string? yourEmail, ApplicationContext context, IGeneratePdf pdf, IFluentEmail email) =>
        {
            var (startDateTime, endDateTime) = DateHelper.GetPreviousMonthDateRange();
            var weatherReport = await context.WeatherDatas.Where(x => x.SearchedTime >= startDateTime && x.SearchedTime <= endDateTime).ToListAsync();
            var forHeader = new HeaderContent("1", endDateTime.Day.ToString(), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(endDateTime.Month));
            var content = pdf.CreatePdf(weatherReport, forHeader).GeneratePdf();


            if (yourEmail is not null)
            {
                await email.To(yourEmail).Subject("Weather report").Body("Tashkent weather report for previous month").Attach(new Attachment
                {
                    ContentType = "application/pdf",
                    Data = new MemoryStream(content),
                    Filename = $"Report for {endDateTime.Month}th month"

                }).SendAsync();

                return Results.Ok("Email report has been sent to your email");
            }


            return Results.File(content, "application/pdf", "weather-report.pdf");
        })
          .WithSummary("It shows weather for previous month")
          .WithOpenApi();
    }

    public static void MapWeatherEndPoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (string city, IHttpClientFactory _http, IConfiguration _config) =>
        {
            try
            {
                var apiKey = _config.GetValue<string>("WeatherApi");
                var client = _http.CreateClient("weather");
                var weatherDto = await client.GetFromJsonAsync<WeatherResponseModel>($"?q={city}&appid={apiKey}&units=metric");
                return Results.Ok(weatherDto);
            }

            catch (HttpRequestException ex)
            {
                var statusCode = (int)ex.StatusCode!.Value;
                var message = statusCode switch
                {
                    401 => "Wrong api key",
                    429 => "Too many requests",
                    404 => "Wrong specified city name",
                    _ => "Something wrong happened. Try again later"
                };
                return Results.Json(new { Message = message }, statusCode: statusCode);
            }
        }).RequireRateLimiting("fixedWindow");
    }
}

