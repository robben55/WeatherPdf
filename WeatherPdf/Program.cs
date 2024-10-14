using FluentEmail.Core;
using FluentEmail.Core.Models;
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


builder.Services.AddServices().AddFluentEmail(builder.Configuration.GetSection("Email"));

var app = builder.Build();

app.MapGet("monthly-report", async (string? yourEmail, ApplicationContext context, IGeneratePdf pdf, IFluentEmail email) =>
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

    if(yourEmail is not null)
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






if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.Run();

