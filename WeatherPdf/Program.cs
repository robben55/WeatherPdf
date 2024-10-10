using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Globalization;
using WeatherPdf.Database.Context;
using WeatherPdf.DependencyInjection;
using WeatherPdf.Pdf.Weather.ContentModels;
using WeatherPdf.Services.Pf;
using WeatherPdf.Utils;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddServices();

var app = builder.Build();


app.MapGet("monthly-report", async (ApplicationContext context, IGeneratePdf pdf) =>
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
    return Results.File(content, "application/pdf", "weather-report.pdf");
})
    .WithSummary("It shows weather for previous month")
    .WithOpenApi();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

