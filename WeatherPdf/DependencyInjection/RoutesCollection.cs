using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System.Globalization;
using WeatherPdf.Database.Context;
using WeatherPdf.Pdf.Weather.ContentModels;
using WeatherPdf.Services.Pf;
using WeatherPdf.Utils;

namespace WeatherPdf.DependencyInjection;

public static class RoutesCollection
{
    public static void MapReportEndPoints(this RouteGroupBuilder group)
    {
        group.MapGet("/monthly", async (string? yourEmail, ApplicationContext context, IGeneratePdf pdf, IFluentEmail email) =>
        {
            var (startDateTime, endDateTime) = DateHelper.GetPreviousMonthDateRange();
            var weatherReport = await context.WeatherDatas.Where(x => x.SearchedTime >= startDateTime && x.SearchedTime <= endDateTime).ToListAsync();
            var forHeader = new HeaderContent()
            {
                From = "1",
                To = endDateTime.Day.ToString(),
                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(endDateTime.Month)
            };
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
}



