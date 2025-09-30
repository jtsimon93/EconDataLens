using EconDataLens.Core.Configuration;
using EconDataLens.Core.Interfaces;
using EconDataLens.Core.Entities.Cpi;
using EconDataLens.Services;
using Microsoft.Extensions.Options;

namespace EconDataLens.Tests.EtlParserTests;

public class CpiPeriodParserTests
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
    public async Task ParseCpiPeriodAsync_HeaderOnly_YieldsNoResults()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "cu.period.empty");
        var rows = new List<CpiPeriod>();
        
        await foreach (var row in _parser.ParseCpiPeriodsAsync(path))
            rows.Add(row);
        
        Assert.That(rows, Is.Empty);
    }

    [Test]
    public async Task ParseCpiPeriodAsync_FileWithRecords_YieldsResults()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "cu.period.sample");
        var rows = new List<CpiPeriod>();

        await foreach (var row in _parser.ParseCpiPeriodsAsync(path))
            rows.Add(row);

        Assert.That(rows, Has.Count.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(rows[0].Period, Is.EqualTo("M01"));
            Assert.That(rows[0].PeriodAbbreviation, Is.EqualTo("JAN"));
            Assert.That(rows[0].PeriodName, Is.EqualTo("January"));

            Assert.That(rows[1].Period, Is.EqualTo("S03"));
            Assert.That(rows[1].PeriodAbbreviation, Is.EqualTo("AN AV"));
            Assert.That(rows[1].PeriodName, Is.EqualTo("Annual Average"));
        });
    }

}