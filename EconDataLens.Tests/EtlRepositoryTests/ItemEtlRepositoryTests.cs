using EconDataLens.Core.Entities.Cpi;
using EconDataLens.Core.Interfaces;
using EconDataLens.Core.Configuration;
using EconDataLens.Services;
using EconDataLens.Data;
using EconDataLens.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EconDataLens.Tests.EtlRepositoryTests;

public class ItemEtlRepositoryTests
{
    private EconDataLensDbContext _dbContext = null!;
    private ICpiDataFileParser _parser = null!;
    private ICpiIngestionRepository _repository = null!;
    private ICpiIngestionService _service = null!;
    private string _connectionString = string.Empty;

    [SetUp]
    public async Task SetUp()
    {
        _connectionString = PostgresFixture.ConnectionString;
        var opts = new DbContextOptionsBuilder<EconDataLensDbContext>()
            .UseNpgsql(_connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        await DbReset.RecreateDatabaseAsync(_connectionString);

        _dbContext = new EconDataLensDbContext(opts);

        var blsOptions = Options.Create(new BlsOptions
        {
            Cpi = new CpiOptions
            {
                FootnoteFile = "cu.item.sample"
            }
        });

        var downloadOptions = Options.Create(new DownloadOptions
        {
            DownloadDirectory = "TestData/EtlData",
            DeleteDownloadedFiles = false
        });

        _parser = new CpiDataFileParser(blsOptions, downloadOptions);
        _repository = new CpiIngestionRepository(_dbContext);
    }

    [Test]
    public async Task UpsertCpiItemsAsync_NewItems_InsertsSuccessfully()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.item.sample");

        await _repository.UpsertCpiItemAsync(_parser.ParseCpiItemsAsync(path));

        var count = await _dbContext.CpiItem.CountAsync();

        Assert.That(count, Is.EqualTo(400));

        var sample = await _dbContext.CpiItem.FirstOrDefaultAsync(i => i.ItemCode == "AA0");

        Assert.That(sample, Is.Not.Null);
        Assert.That(sample.ItemName, Is.EqualTo("All items - old base"));

        sample = await _dbContext.CpiItem.FirstOrDefaultAsync(i => i.ItemCode == "SSHJ031");

        Assert.That(sample, Is.Not.Null);
        Assert.That(sample.ItemName, Is.EqualTo("Infants' furniture"));

    }

    [Test]
    public async Task UpsertCpiItemsAsync_ExistingItems_UpdatesSuccessfully()
    {
        await DbReset.RecreateDatabaseAsync(_connectionString);
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.item.sample");

        await _repository.UpsertCpiItemAsync(_parser.ParseCpiItemsAsync(path));

        var count = await _dbContext.CpiItem.CountAsync();

        Assert.That(count, Is.EqualTo(400));

        var sample = await _dbContext.CpiItem.FirstOrDefaultAsync(i => i.ItemCode == "AA0");

        Assert.That(sample, Is.Not.Null);
        Assert.That(sample.ItemName, Is.EqualTo("All items - old base"));

        sample = await _dbContext.CpiItem.FirstOrDefaultAsync(i => i.ItemCode == "SSHJ031");

        Assert.That(sample, Is.Not.Null);
        Assert.That(sample.ItemName, Is.EqualTo("Infants' furniture"));

        // Ingest modified data

        path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.item.modified.sample");

        await _repository.UpsertCpiItemAsync(_parser.ParseCpiItemsAsync(path));

        count = await _dbContext.CpiItem.CountAsync();

        Assert.That(count, Is.EqualTo(400));

        sample = await _dbContext.CpiItem.FirstOrDefaultAsync(i => i.ItemCode == "SSHJ031");

        Assert.That(sample, Is.Not.Null);
        Assert.That(sample.ItemName, Is.EqualTo("Infants' furniture UPDATED"));
    }

    [TearDown]
    public async Task TearDown()
    {
        await _dbContext.DisposeAsync();
    }

}
