namespace EconDataLens.Core.Configuration;

/// <summary>
/// Strongly typed configuration options for file download settings.
/// </summary>
/// <remarks>
/// This class maps to the <c>DownloadOptions</c> section of <c>appsettings.json</c>.
/// It can be registered in the DI container using <see cref="Microsoft.Extensions.Options.IOptions{TOptions}"/>.
///
/// Example configuration section:
/// <code lang="json">
/// "DownloadOptions": {
///     "DownloadDirectory": "downloads",
///     "DeleteDownloadedFiles": false
/// }
/// </code>
/// </remarks>
public class DownloadOptions
{
    /// <summary>
    /// Gets or sets the DeleteDownloadedFiles option.
    /// If true, downloaded files will be deleted after processing to save disk space.
    /// If false, downloaded files will be retained in the specified download directory.
    /// </summary>
    public bool DeleteDownloadedFiles { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the directory where downloaded files will be saved.
    /// This is a relative path from the application's working directory.
    /// </summary>
    public string DownloadDirectory { get; set; } = "downloads";
}