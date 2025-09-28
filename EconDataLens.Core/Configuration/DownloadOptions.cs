namespace EconDataLens.Core.Configuration;

public class DownloadOptions
{
    public bool DeleteDownloadedFiles = false;
    public string DownloadDirectory { get; set; } = "downloads";
}