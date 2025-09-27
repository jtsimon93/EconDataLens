namespace EconDataLens.Core.Entities.Cpi;

public class CpiSeries
{
    public string SeriesId { get; set; } = null!;
    public string AreaCode { get; set; } = null!;
    public string ItemCode { get; set; } = null!;
    public string Seasonal { get; set; } = null!;
    public string PeriodicityCode { get; set; } = null!;
    public string BaseCode { get; set; } = null!;
    public string BasePeriod { get; set; } = null!;
    public string SeriesTitle { get; set; } = null!;
    public string? FootnoteCodes { get; set; }
    public int BeginYear { get; set; }
    public string BeginPeriod { get; set; } = null!;
    public int EndYear { get; set; }
    public string EndPeriod { get; set; } = null!;
}