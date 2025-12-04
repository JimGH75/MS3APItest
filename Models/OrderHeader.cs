namespace MonS3ApiLight.Models;

public class OrderHeader
{
    public long CRadku { get; set; }
    public string Doklad { get; set; } = string.Empty;
    public DateTime Vystaveno { get; set; }
    public string Firma { get; set; } = string.Empty;
    public string Mesto { get; set; } = string.Empty;
    public string Ulice { get; set; } = string.Empty;
    public string PSC { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Linka { get; set; } = string.Empty;
    public decimal Cena { get; set; }
    public decimal VahaKg { get; set; }
}
