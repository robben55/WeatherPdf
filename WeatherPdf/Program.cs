using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Globalization;
using WeatherPdf.Database.Context;
using WeatherPdf.DependencyInjection;
using WeatherPdf.Mappings;
using WeatherPdf.Pdf.Weather.ContentModels;
using WeatherPdf.Responses;
using WeatherPdf.Services.Pf;
using WeatherPdf.Utils;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

var services = builder.Configuration;
builder.Services
    .AddServices()
    .AddFluentEmail(builder.Configuration.GetSection("Email"))
    .AddHttpClientForWeatherApi()
    .RegisterMapsterConfiguration();

var app = builder.Build();

app.MapGroup("/v1/pdf-report").MapReportEndPoints();
app.MapGroup("v1/weather").MapWeatherEndPoint();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.Run();

