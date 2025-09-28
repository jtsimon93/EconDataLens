namespace EconDataLens.Core.Configuration;

public class DownloadOptions
{
    public string DownloadDirectory { get; set; } = "downloads";
    public bool DeleteDownloadedFiles = false;
}