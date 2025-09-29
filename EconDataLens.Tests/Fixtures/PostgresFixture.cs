using EconDataLens.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace EconDataLens.Tests.Fixtures;

[SetUpFixture]
public class PostgresFixture
{
    private PostgreSqlContainer _container = default!;
    public string ConnectionString => _container.GetConnectionString();

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:17-alpine")
            .WithDatabase("econ_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true)
            .Build();

        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<EconDataLensDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var ctx = new EconDataLensDbContext(options);
        await ctx.Database.MigrateAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _container.DisposeAsync();
    }
}