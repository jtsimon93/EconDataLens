using EconDataLens.Core.Interfaces;
using EconDataLens.Core.Entities.Cpi;
using EconDataLens.Core.Configuration;
using EconDataLens.Services;
using Microsoft.Extensions.Options;

namespace EconDataLens.Tests.EtlParserTests;

public class CpiItemParserTests
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
    public async Task ParseCpiItemAsync_HeaderOnly_YieldsNoResults()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ParserData", "cu.item.empty");
        var rows = new List<CpiItem>();

        await foreach (var row in _parser.ParseCpiItemsAsync(path))
            rows.Add(row);

        Assert.That(rows, Is.Empty);
    }

    [Test]
    public async Task ParseCpiItemAsync_FileWithRecords_YieldsResults()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ParserData", "cu.item.sample");
        var rows = new List<CpiItem>();

        await foreach (var row in _parser.ParseCpiItemsAsync(path))
            rows.Add(row);

        Assert.That(rows, Has.Count.EqualTo(3));

        Assert.Multiple(() =>
        {
            Assert.That(rows, Is.Not.Null);
            Assert.That(rows[0].ItemCode, Is.EqualTo("AA0"));
            Assert.That(rows[0].ItemName, Is.EqualTo("All items - old base"));
            Assert.That(rows[1].ItemCode, Is.EqualTo("AA0R"));
            Assert.That(rows[1].ItemName, Is.EqualTo("Purchasing power of the consumer dollar - old base"));
            Assert.That(rows[2].ItemCode, Is.EqualTo("SA0"));
            Assert.That(rows[2].ItemName, Is.EqualTo("All items"));
        });
    }

}
