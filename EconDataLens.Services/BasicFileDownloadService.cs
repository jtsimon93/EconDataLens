using System.Net;
using EconDataLens.Core.Interfaces;

namespace EconDataLens.Services;

public class BasicFileDownloadService : IFileDownloadService
{
    private readonly HttpClient _httpClient;

    public BasicFileDownloadService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _httpClient.Timeout = TimeSpan.FromMinutes(10);
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0 Safari/537.36");
        _httpClient.DefaultRequestHeaders.Accept.ParseAdd(
            "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        _httpClient.DefaultRequestHeaders.Referrer = new Uri("https://www.bls.gov/");
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.5");
        _httpClient.DefaultRequestHeaders.AcceptCharset.ParseAdd("utf-8");
        _httpClient.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");
    }

    public async Task<string> DownloadFileAsync(string url, string destinationPath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException(nameof(url));

        if (string.IsNullOrWhiteSpace(destinationPath)) throw new ArgumentNullException(nameof(destinationPath));

        destinationPath = Path.GetFullPath(destinationPath);
        var dir = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        var tempPath = destinationPath + ".partial";

        using var resp = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);

        if (resp.StatusCode == HttpStatusCode.NotModified)
            return destinationPath;

        resp.EnsureSuccessStatusCode();

        await using (var httpStream = await resp.Content.ReadAsStreamAsync(ct))
        await using (var fileStream =
                     new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true))
        {
            await httpStream.CopyToAsync(fileStream, ct);
            await fileStream.FlushAsync(ct);
        }

        if (File.Exists(destinationPath))
            File.Replace(tempPath, destinationPath, destinationPath + ".backup", true);
        else
            File.Move(tempPath, destinationPath);

        // Cleanup stray temp if replace filed mid-way
        if (File.Exists(tempPath)) File.Delete(tempPath);

        return destinationPath;
    }
}