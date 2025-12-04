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
        // 1) Najdeme nebo použijeme DLL
        string dllPath =
            string.IsNullOrWhiteSpace(_cfg.DllPath)
                ? _main.FindDLL(MonS3APIDataMain.MonS3APIDLLType.MonS3Reader)
                : _cfg.DllPath;

        if (!_main.LoadDLL(dllPath, "MonS3ApiLight"))
            throw new Exception("Nepodaøilo se naèíst knihovnu DLL: " + dllPath);

        // 2) Nastavíme cestu k datùm – bez validace
        if (!_main.SetDataPath(_cfg.DataPath))
            throw new Exception("MonS3Reader odmítl cestu k datùm: " + _cfg.DataPath);

        // 3) Instance programu
        _program = _main.GetProgramInstance();

        // 4) Pøipojení k databázi
        if (!_program.ConnectData())
            throw new Exception("Nepodaøilo se pøipojit k Money S3 datùm.");

        // 5) Login – nepøekládáme heslo, nepoužíváme hash
        //    pouze pøesnì to, co je v configu (i prázdný string)
        if (!_program.Login(_cfg.Password ?? ""))
        {
            throw new Exception("Login selhal – nesprávné heslo nebo nedostateèná práva.");
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

    public IEnumerable<dynamic> LoadAddressRaw()
    {
        var what = new MonS3APIDataWhatList();
        what.AddAll();

        var where = new MonS3APIDataWhereList();
        var rows = _program.GetRows("AdresarF", what, where);
        while (rows.Next())
        {
            yield return new
            {
                Cislo = rows.GetColByName("Cislo").AsInt,
                Nazev = rows.GetColByName("Nazev").AsString,
                Misto = rows.GetColByName("Misto").AsString,
                Ulice = rows.GetColByName("Ulice").AsString,
                PSC = rows.GetColByName("PSC").AsString,
                AdrKlice = rows.GetColByName("AdrKlice").AsString,
                EMail = rows.GetColByName("EMail").AsString
            };
        }
    }

    public IEnumerable<dynamic> LoadItemsRaw()
    {
        var what = new MonS3APIDataWhatList();
        what.AddAll();
        var where = new MonS3APIDataWhereList();
        var rows = _program.GetRows("ObjPrijPol", what, where);

        while (rows.Next())
        {
            yield return new
            {
                Cislo = rows.GetColByName("Cislo").AsInt,
                CisloPoloz = rows.GetColByName("CisloPoloz").AsInt,
                ZbyvaMJ = rows.GetColByName("ZbyvaMJ").AsDecimal,
                Hmotnost = rows.GetColByName("Hmotnost").AsDecimal,
                Popis = rows.GetColByName("Popis").AsString
            };
        }
    }

    public IEnumerable<dynamic> LoadOrdersRaw()
    {
        var what = new MonS3APIDataWhatList();
        what.AddAll();
        var where = new MonS3APIDataWhereList();
        where.AddWhere("Druh", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateString("R"));

        var rows = _program.GetRows("ObjPrijHl", what, where);

        while (rows.Next())
        {
            var vyrizeno = rows.GetColByName("Vyrizeno").AsDate;
            if (vyrizeno != DateTime.MinValue)
                continue;

            yield return new
            {
                CRadku = rows.GetColByName("CRadku").AsInt,
                Doklad = rows.GetColByName("Doklad").AsString,
                Vystaveno = rows.GetColByName("Vystaveno").AsDate,
                KonecPrij = rows.GetColByName("KonecPrij").AsInt,
                Adresa = rows.GetColByName("Adresa").AsString,
                Celkem = rows.GetColByName("Celkem").AsDecimal
            };
        }
    }
}
