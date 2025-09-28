using System.Net;
using EconDataLens.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EconDataLens.Data;
using EconDataLens.Services;
using EconDataLens.Core.Configuration;
using EconDataLens.Repositories;

namespace EconDataLens.Etl;

public class Program
{
    public static async Task Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                // Loads appsettings.json and environment variables
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                // Register DbContext with connection string from appsettings.json
                var connStr = context.Configuration.GetConnectionString("Postgres");

                services.AddDbContext<EconDataLensDbContext>(options =>
                    options.UseNpgsql(connStr)
                    );

                services.Configure<DownloadOptions>(context.Configuration.GetSection("DownloadOptions"));
                services.Configure<BlsOptions>(context.Configuration.GetSection("BlsOptions"));
                
                services.AddHttpClient<IFileDownloadService, BasicFileDownloadService>()
                    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
                    {
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                    });

                services.AddScoped<ICpiRepository, CpiRepository>();
                services.AddScoped<ICpiDataFileParser, CpiDataFileParser>();
                services.AddScoped<ICpiIngestionService, CpiIngestionService>();
                services.AddScoped<ICpiEtlService, CpiEtlService>();

            })
            .Build();

        Console.WriteLine("🔧 Starting ETL process...");

        // Resolve DbContext and try a connection
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EconDataLensDbContext>();
        var cpiEtlService = scope.ServiceProvider.GetRequiredService<ICpiEtlService>();

        try
        {
            db.Database.Migrate();

            Console.WriteLine("✅ Database connection successful.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Failed to connect to database:");
            Console.WriteLine(ex.Message);
        }
        
        try
        {
            Console.WriteLine("Running CPI ETL...");
            await cpiEtlService.RunEtlAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Failed to import CPI Areas:");
            Console.WriteLine(ex.Message);
        }
    }
}