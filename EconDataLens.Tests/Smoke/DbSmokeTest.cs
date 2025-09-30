using Microsoft.EntityFrameworkCore;
using EconDataLens.Core.Entities.Cpi;
using EconDataLens.Data;

namespace EconDataLens.Tests.Smoke;

public class DbSmokeTests
{
    private static EconDataLensDbContext CreateDb()
    {
        var opts = new DbContextOptionsBuilder<EconDataLensDbContext>()
            .UseNpgsql(PostgresFixture.ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new EconDataLensDbContext(opts);
    }

    [Test]
    public async Task Can_Migrate_And_Do_Basic_CRUD()
    {
        await using var db = CreateDb();

        // Insert
        db.CpiArea.Add(new CpiArea { AreaCode = "ZZZ0", AreaName = "Test Area" });
        await db.SaveChangesAsync();

        // Query
        var found = await db.CpiArea.AsNoTracking().CountAsync(a => a.AreaCode == "ZZZ0");
        Assert.That(found, Is.EqualTo(1));

        // Cleanup (optional)
        db.CpiArea.RemoveRange(db.CpiArea.Where(a => a.AreaCode == "ZZZ0"));
        await db.SaveChangesAsync();
    }
}