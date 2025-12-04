namespace MonS3ApiLight.Models;

public class AddressRecord
{
    public long Cislo { get; set; }
    public string Nazev { get; set; } = string.Empty;
    public string Misto { get; set; } = string.Empty;
    public string Ulice { get; set; } = string.Empty;
    public string PSC { get; set; } = string.Empty;
    public string AdrKlice { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
