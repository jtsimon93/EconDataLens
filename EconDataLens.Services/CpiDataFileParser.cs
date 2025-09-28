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
                AreaCode = parts[0],
                AreaName = parts[1]
            };
        }
    }

    public async IAsyncEnumerable<CpiData> ParseCpiDataAsync(string? filePath, CancellationToken ct = default)
    {
        throw new NotImplementedException();
        yield break;
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
                FootnoteCode = parts[0],
                FootnoteText = parts[1]
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
                ItemCode = parts[0],
                ItemName = parts[1]
            };
        }
    }

    public async IAsyncEnumerable<CpiPeriod> ParseCpiPeriodsAsync(string? filePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            filePath = Path.Combine(_downloadOptions.DownloadDirectory, _blsOptions.Cpi.PeriodFile);
        }
        
        if(!File.Exists(filePath)) throw new FileNotFoundException($"CPI Period file not found at path: {filePath}");

        await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 1 << 16, useAsync: true);
        using var sr = new StreamReader(fs, new UTF8Encoding(false), detectEncodingFromByteOrderMarks: true,
            bufferSize: 1 << 16);

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
            
            if(parts.Length < 3)
                throw new FormatException($"Unexpected number of columns in CPI Period file line: {line}");

            yield return new CpiPeriod
            {
                Period = parts[0],
                PeriodAbbreviation = parts[1],
                PeriodName = parts[2]
            };
        }
    }

    public async IAsyncEnumerable<CpiSeries> ParseCpiSeriesAsync(string? filePath, CancellationToken ct = default)
    {
        throw new NotImplementedException();
        yield break;
    }
}