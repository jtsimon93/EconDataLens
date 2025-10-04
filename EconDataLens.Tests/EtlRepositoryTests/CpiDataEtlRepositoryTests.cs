using EconDataLens.Core.Interfaces;
using EconDataLens.Core.Configuration;
using EconDataLens.Services;
using EconDataLens.Data;
using EconDataLens.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EconDataLens.Tests.EtlRepositoryTests;

public class CpiDataEtlRepositoryTests
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
    public async Task UpsertCpiDataAsync_NewData_InsertsSuccessfully()
    {
        await UpsertDependencies();

        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.data.0.Current.sample");

        await _repository.UpsertCpiDataAsync(_parser.ParseCpiDataAsync(path));

        var count = await _dbContext.CpiData.CountAsync();

        Assert.That(count, Is.EqualTo(4));

        var sample = await _dbContext.CpiData.FirstOrDefaultAsync(d => d.SeriesId == "CUSR0000SA0" && d.Year == 1997 && d.Period == "M01");

        Assert.Multiple(() =>
        {
            Assert.That(sample, Is.Not.Null);
            Assert.That(sample!.Value == 159.40m);
        });
    }

    [Test]
    public async Task UpsertCpiDataAsync_ExistingData_UpdatesSuccessfully()
    {
        await UpsertDependencies();
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.data.0.Current.sample");
        var modifiedPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.data.0.Current.modified.sample");

        await _repository.UpsertCpiDataAsync(_parser.ParseCpiDataAsync(path));

        var count = await _dbContext.CpiData.CountAsync();

        Assert.That(count, Is.EqualTo(4));

        var sample = await _dbContext.CpiData.FirstOrDefaultAsync(d => d.SeriesId == "CUSR0000SA0" && d.Year == 1997 && d.Period == "M01");

        Assert.Multiple(() =>
        {
            Assert.That(sample, Is.Not.Null);
            Assert.That(sample!.Value == 159.40m);
        });

        // Run the upsert again with modified data
        await _repository.UpsertCpiDataAsync(_parser.ParseCpiDataAsync(modifiedPath));

        count = await _dbContext.CpiData.CountAsync();

        Assert.That(count, Is.EqualTo(4));

        sample = await _dbContext.CpiData.FirstOrDefaultAsync(d => d.SeriesId == "CUSR0000SA0" && d.Year == 1997 && d.Period == "M01");

        Assert.Multiple(() =>
        {
            Assert.That(sample, Is.Not.Null);
            Assert.That(sample!.Value == 999.99m);
        });

        // Verify another record was unchanged
        sample = await _dbContext.CpiData.FirstOrDefaultAsync(d => d.SeriesId == "CUSR0000SA0" && d.Year == 1997 && d.Period == "M02");

        Assert.Multiple(() =>
        {
            Assert.That(sample, Is.Not.Null);
            Assert.That(sample!.Value, Is.EqualTo(159.70m));
        });

    }

    private async Task UpsertDependencies()
    {
        var areaPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.area.sample");
        var itemPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.item.sample");
        var seriesPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.series.sample");
        var footnotePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.footnote.sample");
        var periodPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.period.sample");

        await _repository.UpsertCpiAreaAsync(_parser.ParseCpiAreasAsync(areaPath));
        await _repository.UpsertCpiPeriodAsync(_parser.ParseCpiPeriodsAsync(periodPath));
        await _repository.UpsertCpiItemAsync(_parser.ParseCpiItemsAsync(itemPath));
        await _repository.UpsertCpiFootnotesAsync(_parser.ParseCpiFootnoteAsync(footnotePath));
        await _repository.UpsertCpiSeriesAsync(_parser.ParseCpiSeriesAsync(seriesPath));
    }


    [TearDown]
    public async Task TearDown()
    {
        await _dbContext.DisposeAsync();
    }
}
