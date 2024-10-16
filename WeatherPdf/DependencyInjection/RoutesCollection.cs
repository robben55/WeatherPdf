using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Configuration;
using System.Globalization;
using WeatherPdf.Database.Context;
using WeatherPdf.Dto;
using WeatherPdf.Pdf.Weather.ContentModels;
using WeatherPdf.Services.Pf;
using WeatherPdf.Utils;
using static System.Net.WebRequestMethods;

namespace WeatherPdf.DependencyInjection;

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
          .WithOpenApi()
          .RequireRateLimiting("fixedWindow");
    }



    public static void MapWeatherEndPoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (string city, IHttpClientFactory _http, IConfiguration _config) =>
        {
            var apiKey = _config.GetValue<string>("WeatherApi");
            var client = _http.CreateClient("weather");
            var weatherDto = await client.GetFromJsonAsync<WeatherDto>($"?q={city}&appid={apiKey}&units=metric");

            return Results.Ok(weatherDto);
        });
    }
}



