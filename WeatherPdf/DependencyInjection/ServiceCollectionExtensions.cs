using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WeatherPdf.Database.Context;
using WeatherPdf.Services.Pf;
using WeatherPdf.Settings;

namespace WeatherPdf.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
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

        return services;
    }
}
