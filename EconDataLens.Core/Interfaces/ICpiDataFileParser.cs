namespace EconDataLens.Core.Interfaces;
using EconDataLens.Core.Entities.Cpi;

public interface ICpiDataFileParser
{
    Task<IEnumerable<CpiArea>> ParseCpiAreasAsync(string filePath);
    Task<IEnumerable<CpiData>> ParseCpiDataAsync(string filePath);
    Task<IEnumerable<CpiFootnote>> ParseCpiFootnoteAsync(string filePath);
    Task<IEnumerable<CpiItem>> ParseCpiItemsAsync(string filePath);
    Task<IEnumerable<CpiPeriod>> ParseCpiPeriodsAsync(string filePath);
    Task<IEnumerable<CpiSeries>> ParseCpiSeriesAsync(string filePath);
}