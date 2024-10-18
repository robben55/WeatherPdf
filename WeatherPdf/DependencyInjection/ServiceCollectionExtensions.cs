using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.RateLimiting;
using WeatherPdf.Database.Context;
using WeatherPdf.Services.Caching;
using WeatherPdf.Services.Pf;
using WeatherPdf.Settings;
using WeatherPdf.Utils;

namespace WeatherPdf.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddScoped<IRedisCacheService, RedisCacheService>();
        services.AddSwaggerGen();
        services.AddOptions<CosmosSettings>()
                    .BindConfiguration(CosmosSettings.ConfigurationSection)
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

        services.AddDbContext<ApplicationContext>((provider, context) =>
        {
            var cosmosSettings = provider.GetRequiredService<IOptions<CosmosSettings>>().Value;            
            context.UseCosmos(cosmosSettings.EndPoint, cosmosSettings.SecurityKey, cosmosSettings.Name);
        });

        services.AddTransient<IGeneratePdf, GeneratePdf>();
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddFixedWindowLimiter("fixedWindow", options =>
            {
                options.Window = TimeSpan.FromMinutes(10);
                options.PermitLimit = 3;
                options.QueueLimit = 0;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });            
        });
        return services;
    }

    public static IServiceCollection AddCaching(
            this IServiceCollection services, IConfiguration configuration)
    {
        var redis = configuration.GetValue<string>("Redis")!;
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString(redis);
            options.InstanceName = "Weather_";
        });

        return services;
    }

    public static IServiceCollection AddHttpClientForWeatherApi(
            this IServiceCollection services
        )
    {
        services.AddHttpClient("weather", (httpClient) =>
        {
            httpClient.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/weather");
        });


        return services;
    }

    public static IServiceCollection AddFluentEmail(
        this IServiceCollection services,
        IConfigurationSection configuration
        )
    {
        var from = configuration.GetValue<string>("From");

        services.AddFluentEmail(from).AddSmtpSender(new SmtpClient()
        {
            Host = configuration.GetValue<string>("Host")!,
            Port = configuration.GetValue<int>("Port"),
            EnableSsl = configuration.GetValue<bool>("Ssl"),
            Credentials = new NetworkCredential(configuration.GetValue<string>("UserName"), configuration.GetValue<string>("Password"))
        });
        return services;
    }

}
