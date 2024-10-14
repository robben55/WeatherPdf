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

app.MapGroup("/v1/report/pdf").MapReportEndPoints();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.Run();

