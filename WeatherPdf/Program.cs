using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Globalization;
using WeatherPdf.Database.Context;
using WeatherPdf.DependencyInjection;
using WeatherPdf.Pdf.Weather.ContentModels;
using WeatherPdf.Services;
using WeatherPdf.Settings;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddServices();

var app = builder.Build();


app.MapGet("monthly-report", async (ApplicationContext context, IGeneratePdf pdf) =>
{
    var currentYear = DateTime.Now.Year;
    var month = DateTime.Now.Month;
    var end = DateTime.DaysInMonth(currentYear, month);

    var startDateTime = new DateTime(currentYear, month, 1);
    var endDateTime = new DateTime(currentYear, month, end);

    var weatherReport = await context.WeatherDatas.Where(x => x.SearchedTime >= startDateTime && x.SearchedTime <= endDateTime).ToListAsync();
    var forHeader = new HeaderContent()
    {
        From = "1",
        To = end.ToString(),
        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month)
    };
    var content = pdf.CreatePdf(weatherReport, forHeader).GeneratePdf();
    return Results.File(content, "application/pdf", "weather-report.pdf");
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

