using EconDataLens.Core.Configuration;
using EconDataLens.Core.Entities.Cpi;
using EconDataLens.Core.Interfaces;
using EconDataLens.Services;
using Microsoft.Extensions.Options;

namespace EconDataLens.Tests.EtlParserTests;

[TestFixture]
public class CpiFootnoteParserTests
{
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

    private ICpiDataFileParser _parser = null!;

    [Test]
    public async Task ParseCpiFootnoteAsync_HeaderOnly_YieldsNoResults()
    {
        // Arrange
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ParserData", "cu.footnote.empty");

        // Act
        var rows = new List<CpiFootnote>();

        await foreach (var row in _parser.ParseCpiFootnoteAsync(path))
            rows.Add(row);

        // Assert
        Assert.That(rows, Is.Empty);
    }

    [Test]
    public async Task ParseCpiFootnoteAsync_FileWithRecords_YieldsResults()
    {
        // Arrange
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ParserData", "cu.footnote.sample");

        // Act
        var rows = new List<CpiFootnote>();

        await foreach (var row in _parser.ParseCpiFootnoteAsync(path))
            rows.Add(row);

        // Assert
        // Sample file has 2 data rows plus header
        Assert.Multiple(() =>
        {
            Assert.That(rows, Has.Count.EqualTo(2));
            Assert.That(rows[0].FootnoteCode, Is.EqualTo("F1"));
            Assert.That(rows[0].FootnoteText, Is.EqualTo("This is a footnote"));
            Assert.That(rows[1].FootnoteCode, Is.EqualTo("F2"));
            Assert.That(rows[1].FootnoteText, Is.EqualTo("This is another footnote"));
        });
    }
}
