using System.Text;
using EconDataLens.Core.Configuration;
using EconDataLens.Core.Entities.Cpi;
using EconDataLens.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace EconDataLens.Services;

public class CpiDataFileParser : ICpiDataFileParser
{
    private readonly BlsOptions _blsOptions;
    private readonly DownloadOptions _downloadOptions;

    public CpiDataFileParser(IOptions<BlsOptions> blsSettings, IOptions<DownloadOptions> downloadOptions)
    {
        _blsOptions = blsSettings.Value;
        _downloadOptions = downloadOptions.Value;
    }

    public async IAsyncEnumerable<CpiArea> ParseCpiAreasAsync(string? filePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            filePath = Path.Combine(_downloadOptions.DownloadDirectory, _blsOptions.Cpi.AreaFile);

        if (!File.Exists(filePath)) throw new FileNotFoundException($"CPI Area file not found at path: {filePath}");

        await using var fs = new FileStream(
            filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
            1 << 16, true);

        using var sr = new StreamReader(
            fs,
            new UTF8Encoding(false),
            true,
            1 << 16);

        var isHeader = true;

        while (await sr.ReadLineAsync(ct) is { } line)
        {
            ct.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (isHeader)
            {
                isHeader = false;
                continue;
            }

            var parts = line.Split('\t');
            if (parts.Length < 2)
                throw new FormatException($"Unexpected number of columns in CPI Area file line: {line}");

            yield return new CpiArea
            {
                AreaCode = parts[0].Trim(),
                AreaName = parts[1].Trim()
            };
        }
    }

    public async IAsyncEnumerable<CpiData> ParseCpiDataAsync(string? filePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            filePath = Path.Combine(_downloadOptions.DownloadDirectory, _blsOptions.Cpi.DataFile);
        if (!File.Exists(filePath)) throw new FileNotFoundException($"CPI Data file not found at path: {filePath}");

        await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
            1 << 16, true);
        using var sr = new StreamReader(fs, new UTF8Encoding(false), true, 1 << 16);

        var isHeader = true;

        while (await sr.ReadLineAsync(ct) is { } line)
        {
            ct.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (isHeader)
            {
                isHeader = false;
                continue;
            }

            var parts = line.Split('\t');

            if (parts.Length < 4)
                throw new FormatException($"Unexpected number of columns in CPI Data file line: {line}");

            yield return new CpiData
            {
                SeriesId = parts[0].Trim(),
                Year = int.Parse(parts[1].Trim()),
                Period = parts[2].Trim(),
                Value = decimal.Parse(parts[3].Trim()),
                FootnoteCodes = parts.Length > 4 ? parts[4].Trim() : null
            };
        }
    }

    public async IAsyncEnumerable<CpiFootnote> ParseCpiFootnoteAsync(string? filePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            filePath = Path.Combine(_downloadOptions.DownloadDirectory, _blsOptions.Cpi.FootnoteFile);

        if (!File.Exists(filePath)) throw new FileNotFoundException($"CPI Footnote file not found at path: {filePath}");

        await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
            1 << 16, true);

        using var sr = new StreamReader(fs, new UTF8Encoding(false),
            true, 1 << 16);

        var isHeader = true;

        while (await sr.ReadLineAsync(ct) is { } line)
        {
            ct.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (isHeader)
            {
                isHeader = false;
                continue;
            }

            var parts = line.Split('\t');
            if (parts.Length < 2)
                throw new FormatException($"Unexpected number of columns in CPI Footnote file line: {line}");

            yield return new CpiFootnote
            {
                FootnoteCode = parts[0].Trim(),
                FootnoteText = parts[1].Trim()
            };
        }
    }

    public async IAsyncEnumerable<CpiItem> ParseCpiItemsAsync(string? filePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            filePath = Path.Combine(_downloadOptions.DownloadDirectory, _blsOptions.Cpi.ItemFile);

        if (!File.Exists(filePath)) throw new FileNotFoundException($"CPI Item file not found at path: {filePath}");

        await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
            1 << 16, true);
        using var sr = new StreamReader(fs, new UTF8Encoding(false),
            true, 1 << 16);

        var isHeader = true;

        while (await sr.ReadLineAsync(ct) is { } line)
        {
            ct.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (isHeader)
            {
                isHeader = false;
                continue;
            }

            var parts = line.Split('\t');

            if (parts.Length < 2)
                throw new FormatException($"Unexpected number of columns in CPI Item file line: {line}");

            yield return new CpiItem
            {
                ItemCode = parts[0].Trim(),
                ItemName = parts[1].Trim()
            };
        }
    }

    public async IAsyncEnumerable<CpiPeriod> ParseCpiPeriodsAsync(string? filePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            filePath = Path.Combine(_downloadOptions.DownloadDirectory, _blsOptions.Cpi.PeriodFile);

        if (!File.Exists(filePath)) throw new FileNotFoundException($"CPI Period file not found at path: {filePath}");

        await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
            1 << 16, true);
        using var sr = new StreamReader(fs, new UTF8Encoding(false), true,
            1 << 16);

        var isHeader = true;

        while (await sr.ReadLineAsync(ct) is { } line)
        {
            ct.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (isHeader)
            {
                isHeader = false;
                continue;
            }

            var parts = line.Split('\t');

            if (parts.Length < 3)
                throw new FormatException($"Unexpected number of columns in CPI Period file line: {line}");

            yield return new CpiPeriod
            {
                Period = parts[0].Trim(),
                PeriodAbbreviation = parts[1].Trim(),
                PeriodName = parts[2].Trim()
            };
        }
    }

    public async IAsyncEnumerable<CpiSeries> ParseCpiSeriesAsync(string? filePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            filePath = Path.Combine(_downloadOptions.DownloadDirectory, _blsOptions.Cpi.SeriesFile);

        if (!File.Exists(filePath)) throw new FileNotFoundException($"CPI Series file not found at path: {filePath}");

        await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
            1 << 16, true);

        using var sr = new StreamReader(fs, new UTF8Encoding(false), true,
            1 << 16);

        var isHeader = true;

        while (await sr.ReadLineAsync(ct) is { } line)
        {
            ct.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (isHeader)
            {
                isHeader = false;
                continue;
            }

            var parts = line.Split('\t');

            if (parts.Length < 13)
                throw new FormatException($"Unexpected number of columns in CPI Series file line: {line}");

            yield return new CpiSeries
            {
                SeriesId = parts[0].Trim(),
                AreaCode = parts[1].Trim(),
                ItemCode = parts[2].Trim(),
                Seasonal = parts[3].Trim(),
                PeriodicityCode = parts[4].Trim(),
                BaseCode = parts[5].Trim(),
                BasePeriod = parts[6].Trim(),
                SeriesTitle = parts[7].Trim(),
                FootnoteCodes = parts[8].Trim(),
                BeginYear = int.Parse(parts[9].Trim()),
                BeginPeriod = parts[10].Trim(),
                EndYear = int.Parse(parts[11].Trim()),
                EndPeriod = parts[12].Trim()
            };
        }
    }
}