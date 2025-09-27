using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace EconDataLens.Data;

public class EconDataLensDbContextFactory : IDesignTimeDbContextFactory<EconDataLensDbContext>
{
    public EconDataLensDbContext CreateDbContext(string[] args)
    {
        // Resolve configuration (supports appsettings, env vars, user secrets)
        var basePath = Directory.GetCurrentDirectory();
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()            // e.g. ConnectionStrings__Postgres
            .Build();

        // Connection string fallbacks
        var conn = config.GetConnectionString("Postgres");

        var options = new DbContextOptionsBuilder<EconDataLensDbContext>()
            .UseNpgsql(conn, npgsql =>
            {
            })
            .Options;

        return new EconDataLensDbContext(options);
    }
}