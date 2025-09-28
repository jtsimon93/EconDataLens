using EconDataLens.Core.Configuration;
using EconDataLens.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace EconDataLens.Services;

public class CpiIngestionService : ICpiIngestionService
{
    private readonly BlsOptions _blsOptions;
    private readonly ICpiRepository _cpiRepository;
    private readonly DownloadOptions _downloadOptions;
    private readonly IFileDownloadService _fileDownloadService;
    private readonly ICpiDataFileParser _parser;

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
    ///     Downloads the CPI Area file from BLS, parses it, and upserts the data into the database.
    ///     Utilizes streaming to handle large files efficiently.
    /// </summary>
    public async Task ImportAreasAsync(CancellationToken ct = default)
    {
        var url = _blsOptions.Cpi.BaseUrl + _blsOptions.Cpi.AreaFile;
        var path = Path.Combine(_downloadOptions.DownloadDirectory, _blsOptions.Cpi.AreaFile);
        await _fileDownloadService.DownloadFileAsync(url, path, ct);

        await _cpiRepository.UpsertCpiAreaAsync(_parser.ParseCpiAreasAsync(path, ct), ct);
    }

    /// <summary>
    ///     Downloads the CPI Footnote file from BLS, parses it, and upserts the data into the database.
    ///     Utilizes streaming to handle large files efficiently.
    /// </summary>
    public async Task ImportFootnoteAsync(CancellationToken ct = default)
    {
        var url = _blsOptions.Cpi.BaseUrl + _blsOptions.Cpi.FootnoteFile;
        var path = Path.Combine(_downloadOptions.DownloadDirectory, _blsOptions.Cpi.FootnoteFile);
        await _fileDownloadService.DownloadFileAsync(url, path, ct);

        await _cpiRepository.UpsertCpiFootnotesAsync(_parser.ParseCpiFootnoteAsync(path, ct), ct);
    }

    /// <summary>
    ///     Downlods the CPI Item file from BLS, parses it, and upserts the data into the database.
    ///     Utilizes streaming to handle large files efficiently.
    /// </summary>
    public async Task ImportItemAsync(CancellationToken ct = default)
    {
        var url = _blsOptions.Cpi.BaseUrl + _blsOptions.Cpi.ItemFile;
        var path = Path.Combine(_downloadOptions.DownloadDirectory, _blsOptions.Cpi.ItemFile);
        await _fileDownloadService.DownloadFileAsync(url, path, ct);
        await _cpiRepository.UpsertCpiItemAsync(_parser.ParseCpiItemsAsync(path, ct), ct);
    }

    /// <summary>
    ///     Downloads the CPI Period file from BLS, parses it, and upserts the data into the database.
    ///     Utilizes streaming to handle large files efficiently.
    /// </summary>
    public async Task ImportPeriodAsync(CancellationToken ct = default)
    {
        var url = _blsOptions.Cpi.BaseUrl + _blsOptions.Cpi.PeriodFile;
        var path = Path.Combine(_downloadOptions.DownloadDirectory, _blsOptions.Cpi.PeriodFile);
        await _fileDownloadService.DownloadFileAsync(url, path, ct);
        await _cpiRepository.UpsertCpiPeriodAsync(_parser.ParseCpiPeriodsAsync(path, ct), ct);
    }

    /// <summary>
    ///     Downloads the CPI Series file from BLS, parses it, and upserts the data
    ///     Utilizes streaming to handle large files efficiently
    /// </summary>
    public async Task ImportSeriesAsync(CancellationToken ct = default)
    {
        var url = _blsOptions.Cpi.BaseUrl + _blsOptions.Cpi.SeriesFile;
        var path = Path.Combine(_downloadOptions.DownloadDirectory, _blsOptions.Cpi.SeriesFile);
        await _fileDownloadService.DownloadFileAsync(url, path, ct);
        await _cpiRepository.UpsertCpiSeriesAsync(_parser.ParseCpiSeriesAsync(path, ct), ct);
    }

    /// <summary>
    ///     Downloads the CPI data file from BLS, parses it, and upserts the data into the database.
    ///     Utilizes streaming to handle large files efficiently.
    /// </summary>
    public async Task ImportCpiDataAsync(CancellationToken ct = default)
    {
        var url = _blsOptions.Cpi.BaseUrl + _blsOptions.Cpi.DataFile;
        var path = Path.Combine(_downloadOptions.DownloadDirectory, _blsOptions.Cpi.DataFile);
        await _fileDownloadService.DownloadFileAsync(url, path, ct);
        await _cpiRepository.UpsertCpiDataAsync(_parser.ParseCpiDataAsync(path, ct), ct);
    }
}