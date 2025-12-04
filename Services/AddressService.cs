using MonS3ApiLight.Models;

namespace MonS3ApiLight.Services;

public class AddressService
{
    private readonly MonS3ReaderService _reader;
    private List<AddressRecord>? _cache;

    public AddressService(MonS3ReaderService reader)
    {
        _reader = reader;
        // initialize and load on demand
        _reader.Init();
    }

    public List<AddressRecord> GetAddresses()
    {
        if (_cache != null) return _cache;

        _cache = new List<AddressRecord>();
        foreach (var a in _reader.LoadAddressRaw())
        {
            _cache.Add(new AddressRecord
            {
                Cislo = (long)a.Cislo,
                Nazev = a.Nazev,
                Misto = a.Misto,
                Ulice = a.Ulice,
                PSC = a.PSC,
                AdrKlice = a.AdrKlice ?? string.Empty,
                Email = a.EMail ?? string.Empty
            });
        }
        return _cache;
    }
}
