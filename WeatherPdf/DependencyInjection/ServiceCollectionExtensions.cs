using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using WeatherPdf.Database.Context;
using WeatherPdf.Models.Dtos;
using WeatherPdf.Models.ResponseModels;
using WeatherPdf.Services.Email;
using WeatherPdf.Services.Pf;
using WeatherPdf.Services.Security;

namespace WeatherPdf.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddTransient<IGeneratePdf, GeneratePdf>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddSingleton<TokenGenerator>();
        return services;
    }

    public static IServiceCollection AddIdentitySettings(
        this IServiceCollection services)
    {
        services.AddDefaultIdentity<IdentityUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;


            options.SignIn.RequireConfirmedEmail = true;

            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;

            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;

            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;

        }).AddEntityFrameworkStores<ApplicationContext>()
          .AddPasswordValidator<Services.Security.PasswordValidator<IdentityUser>>();



        return services;
    }

    public static IServiceCollection AddAuthenticationSettings(
        this IServiceCollection services, IConfiguration config)
    {

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(x =>
            {
                byte[] _key = Encoding.UTF8.GetBytes(config.GetValue<string>("Secret")!);
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(_key.ToArray()),
                    ValidIssuer = config.GetSection("BearerSection:ValidIssuer").Value,
                    ValidAudience = config.GetSection("BearerSection:ValidAudience").Value,
                    ValidateIssuerSigningKey = config.GetValue<bool>("BearerSection:ValidateIssuerSigningKey"),
                    ValidateLifetime = config.GetValue<bool>("BearerSection:ValidateLifetime"),
                    ValidateIssuer = config.GetValue<bool>("BearerSection:ValidateIssuer"),
                    ValidateAudience = config.GetValue<bool>("BearerSection:ValidateAudience")
                };
            });

        return services;
    }

    public static IServiceCollection AddDatabaseService(
            this IServiceCollection services,
            IConfiguration config
        )
    {
        var connectionString = config.GetValue<string>("DatabaseCredentials");
        services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connectionString));


        return services;
    }

    public static IServiceCollection AddRateLimiterService(
            this IServiceCollection services
        )
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, token) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    var response = new
                    {
                        Reason = "Too many attempts",
                        TimeLeft = $"{retryAfter} minutes"
                    };

                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(response), cancellationToken: token);
                }
                else
                {
                    var response = new
                    {
                        Reason = "Too many attempts",
                        TimeLeft = $"{retryAfter} minutes"
                    };

                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(response), cancellationToken: token);
                }
            };


            options.AddPolicy(policyName: "limit", partitioner: httpContext =>
            {
                var accessToken = httpContext.Features.Get<IAuthenticateResultFeature>()?
                              .AuthenticateResult?.Properties?.GetTokenValue("access_token")?.ToString()
                          ?? string.Empty;

                if (accessToken is not "") //svoy
                {
                    return RateLimitPartition.GetFixedWindowLimiter(accessToken, _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(1),
                            PermitLimit = 5,
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        });
                }


                return RateLimitPartition.GetFixedWindowLimiter(accessToken, _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(5),
                            PermitLimit = 3,
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        });
            });
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

    public static IServiceCollection RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<WeatherResponseModel, WeatherDto>
            .NewConfig()
            .Map(x => x.Country, z => z.Name)
            .Map(x => x.Temperature, z => z.Main!.Temp)
            .Map(x => x.Pressure, z => z.Main!.Pressure)
            .Map(x => x.Humidity, z => z.Main!.Humidity);
        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        return services;
    }

}
