using EconDataLens.Core.Configuration;
using EconDataLens.Core.Entities.Cpi;
using EconDataLens.Core.Interfaces;
using EconDataLens.Services;
using Microsoft.Extensions.Options;


namespace EconDataLens.Tests.EtlParserTests;

public class CpiDataParserTests
{
    private ICpiDataFileParser _parser = null!;

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
    public async Task ParseCpiFootnoteAsync_HeaderOnly_YieldsNoResults()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ParserData", "cu.data.0.Current.empty");
        var rows = new List<CpiData>();

        await foreach (var row in _parser.ParseCpiDataAsync(path))
            rows.Add(row);

        Assert.That(rows, Has.Count.EqualTo(0));
        Assert.That(rows, Is.Empty);
    }

    [Test]
    public async Task ParseCpiDataAsync_FileWithRecords_YieldsResults()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ParserData", "cu.data.0.Current.sample");
        var rows = new List<CpiData>();

        await foreach (var row in _parser.ParseCpiDataAsync(path))
            rows.Add(row);

        Assert.That(rows, Has.Count.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(rows[0].SeriesId, Is.EqualTo("CUSR0000SA0"));
            Assert.That(rows[0].Year, Is.EqualTo(1997));
            Assert.That(rows[0].Period, Is.EqualTo("M01"));
            Assert.That(rows[0].Value, Is.EqualTo(159.40m));
            Assert.That(rows[0].FootnoteCodes, Is.Null);

            Assert.That(rows[1].SeriesId, Is.EqualTo("CUSR0000SA0"));
            Assert.That(rows[1].Year, Is.EqualTo(1997));
            Assert.That(rows[1].Period, Is.EqualTo("M02"));
            Assert.That(rows[1].Value, Is.EqualTo(159.70m));
            Assert.That(rows[1].FootnoteCodes, Is.Null);
        });
    }
}
