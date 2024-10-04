using Microsoft.EntityFrameworkCore;
using WeatherPdf.Database.Entities;

namespace WeatherPdf.Database.Context;

public class ApplicationContext : DbContext
{
    public DbSet<WeatherData> WeatherDatas { get; set; } = null!;
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WeatherData>()
            .ToContainer("weather")
            .HasPartitionKey(x => x.Id);

    }
}
