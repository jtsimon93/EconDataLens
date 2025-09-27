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
                
                Console.WriteLine($"Connection string: {connStr}");

                services.AddDbContext<EconDataLensDbContext>(options =>
                    options.UseNpgsql(connStr)
                    );
                
                services.AddHttpClient<IFileDownloadService, BasicFileDownloadService>()
                    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
                    {
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                    });

            })
            .Build();

        // This is just a basic proof of concept to test the database connection and download
        Console.WriteLine("Hello. Connecting to database...");

        // Resolve DbContext and try a connection
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EconDataLensDbContext>();
        
        // Resolve file download service and try a download
        var downloader = scope.ServiceProvider.GetRequiredService<IFileDownloadService>();
        

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
    }
}