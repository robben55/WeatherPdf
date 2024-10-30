using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WeatherPdf.Database.Entities;

namespace WeatherPdf.Database.Context;

public class ApplicationContext : IdentityDbContext
{
    public DbSet<WeatherData> WeatherDatas { get; set; } = null!;
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

    }
}
