namespace EconDataLens.Services;
using EconDataLens.Core.Interfaces;
using EconDataLens.Core.Configuration;
using EconDataLens.Core.Entities.Cpi;
using Microsoft.Extensions.Options;

public class CpiIngestionService : ICpiIngestionService
{
    private readonly IFileDownloadService _fileDownloadService;
    private readonly ICpiDataFileParser _parser;
    private readonly BlsOptions _blsOptions;
    private readonly DownloadOptions _downloadOptions;
    private readonly ICpiRepository _cpiRepository;

    public CpiIngestionService(
        IFileDownloadService fileDownloadService,
        ICpiDataFileParser parser,
        IOptions<BlsOptions> blsOptions,
        IOptions<DownloadOptions> downloadOptions,
        ICpiRepository cpiRepository
    )
    {
        _fileDownloadService = fileDownloadService;
        _parser = parser;
        _blsOptions = blsOptions.Value;
        _downloadOptions = downloadOptions.Value;
        _cpiRepository = cpiRepository;
    }

    /// <summary>
    /// Downloads the CPI Area file from BLS, parses it, and upserts the data into the database.
    /// Utilizes streaming to handle large files efficiently.
    /// </summary>  
    public async Task ImportAreasAsync(CancellationToken ct = default)
    {
        var url  = _blsOptions.Cpi.BaseUrl + _blsOptions.Cpi.AreaFile;
        var path = Path.Combine(_downloadOptions.DownloadDirectory, _blsOptions.Cpi.AreaFile);
        await _fileDownloadService.DownloadFileAsync(url, path, ct);
        
        await _cpiRepository.UpsertCpiAreaAsync(_parser.ParseCpiAreasAsync(path, ct), ct);
    }

}