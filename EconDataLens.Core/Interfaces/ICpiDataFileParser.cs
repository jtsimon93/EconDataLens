namespace EconDataLens.Core.Interfaces;
using EconDataLens.Core.Entities.Cpi;

public interface ICpiDataFileParser
{
    IAsyncEnumerable<CpiArea> ParseCpiAreasAsync(string? filePath, CancellationToken ct = default);
    IAsyncEnumerable<CpiData> ParseCpiDataAsync(string? filePath);
    IAsyncEnumerable<CpiFootnote> ParseCpiFootnoteAsync(string? filePath);
    IAsyncEnumerable<CpiItem> ParseCpiItemsAsync(string? filePath);
    IAsyncEnumerable<CpiPeriod> ParseCpiPeriodsAsync(string? filePath);
    IAsyncEnumerable<CpiSeries> ParseCpiSeriesAsync(string? filePath);
}