using MonS3API;
using Microsoft.Extensions.Options;
using MonS3ApiLight.Config;

namespace MonS3ApiLight.Services;

public class MonS3ReaderService : IDisposable
{
    private readonly S3Config _cfg;
    private MonS3APIDataMain _main;
    private MonS3APIDataProgram _program;

    public MonS3ReaderService(IOptions<S3Config> cfg)
    {
        _cfg = cfg.Value;
        _main = new MonS3APIDataMain();
    }

    public void Init()
    {
        // load DLL and set data path and login
        string dll = _cfg.DllPath;
        if (string.IsNullOrWhiteSpace(dll))
            dll = _main.FindDLL(MonS3APIDataMain.MonS3APIDLLType.MonS3Reader);

        _main.LoadDLL(dll, "MonS3ApiLight");
        _main.SetDataPath(_cfg.DataPath);

        _program = _main.GetProgramInstance();
        _program.ConnectData();

        if (!string.IsNullOrWhiteSpace(_cfg.Password))
        {
            string hash = _program.TranslatePassword(_cfg.Password);
            _program.Login(hash);
        }
        else
        {
            // try login with empty password (depends on setup)
            try { _program.Login(""); } catch { }
        }
    }

    public void Dispose()
    {
        try
        {
            _program?.DisconnectData();
        }
        catch { }
        finally
        {
            _main?.UnLoadDLL();
        }
    }

    // Load addresses from AdresarF
    public IEnumerable<dynamic> LoadAddressRaw()
    {
        var what = new MonS3APIDataWhatList();
        what.AddAll();

        var where = new MonS3APIDataWhereList();
        var rows = _program.GetRows("AdresarF", what, where);
        while (rows.Next())
        {
            yield return new {
                Cislo = rows.GetColByName("Cislo").AsInt32,
                Nazev = rows.GetColByName("Nazev").AsString,
                Misto = rows.GetColByName("Misto").AsString,
                Ulice = rows.GetColByName("Ulice").AsString,
                PSC = rows.GetColByName("PSC").AsString,
                AdrKlice = rows.GetColByName("AdrKlice").AsString,
                EMail = rows.GetColByName("EMail").AsString
            };
        }
    }

    // Load order items from ObjPrijPol
    public IEnumerable<dynamic> LoadItemsRaw()
    {
        var what = new MonS3APIDataWhatList(); what.AddAll();
        var where = new MonS3APIDataWhereList();
        var rows = _program.GetRows("ObjPrijPol", what, where);
        while (rows.Next())
        {
            yield return new {
                Cislo = rows.GetColByName("Cislo").AsInt32,
                CisloPoloz = rows.GetColByName("CisloPoloz").AsInt32,
                ZbyvaMJ = rows.GetColByName("ZbyvaMJ").AsDecimal,
                Hmotnost = rows.GetColByName("Hmotnost").AsDecimal,
                Popis = rows.GetColByName("Popis").AsString
            };
        }
    }

    // Load order headers from ObjPrijHl (only Druh = 'R' and not Vyrizeno)
    public IEnumerable<dynamic> LoadOrdersRaw()
    {
        var what = new MonS3APIDataWhatList(); what.AddAll();
        var where = new MonS3APIDataWhereList();
        where.AddWhere("Druh", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateString("R"));
        var rows = _program.GetRows("ObjPrijHl", what, where);

        while (rows.Next())
        {
            var vyrizeno = rows.GetColByName("Vyrizeno").AsDate;
            if (vyrizeno != DateTime.MinValue && vyrizeno != default(DateTime))
            {
                // skip finished
                continue;
            }

            yield return new {
                CRadku = rows.GetColByName("CRadku").AsInt32,
                Doklad = rows.GetColByName("Doklad").AsString,
                Vystaveno = rows.GetColByName("Vystaveno").AsDate,
                KonecPrij = rows.GetColByName("KonecPrij").AsInt32,
                Adresa = rows.GetColByName("Adresa").AsString,
                Celkem = rows.GetColByName("Celkem").AsDecimal
            };
        }
    }
}
