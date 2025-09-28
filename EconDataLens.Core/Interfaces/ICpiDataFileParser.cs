using EconDataLens.Core.Entities.Cpi;

namespace EconDataLens.Core.Interfaces;

/// <summary>
///     Defines methods for parsing various Bureau of Labor Statistics (BLS) Consumer Price Index (CPI) data files
///     into strongly typed entities.
/// </summary>
public interface ICpiDataFileParser
{
    /// <summary>
    ///     Parses the CPI area reference file (<c>cu.area</c>) and returns an asynchronous stream of <see cref="CpiArea" />
    ///     entities.
    /// </summary>
    /// <param name="filePath">
    ///     Optional file path to the <c>cu.area</c> file. If null, a default path may be used.
    /// </param>
    /// <param name="ct">Cancellation token to observe while awaiting results.</param>
    /// <returns></returns>
    IAsyncEnumerable<CpiArea> ParseCpiAreasAsync(string? filePath, CancellationToken ct = default);

    /// <summary>
    ///     Parses the main CPI data file (<c>cu.data.0.Current</c>) and returns an asynchronous stream of
    ///     <see cref="CpiData" /> entities.
    /// </summary>
    /// <param name="filePath">Optional file path to the <c>cu.data.0.Current</c> file. If null, a default path may be used.</param>
    /// <param name="ct">Cancellation token to observe while awaiting results.</param>
    /// <returns></returns>
    IAsyncEnumerable<CpiData> ParseCpiDataAsync(string? filePath, CancellationToken ct = default);

    /// <summary>
    ///     Parses the CPI footnote reference fil (<c>cu.footnote</c>) and returns an asynchronous stream of
    ///     <see cref="CpiFootnote" /> entities.
    /// </summary>
    /// <param name="filePath">Optional file path to the <c>cu.footnote</c> file. If null, a default path may be used.</param>
    /// <param name="ct">Cancellation token to observe while awaiting results.</param>
    /// <returns>
    ///     An asynchronous stream of <see cref="CpiFootnote" /> entities, where each element represents a single footnote
    ///     definition from the source file. Consumers should enumerate the sequence with <c>await foreach</c>.
    /// </returns>
    IAsyncEnumerable<CpiFootnote> ParseCpiFootnoteAsync(string? filePath, CancellationToken ct = default);

    /// <summary>
    ///     Parses the CPI item reference file (<c>cu.item</c>) and returns an asynchronous stream of <see cref="CpiItem" />
    ///     entities.
    /// </summary>
    /// <param name="filePath">Optional file path to the <c>cu.item</c> file. If null, a default path may be used.</param>
    /// <param name="ct">Cancellation token to observe while awaiting results.</param>
    /// <returns>
    ///     An asynchronous stream of <see cref="CpiItem" /> entities, where each element represents a single item
    ///     definition from the source file. Consumers should enumerate with <c>await foreach</c>.
    /// </returns>
    IAsyncEnumerable<CpiItem> ParseCpiItemsAsync(string? filePath, CancellationToken ct = default);

    /// <summary>
    ///     Parses the CPI period reference file (<c>cu.period</c>) and returns an asynchronous stream of
    ///     <see cref="CpiPeriod" /> entities.
    /// </summary>
    /// <param name="filePath">Optional file path to the <c>cu.period</c> file. If null, a default file path may be used.</param>
    /// <param name="ct">Cancellation token to observe while awaiting results.</param>
    /// <returns>
    ///     An asynchronous stream of <see cref="CpiPeriod" /> entities, where each element represents a single period
    ///     definition from the source file. Consumers should enumerate with <c>await foreach</c>.
    /// </returns>
    IAsyncEnumerable<CpiPeriod> ParseCpiPeriodsAsync(string? filePath, CancellationToken ct = default);

    /// <summary>
    ///     Parses the CPI series reference file (<c>cu.series</c>) and returns an asynchronous stream of
    ///     <see cref="CpiSeries" /> entities.
    /// </summary>
    /// <param name="filePath">Optional file path to the <c>cu.series</c> file. If null, a default path may be used.</param>
    /// <param name="ct">Cancellation token to observe while awaiting results.</param>
    /// <returns>
    ///     An asyncrhonous stream of <see cref="CpiSeries" /> entities, where each element represents a single seies
    ///     definition from the source file. Consumers should enumerate with <c>await foreach</c>.
    /// </returns>
    IAsyncEnumerable<CpiSeries> ParseCpiSeriesAsync(string? filePath, CancellationToken ct = default);
}