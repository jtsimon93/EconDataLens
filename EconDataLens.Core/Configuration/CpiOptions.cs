﻿namespace EconDataLens.Core.Configuration;

/// <summary>
///     Strongly typed configuration options for retrieving Bureau of Labor Statistics (BLS) datasets.
/// </summary>
/// <remarks>
///     This class maps to the <c>BlsOptions:Cpi</c> section of <c>appsettings.json</c>.
///     It can be registered in the DI container using <see cref="Microsoft.Extensions.Options.IOptions{TOptions}" />.
///     Example configuration section:
///     <code lang="json">
/// "BlsOptions": {
///     "Cpi": {
///         "BaseUrl": "https://download.bls.gov/pub/time.series/cu/",
///          "DataFile": "cu.data.0.Current",
///          "AreaFile": "cu.area",
///          "FootnoteFile": "cu.footnote",
///          "ItemFile": "cu.item",
///          "PeriodFile": "cu.period",
///          "SeriesFile": "cu.series"
///     }
/// }
/// </code>
/// </remarks>
public class CpiOptions
{
    /// <summary>
    /// Gets or sets the base URL for downloading CPI data files from the BLS.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the main CPI data file name.
    /// </summary>
    public string DataFile { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the CPI area lookup file name.
    /// </summary>
    public string AreaFile { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the CPI footnote lookup file name.
    /// </summary>
    public string FootnoteFile { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the CPI item lookup file name.
    /// </summary>
    public string ItemFile { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the CPI period lookup file name.
    /// </summary>
    public string PeriodFile { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the CPI series lookup file name.
    /// </summary>
    public string SeriesFile { get; set; } = string.Empty;
}