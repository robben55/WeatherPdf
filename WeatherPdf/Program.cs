using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Globalization;
using WeatherPdf.Database.Context;
using WeatherPdf.Pdf.Weather.ContentModels;
using WeatherPdf.Services;
using WeatherPdf.Settings;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<CosmosSettings>()
            .BindConfiguration(CosmosSettings.ConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

builder.Services.AddDbContext<ApplicationContext>((provider, context) =>
{
    var cosmosSettings = provider.GetRequiredService<IOptions<CosmosSettings>>().Value;
    context.UseCosmos(cosmosSettings.EndPoint, cosmosSettings.SecurityKey, cosmosSettings.Name);
});

builder.Services.AddTransient<IGeneratePdf, GeneratePdf>();

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

