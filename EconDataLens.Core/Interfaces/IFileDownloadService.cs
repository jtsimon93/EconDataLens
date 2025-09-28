namespace EconDataLens.Core.Interfaces;

/// <summary>
///     Contract for a service that handles file downloading operations.
/// </summary>
/// <remarks>
///     Methods defined in this interface facilitate downloading files from specified URLs
///     and saving them to designated local paths. Implementations should handle network errors,
///     retries, and cancellation tokens as appropriate.
/// </remarks>
public interface IFileDownloadService
{
    /// <summary>
    ///     Asynchronously downloads a file from the specified URL and saves it to the given destination path.
    /// </summary>
    /// <param name="url">The URL to download from.</param>
    /// <param name="destinationPath">The path to store the downloaded file.</param>
    /// <param name="ct">A cancellation token to observe while awaiting the download.</param>
    /// <returns>
    ///     A task that resolves to the absolute path of the downloaded file
    ///     once the operation has completed successfully.
    /// </returns>
    Task<string> DownloadFileAsync(string url, string destinationPath, CancellationToken ct = default);
}