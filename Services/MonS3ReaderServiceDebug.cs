using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MonS3API;

namespace MonS3ApiLight.Services
{
    public class MonS3ReaderServiceDebug
    {
        private readonly string _dllPath;
        private readonly string _dataPath;
        private readonly string _password;
        private readonly string _agendaExt;

        public List<string> Log { get; } = new List<string>();

        // Konstruktor používající konfiguraci z appsettings.json
        public MonS3ReaderServiceDebug(IConfiguration config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            _dllPath = config["S3:DllPath"] ?? throw new ArgumentException("DllPath není definován v konfiguraci");
            _dataPath = config["S3:DataPath"] ?? throw new ArgumentException("DataPath není definován v konfiguraci");
            _password = config["S3:Password"] ?? throw new ArgumentException("Password není definován v konfiguraci");
            _agendaExt = config["S3:AgendaExt"] ?? throw new ArgumentException("AgendaExt není definován v konfiguraci");
        }


        // =====================================================================
        //  INIT
        // =====================================================================
        public bool Init()
        {
            Log.Clear();
            try
            {
                Log.Add("=== MonS3ReaderServiceDebug Init ===");
                Log.Add($"DLL Path: {_dllPath}");

                var main = new MonS3APIDataMain();
                main.LoadDLL(_dllPath, "MonS3ApiLight");
                main.SetDataPath(_dataPath);

                Log.Add($"Root DataPath: {_dataPath}");

                var program = main.GetProgramInstance();
                program.ConnectData();
                program.Login(program.TranslatePassword(_password));
                program.DisconnectData();

                // Výpis dostupných agend
                var agendas = program.GetListAgend();
                Log.Add("Dostupné agendy:");
                foreach (var a in agendas)
                    Log.Add($" - {a.Ext}");

                // Najít agendu
                var agendaItem = agendas.GetByExt(_agendaExt);
                if (agendaItem == null)
                {
                    Log.Add($"Chyba: Agenda '{_agendaExt}' nenalezena v DataPath");
                    main.UnLoadDLL();
                    return false;
                }

                var agenda = main.GetAgendaInstance();
                agenda.SetAgenda(_agendaExt);
                agenda.ConnectData();

                Log.Add("=== Inicializace OK ===");

                agenda.DisconnectData();
                main.UnLoadDLL();

                return true;
            }
            catch (Exception ex)
            {
                Log.Add($"Chyba při inicializaci: {ex.Message}");
                return false;
            }
        }


        // =====================================================================
        //  LOAD YEARS
        // =====================================================================
        public List<Dictionary<string, object>> TestGetYearList()
        {
            var results = new List<Dictionary<string, object>>();

            try
            {
                var main = new MonS3APIDataMain();
                main.LoadDLL(_dllPath, "MonS3ApiLight");
                main.SetDataPath(_dataPath);

                var agenda = main.GetAgendaInstance();
                agenda.SetAgenda(_agendaExt);
                agenda.ConnectData();

                var yearList = agenda.GetYearList();

                foreach (var y in yearList)
                {
                    results.Add(new Dictionary<string, object>
                    {
                        ["Year"] = y.Year,
                        ["DateFrom"] = y.DateFrom,
                        ["DateTo"] = y.DateTo,
                        ["Ext"] = y.Ext
                    });
                }

                agenda.DisconnectData();
                main.UnLoadDLL();
            }
            catch (Exception ex)
            {
                Log.Add($"Chyba při načítání YearList: {ex.Message}");
            }

            return results;
        }


        // =====================================================================
        //  LOAD ROWS – univerzální SELECT *
        // =====================================================================
        private List<Dictionary<string, object>> GetRows(string table)
        {
            var results = new List<Dictionary<string, object>>();

            try
            {
                var main = new MonS3APIDataMain();
                main.LoadDLL(_dllPath, "MonS3ApiLight");
                main.SetDataPath(_dataPath);

                var program = main.GetProgramInstance();
                program.ConnectData();
                program.Login(program.TranslatePassword(_password));
                program.DisconnectData();

                var agenda = main.GetAgendaInstance();
                agenda.SetAgenda(_agendaExt);
                agenda.ConnectData();

                // Výběr všech sloupců
                var what = new MonS3APIDataWhatList();
                what.AddAll();
                var where = new MonS3APIDataWhereList();

                var rows = agenda.GetRows(table, what, where);

                // **sloupce tabulky přes GetTableColumns → to je správně**
                MonS3APITableColumnList columnList = program.GetTableColumns(table);

                while (rows.Next())
                {
                    var entry = new Dictionary<string, object>();

                    foreach (var col in columnList)
                    {
                        string name = col.Name;

                        // správný způsob čtení hodnoty
                        var v = rows.GetColByName(name);

                        entry[name] = v.AsString; // vždy existuje
                    }

                    results.Add(entry);
                }

                agenda.DisconnectData();
                main.UnLoadDLL();
            }
            catch (Exception ex)
            {
                Log.Add($"Chyba při načítání {table}: {ex.Message}");
            }

            return results;
        }


        // =====================================================================
        //  PUBLIC TEST ENDPOINTS
        // =====================================================================
        public List<Dictionary<string, object>> TestLoadAddresses()
            => GetRows("AdresarF");

        public List<Dictionary<string, object>> TestLoadOrders()
            => GetRows("Objednavky");
    }
}
