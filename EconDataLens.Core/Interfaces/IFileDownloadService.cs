namespace EconDataLens.Core.Interfaces;

public interface IFileDownloadService
{
    Task<string> DownloadFileAsync(string url, string destinationPath, CancellationToken ct = default);
}