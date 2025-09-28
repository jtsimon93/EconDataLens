namespace EconDataLens.Core.Interfaces;

public interface ICpiIngestionService
{
    Task ImportAreasAsync(CancellationToken ct = default);
}