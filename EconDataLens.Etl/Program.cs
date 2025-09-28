using System;
using System.Net;
using System.Net.Http.Headers;
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
    public static void Main(string[] args)
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

            })
            .Build();

        Console.WriteLine("🔧 Starting ETL process...");

        // Resolve DbContext and try a connection
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EconDataLensDbContext>();
        var cpiIngestionService = scope.ServiceProvider.GetRequiredService<ICpiIngestionService>();

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
        
        // Demonstration purposes, call CPI Area Import
        try
        {
            cpiIngestionService.ImportAreasAsync().GetAwaiter().GetResult();
            Console.WriteLine("✅ CPI Areas imported successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Failed to import CPI Areas:");
            Console.WriteLine(ex.Message);
        }
    }
}