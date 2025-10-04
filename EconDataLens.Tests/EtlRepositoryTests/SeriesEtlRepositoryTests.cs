using EconDataLens.Core.Interfaces;
using EconDataLens.Core.Configuration;
using EconDataLens.Services;
using EconDataLens.Data;
using EconDataLens.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EconDataLens.Tests.EtlRepositoryTests;

public class SeriesEtlRepositoryTests
{
    private EconDataLensDbContext _dbContext = null!;
    private ICpiDataFileParser _parser = null!;
    private ICpiIngestionRepository _repository = null!;
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
        

        var blsOptions = Options.Create(new BlsOptions
        {
            Cpi = new CpiOptions
            {
                SeriesFile = "cu.series.sample"
            }
        });

        var downloadOptions = Options.Create(new DownloadOptions
        {
            DownloadDirectory = "TestData/EtlData",
            DeleteDownloadedFiles = false
        });

        _dbContext = new EconDataLensDbContext(opts);
        _parser = new CpiDataFileParser(blsOptions, downloadOptions);
        _repository = new CpiIngestionRepository(_dbContext);
    }

    [Test]
    public async Task UpsertCpiSeriesAsync_NewSeries_InsertsSuccessfully()
    {
        var areaPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.area.sample");
        var itemPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.item.sample");
        var periodPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData",
            "cu.period.sample");
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.series.sample");

        // Load dependencies first
        await _repository.UpsertCpiAreaAsync(_parser.ParseCpiAreasAsync(areaPath));
        await _repository.UpsertCpiItemAsync(_parser.ParseCpiItemsAsync(itemPath));
        await _repository.UpsertCpiPeriodAsync(_parser.ParseCpiPeriodsAsync(periodPath));
        
        // Now load series
        await _repository.UpsertCpiSeriesAsync(_parser.ParseCpiSeriesAsync(path));

        var count = await _dbContext.CpiSeries.CountAsync();

        Assert.That(count, Is.EqualTo(8103));

        var sample = await _dbContext.CpiSeries.FirstOrDefaultAsync(s => s.SeriesId == "CUSR0000SA0");

        Assert.That(sample, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(sample!.SeriesTitle, Is.EqualTo("All items in U.S. city average, all urban consumers, seasonally adjusted"));
            Assert.That(sample!.AreaCode, Is.EqualTo("0000"));
            Assert.That(sample!.ItemCode, Is.EqualTo("SA0"));
            Assert.That(sample!.Seasonal, Is.EqualTo("S"));
            Assert.That(sample!.PeriodicityCode, Is.EqualTo("R"));
            Assert.That(sample!.BaseCode, Is.EqualTo("S"));
            Assert.That(sample!.BasePeriod, Is.EqualTo("1982-84=100"));
            Assert.That(sample!.FootnoteCodes, Is.Null);
            Assert.That(sample!.BeginYear, Is.EqualTo(1947));
            Assert.That(sample!.BeginPeriod, Is.EqualTo("M01"));
            Assert.That(sample!.EndYear, Is.EqualTo(2025));
            Assert.That(sample!.EndPeriod, Is.EqualTo("M08"));
        });
    }

    [Test]
    public async Task UpsertCpiSeriesAsync_ExistingSeries_UpdatesSuccessfully()
    {
        var areaPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.area.sample");
        var itemPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.item.sample");
        var periodPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData",
            "cu.period.sample");
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.series.sample");
        var modifiedPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData",
            "cu.series.modified.sample");

        // Load dependencies first
        await _repository.UpsertCpiAreaAsync(_parser.ParseCpiAreasAsync(areaPath));
        await _repository.UpsertCpiItemAsync(_parser.ParseCpiItemsAsync(itemPath));
        await _repository.UpsertCpiPeriodAsync(_parser.ParseCpiPeriodsAsync(periodPath));
        
        // Now load series
        await _repository.UpsertCpiSeriesAsync(_parser.ParseCpiSeriesAsync(path));

        var count = await _dbContext.CpiSeries.CountAsync();

        Assert.That(count, Is.EqualTo(8103));
        
        // Load modified series
        await _repository.UpsertCpiSeriesAsync(_parser.ParseCpiSeriesAsync(modifiedPath));
        
        Assert.That(count, Is.EqualTo(8103));

        var sample = await _dbContext.CpiSeries.FirstOrDefaultAsync(s => s.SeriesId == "CUSR0000SA0");

        Assert.That(sample, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(sample!.SeriesTitle, Is.EqualTo("modified"));
            Assert.That(sample!.AreaCode, Is.EqualTo("0230"));
            Assert.That(sample!.ItemCode, Is.EqualTo("SA0"));
            Assert.That(sample!.Seasonal, Is.EqualTo("S"));
            Assert.That(sample!.PeriodicityCode, Is.EqualTo("R"));
            Assert.That(sample!.BaseCode, Is.EqualTo("S"));
            Assert.That(sample!.BasePeriod, Is.EqualTo("1982-84=100"));
            Assert.That(sample!.FootnoteCodes, Is.Null);
            Assert.That(sample!.BeginYear, Is.EqualTo(1999));
            Assert.That(sample!.BeginPeriod, Is.EqualTo("M01"));
            Assert.That(sample!.EndYear, Is.EqualTo(2025));
            Assert.That(sample!.EndPeriod, Is.EqualTo("M08"));
        });
    }

    [TearDown]
    public async Task TearDown()
    {
        await _dbContext.DisposeAsync();
    }
}
