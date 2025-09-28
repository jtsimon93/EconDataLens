namespace EconDataLens.Core.Interfaces;

public interface ICpiIngestionService
{
    Task ImportAreasAsync(CancellationToken ct = default);
    Task ImportFootnoteAsync(CancellationToken ct = default);
    Task ImportItemAsync(CancellationToken ct = default);
}