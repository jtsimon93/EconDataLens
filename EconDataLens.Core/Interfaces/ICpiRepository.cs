using EconDataLens.Core.Entities.Cpi;

namespace EconDataLens.Core.Interfaces;

public interface ICpiRepository
{
    Task UpsertCpiAreaAsync(IEnumerable<CpiArea> areas);
    Task UpsertCpiDataAsync(IEnumerable<CpiData> cpiData);
    Task UpsertCpiFootnotesAsync(IEnumerable<CpiFootnote> footnotes);
    Task UpsertCpiItemAsync(IEnumerable<CpiItem> cpiItems);
    Task UpsertCpiPeriodAsync(IEnumerable<CpiPeriod> periods);
    Task UpsertCpiSeriesAsync(IEnumerable<CpiSeries> series);
}