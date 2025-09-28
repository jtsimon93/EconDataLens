using EconDataLens.Core.Entities.Cpi;

namespace EconDataLens.Core.Interfaces;

public interface ICpiDataFileParser
{
    IAsyncEnumerable<CpiArea> ParseCpiAreasAsync(string? filePath, CancellationToken ct = default);
    IAsyncEnumerable<CpiData> ParseCpiDataAsync(string? filePath, CancellationToken ct = default);
    IAsyncEnumerable<CpiFootnote> ParseCpiFootnoteAsync(string? filePath, CancellationToken ct = default);
    IAsyncEnumerable<CpiItem> ParseCpiItemsAsync(string? filePath, CancellationToken ct = default);
    IAsyncEnumerable<CpiPeriod> ParseCpiPeriodsAsync(string? filePath, CancellationToken ct = default);
    IAsyncEnumerable<CpiSeries> ParseCpiSeriesAsync(string? filePath, CancellationToken ct = default);
}