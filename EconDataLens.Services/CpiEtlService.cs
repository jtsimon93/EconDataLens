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

        // Phase 2 (Dependent on Phase 1):
        // This will be CPI Series

        // Phase 3 (Dependent on Phase 2):
        // This will be CPI Data
    }
}