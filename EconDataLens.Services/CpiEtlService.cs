using EconDataLens.Core.Interfaces;

namespace EconDataLens.Services;

public class CpiEtlService : ICpiEtlService
{
    private readonly ICpiIngestionService _cpiIngestionService;

    public CpiEtlService(ICpiIngestionService cpiIngestionService)
    {
        _cpiIngestionService = cpiIngestionService;
    }

    public async Task RunEtlAsync()
    {
        // Phase 1  
        await _cpiIngestionService.ImportAreasAsync();
        await _cpiIngestionService.ImportFootnoteAsync();
        await _cpiIngestionService.ImportItemAsync();
        await _cpiIngestionService.ImportPeriodAsync();

        // Phase 2 (Dependent on Phase 1):
        await _cpiIngestionService.ImportSeriesAsync();

        // Phase 3 (Dependent on Phase 2):
        await _cpiIngestionService.ImportCpiDataAsync();
    }
}