using EconDataLens.Core.Interfaces;
using EconDataLens.Core.Entities.Cpi;
using EconDataLens.Core.Configuration;
using EconDataLens.Services;
using Microsoft.Extensions.Options;

namespace EconDataLens.Tests.EtlParserTests;

public class CpiSeriesParserTests
{
    private ICpiDataFileParser _parser;
    
    [SetUp]
    public void SetUp()
    {
        // This isn't actually used in the test, but the parser requires it.
        var blsOptions = Options.Create(new BlsOptions
        {
            Cpi = new CpiOptions
            {
                FootnoteFile = "cu.footnote"
            }
        });

        // This isn't actually used in the test, but the parser requires it.
        var downloadOptions = Options.Create(new DownloadOptions
        {
            DownloadDirectory = "downloads",
            DeleteDownloadedFiles = false
        });

        _parser = new CpiDataFileParser(blsOptions, downloadOptions);
    }

    [Test]
    public async Task ParseCpiSeriesAsync_HeaderOnly_YieldsNoResults()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "cu.series.empty");
        var rows = new List<CpiSeries>();

        await foreach (var row in _parser.ParseCpiSeriesAsync(path))
            rows.Add(row);

        Assert.That(rows, Is.Empty);
    }
    
    [Test]
    public async Task ParseCpiSeriesAsync_FileWithRecords_YieldsResults()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "cu.series.sample");
        var rows = new List<CpiSeries>();

        await foreach (var row in _parser.ParseCpiSeriesAsync(path))
            rows.Add(row);

        Assert.That(rows, Has.Count.EqualTo(2));
        
        Assert.Multiple(() =>
        {
            Assert.That(rows, Is.Not.Null);
            Assert.That(rows[0].SeriesId, Is.EqualTo("CUSR0000SA0"));
            Assert.That(rows[0].AreaCode, Is.EqualTo("0000"));
            Assert.That(rows[0].ItemCode, Is.EqualTo("SA0"));
            Assert.That(rows[0].Seasonal, Is.EqualTo("S"));
            Assert.That(rows[0].PeriodicityCode, Is.EqualTo("R"));
            Assert.That(rows[0].BaseCode, Is.EqualTo("S"));
            Assert.That(rows[0].BasePeriod, Is.EqualTo("1982-84=100"));
            Assert.That(rows[0].SeriesTitle, Is.EqualTo("All items in U.S. city average, all urban consumers, seasonally adjusted"));
            Assert.That(rows[0].FootnoteCodes, Is.Null);
            Assert.That(rows[0].BeginYear, Is.EqualTo(1947));
            Assert.That(rows[0].BeginPeriod, Is.EqualTo("M01"));
            Assert.That(rows[0].EndYear, Is.EqualTo(2025));
            Assert.That(rows[0].EndPeriod, Is.EqualTo("M08"));
            
            Assert.That(rows[1].SeriesId, Is.EqualTo("CUSR0000SA0E"));
            Assert.That(rows[1].AreaCode, Is.EqualTo("0000"));
            Assert.That(rows[1].ItemCode, Is.EqualTo("SA0E"));
            Assert.That(rows[1].Seasonal, Is.EqualTo("S"));
            Assert.That(rows[1].PeriodicityCode, Is.EqualTo("R"));
            Assert.That(rows[1].BaseCode, Is.EqualTo("S"));
            Assert.That(rows[1].BasePeriod, Is.EqualTo("1982-84=100"));
            Assert.That(rows[1].SeriesTitle, Is.EqualTo("Energy in U.S. city average, all urban consumers, seasonally adjusted"));
            Assert.That(rows[1].FootnoteCodes, Is.Null);
            Assert.That(rows[1].BeginYear, Is.EqualTo(1957));
            Assert.That(rows[1].BeginPeriod, Is.EqualTo("M01"));
            Assert.That(rows[1].EndYear, Is.EqualTo(2025));
            Assert.That(rows[1].EndPeriod, Is.EqualTo("M08"));
        });
    }
}