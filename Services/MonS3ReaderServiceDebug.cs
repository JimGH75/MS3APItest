using MonS3API;
using Microsoft.Extensions.Options;
using MonS3ApiLight.Config;

namespace MonS3ApiLight.Services;

public class MonS3ReaderServiceDebug : IDisposable
{
    private readonly S3Config _cfg;
    private MonS3APIDataMain _main;
    private MonS3APIDataProgram _program;

    public List<string> Log { get; private set; } = new List<string>();

    public MonS3ReaderServiceDebug(IOptions<S3Config> cfg)
    {
        _cfg = cfg.Value;
        _main = new MonS3APIDataMain();
    }

    public bool Init()
    {
        try
        {
            Log.Add("=== MonS3ReaderServiceDebug Init ===");

            // 1) Najdeme DLL
            string dllPath = string.IsNullOrWhiteSpace(_cfg.DllPath)
                ? _main.FindDLL(MonS3APIDataMain.MonS3APIDLLType.MonS3Reader)
                : _cfg.DllPath;
            Log.Add($"DLL Path: {dllPath}");

            // 2) Načtení DLL
            _main.LoadDLL(dllPath, "MonS3ApiLight");
            Log.Add("DLL načtena.");

            // 3) Nastavení datové cesty
            _main.SetDataPath(_cfg.DataPath);
            Log.Add($"DataPath nastaven: {_cfg.DataPath}");

            // 4) Vytvoření instance programu
            _program = _main.GetProgramInstance();
            Log.Add("Instance programu vytvořena.");

            // 5) Připojení k datům
            _program.ConnectData();
            Log.Add("ConnectData voláno.");

            // 6) Login
            _program.Login(_cfg.Password ?? "");
            Log.Add("Login voláno.");

            Log.Add("=== Inicializace dokončena ===");
            return true;
        }
        catch (Exception ex)
        {
            Log.Add($"Chyba při inicializaci: {ex.Message}");
            return false;
        }
    }

    public void Dispose()
    {
        try
        {
            _program?.DisconnectData();
            Log.Add("DisconnectData voláno.");
        }
        catch (Exception ex)
        {
            Log.Add($"Chyba při Disconnect: {ex.Message}");
        }
        finally
        {
            _main?.UnLoadDLL();
            Log.Add("DLL uvolněna.");
        }
    }

    // Základní testovací metody
    public IEnumerable<dynamic> TestLoadAddresses()
    {
        var what = new MonS3APIDataWhatList(); what.AddAll();
        var where = new MonS3APIDataWhereList();

        var rows = _program.GetRows("AdresarF", what, where);
        while (rows.Next())
        {
            yield return new
            {
                Cislo = rows.GetColByName("Cislo").AsInt,
                Nazev = rows.GetColByName("Nazev").AsString
            };
        }
    }

    public IEnumerable<dynamic> TestLoadOrders()
    {
        var what = new MonS3APIDataWhatList(); what.AddAll();
        var where = new MonS3APIDataWhereList();
        where.AddWhere("Druh", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateString("R"));

        var rows = _program.GetRows("ObjPrijHl", what, where);
        while (rows.Next())
        {
            yield return new
            {
                Doklad = rows.GetColByName("Doklad").AsString,
                Celkem = rows.GetColByName("Celkem").AsDecimal
            };
        }
    }
}
