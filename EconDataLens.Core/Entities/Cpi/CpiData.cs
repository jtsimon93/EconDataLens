namespace EconDataLens.Core.Entities.Cpi;

public class CpiData
{
    public string SeriesId { get; set; } = null!;
    public int Year { get; set; }
    public string Period { get; set; } = null!;
    public decimal Value { get; set; }
    public string? FootnoteCodes { get; set; }
}