namespace MonS3ApiLight.Models;

public class LineInfo
{
    public string Linka { get; set; } = string.Empty;
    public decimal TotalKg { get; set; }
    public int OrdersCount { get; set; }
    public decimal CapacityPct { get; set; }
    public List<string> Emails { get; set; } = new();
}
