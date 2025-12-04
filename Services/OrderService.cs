using MonS3ApiLight.Models;

namespace MonS3ApiLight.Services;

public class OrderService
{
    private readonly MonS3ReaderService _reader;
    private readonly AddressService _addr;
    private readonly int _capacityKg;

    public OrderService(MonS3ReaderService reader, AddressService addr, IConfiguration cfg)
    {
        _reader = reader;
        _addr = addr;
        _capacityKg = cfg.GetValue<int>("Model:DefaultCapacityKg", 1500);
    }

    public IEnumerable<OrderHeader> LoadOrders()
    {
        var addr = _addr.GetAddresses();
        var items = _reader.LoadItemsRaw().Select(i => new {
            Cislo = (long)i.Cislo,
            CisloPoloz = (int)i.CisloPoloz,
            ZbyvaMJ = (decimal)i.ZbyvaMJ,
            Hmotnost = (decimal)i.Hmotnost,
            Popis = (string)i.Popis
        }).ToList();

        var result = new List<OrderHeader>();
        foreach (var o in _reader.LoadOrdersRaw())
        {
            long cis = (long)o.CRadku;
            var a = addr.FirstOrDefault(x => x.Cislo == (long)o.KonecPrij);
            string linka = ParseLinkFromAdrKlice(a?.AdrKlice);

            var orderItems = items.Where(it => it.Cislo == cis).Select(it => new OrderItem {
                Cislo = it.Cislo,
                Poradi = it.CisloPoloz,
                Popis = it.Popis,
                Pocet = it.ZbyvaMJ,
                Hmotnost = it.Hmotnost
            }).ToList();

            decimal totalKg = orderItems.Sum(it => it.VahaKg);

            result.Add(new OrderHeader {
                CRadku = cis,
                Doklad = o.Doklad,
                Vystaveno = o.Vystaveno,
                Firma = a?.Nazev ?? string.Empty,
                Mesto = a?.Misto ?? string.Empty,
                Ulice = a?.Ulice ?? string.Empty,
                PSC = a?.PSC ?? string.Empty,
                Email = a?.Email ?? string.Empty,
                Linka = linka,
                Cena = (decimal)o.Celkem,
                VahaKg = totalKg
            });
        }

        return result;
    }

    private string ParseLinkFromAdrKlice(string? klice)
    {
        if (string.IsNullOrWhiteSpace(klice)) return string.Empty;
        var parts = klice.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim());
        // return first matching token (simple heuristic)
        foreach (var p in parts)
        {
            if (p.Length>0) return p;
        }
        return string.Empty;
    }
}
