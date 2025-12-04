namespace MonS3ApiLight.Models;

public class OrderItem
{
    public long Cislo { get; set; }
    public int Poradi { get; set; }
    public string Popis { get; set; } = string.Empty;
    public decimal Pocet { get; set; }
    public decimal Hmotnost { get; set; }
    public decimal VahaKg => Pocet * Hmotnost;
}
