using EconDataLens.Core.Interfaces;
using EconDataLens.Core.Configuration;
using EconDataLens.Services;
using EconDataLens.Data;
using EconDataLens.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EconDataLens.Tests.EtlRepositoryTests;

public class FootnoteEtlRepositoryTests
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
                SeriesFile = "cu.footnote.sample"
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
    public async Task UpsertCpiFootnoteAsync_NewFootnote_InsertsSuccessfully()
    {
        var footnotePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.footnote.sample");

        await _repository.UpsertCpiFootnotesAsync(_parser.ParseCpiFootnoteAsync(footnotePath));

        var count = await _dbContext.CpiFootnote.CountAsync();

        Assert.That(count, Is.EqualTo(2));

        var sample = await _dbContext.CpiFootnote.FirstOrDefaultAsync(f => f.FootnoteCode == "1");

        Assert.That(sample!.FootnoteText, Is.EqualTo("This is a footnote"));
    }

    [Test]
    public async Task UpsertFootnoteAsync_ExistingFootnote_UpdatesSuccessfully()
    {
        var footnotePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.footnote.sample");
        var footnoteModifiedPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "EtlData", "cu.footnote.modified.sample");

        await _repository.UpsertCpiFootnotesAsync(_parser.ParseCpiFootnoteAsync(footnotePath));

        var count = await _dbContext.CpiFootnote.CountAsync();

        Assert.That(count, Is.EqualTo(2));

        // Re-ingest with modified data
        await _repository.UpsertCpiFootnotesAsync(_parser.ParseCpiFootnoteAsync(footnoteModifiedPath));
        count = await _dbContext.CpiFootnote.CountAsync();

        Assert.That(count, Is.EqualTo(2));

        var sample = await _dbContext.CpiFootnote.FirstOrDefaultAsync(f => f.FootnoteCode == "1");

        Assert.That(sample!.FootnoteText, Is.EqualTo("modified"));
    }

    [TearDown]
    public async Task TearDown()
    {
        await _dbContext.DisposeAsync();
    }
}
