using EconDataLens.Core.Interfaces;
using EconDataLens.Core.Entities.Cpi;
using EconDataLens.Core.Configuration;
using EconDataLens.Services;
using Microsoft.Extensions.Options;

namespace EconDataLens.Tests.ServiceTests;

public class CpiAreaParserTests
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
   public async Task ParseCpiAreaAsync_HeaderOnly_YieldsNoResults()
   {
      var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "cu.area.empty");

      var rows = new List<CpiArea>();
      
      await foreach (var row in _parser.ParseCpiAreasAsync(path))
         rows.Add(row);
      
      Assert.That(rows, Is.Empty);
   }
   
   [Test]
   public async Task ParseCpiAreaAsync_FileWithRecords_YieldsResults()
   {
      var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "cu.area.sample");

      var rows = new List<CpiArea>();
      
      await foreach (var row in _parser.ParseCpiAreasAsync(path))
         rows.Add(row);
      
      Assert.That(rows, Has.Count.EqualTo(4));
      
      Assert.Multiple(() =>
      {
         Assert.That(rows[0].AreaCode, Is.EqualTo("0000"));
         Assert.That(rows[0].AreaName, Is.EqualTo("U.S. city average"));
         
         Assert.That(rows[1].AreaCode, Is.EqualTo("0100"));
         Assert.That(rows[1].AreaName, Is.EqualTo("Northeast"));
         
         Assert.That(rows[2].AreaCode, Is.EqualTo("0110"));
         Assert.That(rows[2].AreaName, Is.EqualTo("New England"));

         Assert.That(rows[3].AreaCode, Is.EqualTo("0120"));
         Assert.That(rows[3].AreaName, Is.EqualTo("Middle Atlantic"));

         // Assert row 4 throws index out of range if accessed
         Assert.Throws<ArgumentOutOfRangeException>(() =>
         {
            var _ = rows[4].AreaCode;
         });
      });
   }
}