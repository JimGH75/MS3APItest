using System;
using System.Collections.Generic;
using System.Linq;
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

        public MonS3ReaderServiceDebug(IConfiguration config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            var moneyS3Section = config.GetSection("MoneyS3");

            _dllPath = moneyS3Section["DllPath"]
                ?? throw new ArgumentException("DllPath není definován v konfiguraci");
            _dataPath = moneyS3Section["DataPath"]
                ?? throw new ArgumentException("DataPath není definován v konfiguraci");
            _password = moneyS3Section["Password"]
                ?? throw new ArgumentException("Password není definován v konfiguraci");

            _agendaExt = moneyS3Section["AgendaExt"] ?? moneyS3Section["Agenda"]
                ?? throw new ArgumentException("AgendaExt/Agenda není definován v konfiguraci");
        }

        // =====================================================================
        //  NAČTENÍ ADRES - PŘESNÁ POSLOUPNOST Z MonS3APITest14
        // =====================================================================
        public List<Dictionary<string, object>> LoadAddresses()
        {
            Log.Clear();
            var results = new List<Dictionary<string, object>>();

            try
            {
                // PŘESNĚ JAKO V PŘÍKLADU MonS3APITest14:

                // 1. Hlavní instance
                MonS3APIDataMain main = new MonS3APIDataMain();
                main.LoadDLL(_dllPath, "MonS3ApiLight");
                Log.Add("1. DLL načtena");

                main.SetDataPath(_dataPath);
                Log.Add("2. DataPath nastaven");

                // 2. Program instance - PŘIHLÁŠENÍ
                MonS3APIDataProgram program = main.GetProgramInstance();
                program.ConnectData();
                Log.Add("3. Program připojen");

                // Přihlášení
                string passwordHash = program.TranslatePassword(_password);
                program.Login(passwordHash);
                Log.Add("4. Přihlášeno");

                // 3. ODEPOJIT PROGRAM - PŘESNĚ JAKO V PŘÍKLADU
                program.DisconnectData();
                Log.Add("5. Program odpojen (dle příkladu)");

                // 4. Agenda instance
                MonS3APIDataAgenda agenda = main.GetAgendaInstance();
                agenda.SetAgenda(_agendaExt);
                agenda.ConnectData();
                Log.Add($"6. Agenda '{_agendaExt}' připojena");

                // 5. Dotaz - MÍSTO "AdAkce" POUŽÍVÁME "AdresarF"
                MonS3APIDataWhatList what = new MonS3APIDataWhatList();
                what.AddAll(); // Chceme všechny sloupce

                MonS3APIDataWhereList where = new MonS3APIDataWhereList();
                // Žádné podmínky - chceme všechny záznamy

                // 6. Spustit dotaz na AGENDĚ - PŘESNĚ JAKO V PŘÍKLADU
                MonS3APIDataSelectResult selectResult = agenda.GetRows("AdresarF", what, where);
                Log.Add($"7. Dotaz spuštěn, nalezeno záznamů: {selectResult.Count}");

                // 7. Načíst data - PŘESNĚ JAKO V PŘÍKLADU
                int count = 0;
                while (selectResult.Next())
                {
                    count++;

                    // Použijeme GetColByName JAKO V PŘÍKLADU
                    // (v příkladu používají "Popis" a "Uzivatel", my použijeme sloupce z AdresarF)
                    var entry = new Dictionary<string, object>
                    {
                        // ZÁKLADNÍ SLOUPCE ADRESÁŘE
                        ["Cislo"] = selectResult.GetColByName("Cislo")?.AsString ?? "",
                        ["Nazev"] = selectResult.GetColByName("Nazev")?.AsString ?? "",
                        ["Misto"] = selectResult.GetColByName("Misto")?.AsString ?? "",
                        ["Ulice"] = selectResult.GetColByName("Ulice")?.AsString ?? "",
                        ["PSC"] = selectResult.GetColByName("PSC")?.AsString ?? "",
                        ["AdrKlice"] = selectResult.GetColByName("AdrKlice")?.AsString ?? "",
                        ["EMail"] = selectResult.GetColByName("EMail")?.AsString ?? ""
                    };

                    results.Add(entry);

                    // Log první záznam
                    if (count == 1)
                    {
                        Log.Add($"   První záznam: {entry["Nazev"]}");
                    }
                }

                Log.Add($"8. Načteno {count} adres");

                // 8. Ukončit - PŘESNĚ JAKO V PŘÍKLADU
                agenda.DisconnectData();
                Log.Add("9. Agenda odpojena");

                main.UnLoadDLL();
                Log.Add("10. DLL uvolněna");
                Log.Add("=== HOTOVO ===");

                return results;
            }
            catch (MonS3APIException ex)
            {
                Log.Add($"!!! Money S3 CHYBA: {ex.ExceptionType}");
                Log.Add($"    Zpráva: {ex.Message}");
                Log.Add($"    Detail: {ex.Detail}");
                throw;
            }
            catch (Exception ex)
            {
                Log.Add($"!!! OBECNÁ CHYBA: {ex.Message}");
                throw;
            }
        }

        // =====================================================================
        //  NAČTENÍ OBJEDNÁVEK - STEJNÁ POSLOUPNOST
        // =====================================================================
        public List<Dictionary<string, object>> LoadOrders()
        {
            Log.Clear();
            var results = new List<Dictionary<string, object>>();

            try
            {
                Log.Add("=== Načítání objednávek ===");

                // PŘESNĚ STEJNÁ POSLOUPNOST
                MonS3APIDataMain main = new MonS3APIDataMain();
                main.LoadDLL(_dllPath, "MonS3ApiLight");
                main.SetDataPath(_dataPath);

                MonS3APIDataProgram program = main.GetProgramInstance();
                program.ConnectData();

                string passwordHash = program.TranslatePassword(_password);
                program.Login(passwordHash);

                program.DisconnectData();

                MonS3APIDataAgenda agenda = main.GetAgendaInstance();
                agenda.SetAgenda(_agendaExt);
                agenda.ConnectData();

                // Dotaz - zkusíme různé názvy tabulek
                MonS3APIDataWhatList what = new MonS3APIDataWhatList();
                what.AddAll();

                MonS3APIDataWhereList where = new MonS3APIDataWhereList();

                try
                {
                    // Zkusit Objednavky
                    MonS3APIDataSelectResult selectResult = agenda.GetRows("ObSezObj", what, where);
                    Log.Add($"Nalezeno v 'ObSezObj': {selectResult.Count}");

                    while (selectResult.Next())
                    {
                        results.Add(new Dictionary<string, object>
                        {
                            ["CRadku"] = selectResult.GetColByName("CRadku")?.AsString ?? "",
                            ["Doklad"] = selectResult.GetColByName("Doklad")?.AsString ?? "",
                            ["Vystaveno"] = selectResult.GetColByName("Vystaveno")?.AsString ?? "",
                            ["KonecPrij"] = selectResult.GetColByName("KonecPrij")?.AsString ?? "",
                            ["Celkem"] = selectResult.GetColByName("Celkem")?.AsString ?? ""
                        });
                    }
                }
                catch (MonS3APIException ex) when (ex.ExceptionType == MonS3APIExceptionType.TableNotFound)
                {
                    ////////////////// - --tady asi dopnit nějakou hlášku že tabuka neexsituje ---
                }

                agenda.DisconnectData();
                main.UnLoadDLL();

                Log.Add($"Načteno {results.Count} objednávek");
                return results;
            }
            catch (Exception ex)
            {
                Log.Add($"Chyba: {ex.Message}");
                throw;
            }
        }

        // =====================================================================
        //  NAČTENÍ ROKŮ - STEJNÁ POSLOUPNOST
        // =====================================================================
        public List<Dictionary<string, object>> LoadYears()
        {
            Log.Clear();
            var results = new List<Dictionary<string, object>>();

            try
            {
                Log.Add("=== Načítání roků ===");

                // PŘESNĚ STEJNÁ POSLOUPNOST
                MonS3APIDataMain main = new MonS3APIDataMain();
                main.LoadDLL(_dllPath, "MonS3ApiLight");
                main.SetDataPath(_dataPath);

                MonS3APIDataProgram program = main.GetProgramInstance();
                program.ConnectData();

                string passwordHash = program.TranslatePassword(_password);
                program.Login(passwordHash);

                program.DisconnectData();

                MonS3APIDataAgenda agenda = main.GetAgendaInstance();
                agenda.SetAgenda(_agendaExt);
                agenda.ConnectData();

                MonS3APIYearList yearList = agenda.GetYearList();

                foreach (MonS3APIYear y in yearList)
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

                Log.Add($"Načteno {results.Count} roků");
                return results;
            }
            catch (Exception ex)
            {
                Log.Add($"Chyba: {ex.Message}");
                throw;
            }
        }

        // =====================================================================
        //  NAČTENÍ OBJEDNÁVEK PRO KONKRÉTNÍ ROK
        // =====================================================================
        public List<Dictionary<string, object>> LoadOrdersForYear(int year)
        {
            Log.Clear();
            var results = new List<Dictionary<string, object>>();

            try
            {
                Log.Add($"=== Načítání objednávek pro rok {year} ===");

                // STEJNÁ POSLOUPNOST
                MonS3APIDataMain main = new MonS3APIDataMain();
                main.LoadDLL(_dllPath, "MonS3ApiLight");
                main.SetDataPath(_dataPath);

                MonS3APIDataProgram program = main.GetProgramInstance();
                program.ConnectData();

                string passwordHash = program.TranslatePassword(_password);
                program.Login(passwordHash);

                program.DisconnectData();

                MonS3APIDataAgenda agenda = main.GetAgendaInstance();
                agenda.SetAgenda(_agendaExt);
                agenda.ConnectData();

                // 1. Získat seznam roků a najít ten správný
                var yearList = agenda.GetYearList();
                var targetYear = yearList.FirstOrDefault(y => y.Year == year);

                if (targetYear == null)
                {
                    Log.Add($"Rok {year} nenalezen v agendě!");
                    Log.Add($"Dostupné roky: {string.Join(", ", yearList.Select(y => y.Year))}");
                    throw new Exception($"Rok {year} neexistuje v agendě {_agendaExt}");
                }

                Log.Add($"Pracuji s rokem: {targetYear.Year} ({targetYear.DateFrom:dd.MM.yyyy} - {targetYear.DateTo:dd.MM.yyyy})");

                // 2. Zkusit najít tabulku s objednávkami
                string[] possibleOrderTables = { "ObSezObj" };
                string foundTable = null;

                foreach (var tableName in possibleOrderTables)
                {
                    try
                    {
                        // Test, zda tabulka existuje
                        var what = new MonS3APIDataWhatList();
                        what.AddAll();
                        var where = new MonS3APIDataWhereList();
                        where.AddWhere("1", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateInt(1));

                        // Limit 1 záznam jen pro test
                        var testRows = agenda.GetRows(tableName, what, where, null, 0, 1);
                        if (testRows.Count >= 0) // Tabulka existuje
                        {
                            foundTable = tableName;
                            Log.Add($"Nalezena tabulka: {tableName}");
                            break;
                        }
                    }
                    catch (MonS3APIException ex) when (ex.ExceptionType == MonS3APIExceptionType.TableNotFound)
                    {
                        // Tabulka neexistuje, pokračujeme
                        continue;
                    }
                    catch
                    {
                        // Jiná chyba - pokračujeme
                        continue;
                    }
                }

                if (foundTable == null)
                {
                    throw new Exception($"Nenalezena žádná tabulka s objednávkami. Zkoušel jsem: {string.Join(", ", possibleOrderTables)}");
                }

                // 3. Načíst objednávky
                var whatAll = new MonS3APIDataWhatList();
                whatAll.AddAll();
                var whereAll = new MonS3APIDataWhereList();

                var rows = agenda.GetRows(foundTable, whatAll, whereAll);
                Log.Add($"Celkem záznamů v {foundTable}: {rows.Count}");

                int count = 0;
                while (rows.Next())
                {
                    count++;

                    var entry = new Dictionary<string, object>();

                    // Společné sloupce objednávek
                    string[] commonColumns = { "CRadku", "Doklad", "Vystaveno", "DatVyst", "DatSplat",
                                               "Celkem", "Druh", "Vyrizeno", "KonecPrij", "Adresa" };

                    foreach (var col in commonColumns)
                    {
                        try
                        {
                            var value = rows.GetColByName(col);
                            entry[col] = value?.AsString ?? "";
                        }
                        catch
                        {
                            entry[col] = "";
                        }
                    }

                    results.Add(entry);

                    // Log první záznam
                    if (count == 1)
                    {
                        Log.Add($"První záznam: {entry["Doklad"]} ({entry["Vystaveno"]}) - {entry["Celkem"]} Kč");
                    }
                }

                Log.Add($"Načteno {count} záznamů z {foundTable}");

                // 4. Ukončit
                agenda.DisconnectData();
                main.UnLoadDLL();

                return results;
            }
            catch (Exception ex)
            {
                Log.Add($"Chyba: {ex.Message}");
                throw;
            }
        }

        // =====================================================================
        //  PROHLEDÁNÍ VŠECH ROKŮ V AGENDĚ
        // =====================================================================
        public Dictionary<string, object> ExploreAllYears()
        {
            Log.Clear();
            var result = new Dictionary<string, object>();
            var yearResults = new Dictionary<int, object>();

            try
            {
                Log.Add("=== Prohledávání všech roků v agendě ===");

                var main = new MonS3APIDataMain();
                main.LoadDLL(_dllPath, "MonS3ApiLight");
                main.SetDataPath(_dataPath);

                var program = main.GetProgramInstance();
                program.ConnectData();

                string passwordHash = program.TranslatePassword(_password);
                program.Login(passwordHash);

                program.DisconnectData();

                var agenda = main.GetAgendaInstance();
                agenda.SetAgenda(_agendaExt);
                agenda.ConnectData();

                // Získat všechny roky
                var yearList = agenda.GetYearList();
                Log.Add($"Nalezeno roků: {yearList.Count}");

                foreach (var yearItem in yearList)
                {
                    Log.Add($"\n--- Zkoumám rok {yearItem.Year} ({yearItem.DateFrom:dd.MM.yyyy} - {yearItem.DateTo:dd.MM.yyyy}) ---");

                    try
                    {
                        // Zkusit najít tabulky s objednávkami v tomto roce
                        string[] tablesToCheck = { "ObjPrijHl", "Doklady", "Objednavky", "Faktury", "PrijHl" };
                        var foundTables = new List<string>();

                        foreach (var table in tablesToCheck)
                        {
                            try
                            {
                                var what = new MonS3APIDataWhatList();
                                what.AddAll();
                                var where = new MonS3APIDataWhereList();

                                // Zkusit načíst max 1 záznam
                                var testRows = agenda.GetRows(table, what, where, null, 0, 1);
                                if (testRows.Count > 0)
                                {
                                    foundTables.Add($"{table} ({testRows.Count} záznamů)");
                                }
                                else
                                {
                                    foundTables.Add($"{table} (0 záznamů)");
                                }
                            }
                            catch (MonS3APIException ex) when (ex.ExceptionType == MonS3APIExceptionType.TableNotFound)
                            {
                                // Tabulka neexistuje
                            }
                            catch
                            {
                                // Jiná chyba
                            }
                        }

                        yearResults[yearItem.Year] = new
                        {
                            year = yearItem.Year,
                            dateFrom = yearItem.DateFrom,
                            dateTo = yearItem.DateTo,
                            ext = yearItem.Ext,
                            foundTables = foundTables
                        };

                        Log.Add($"Rok {yearItem.Year}: {string.Join(", ", foundTables)}");
                    }
                    catch (Exception ex)
                    {
                        Log.Add($"Chyba v roce {yearItem.Year}: {ex.Message}");
                        yearResults[yearItem.Year] = new { error = ex.Message };
                    }
                }

                agenda.DisconnectData();
                main.UnLoadDLL();

                result["success"] = true;
                result["years"] = yearResults;
                result["log"] = new List<string>(Log);

                return result;
            }
            catch (Exception ex)
            {
                Log.Add($"Chyba: {ex.Message}");

                result["success"] = false;
                result["error"] = ex.Message;
                result["log"] = new List<string>(Log);

                return result;
            }
        }

        // =====================================================================
        //  NAČTENÍ POLOŽEK OBJEDNÁVEK PRO KONKRÉTNÍ ROK
        // =====================================================================
        public List<Dictionary<string, object>> LoadOrderItemsForYear(int year)
        {
            Log.Clear();
            var results = new List<Dictionary<string, object>>();

            try
            {
                Log.Add($"=== Načítání položek objednávek pro rok {year} ===");

                var main = new MonS3APIDataMain();
                main.LoadDLL(_dllPath, "MonS3ApiLight");
                main.SetDataPath(_dataPath);

                var program = main.GetProgramInstance();
                program.ConnectData();

                string passwordHash = program.TranslatePassword(_password);
                program.Login(passwordHash);

                program.DisconnectData();

                var agenda = main.GetAgendaInstance();
                agenda.SetAgenda(_agendaExt);
                agenda.ConnectData();

                // Zkusit najít tabulku s položkami
                string[] possibleTables = { "ObjPrijPol", "DokladyPol", "ObjednavkyPol", "FakturyPol" };
                string foundTable = null;

                foreach (var tableName in possibleTables)
                {
                    try
                    {
                        var what = new MonS3APIDataWhatList();
                        what.AddAll();
                        var where = new MonS3APIDataWhereList();

                        var testRows = agenda.GetRows(tableName, what, where, null, 0, 1);
                        foundTable = tableName;
                        Log.Add($"Používám tabulku: {tableName}");
                        break;
                    }
                    catch (MonS3APIException ex) when (ex.ExceptionType == MonS3APIExceptionType.TableNotFound)
                    {
                        continue;
                    }
                }

                if (foundTable == null)
                {
                    throw new Exception($"Nenalezena tabulka s položkami objednávek");
                }

                var whatAll = new MonS3APIDataWhatList();
                whatAll.AddAll();
                var whereAll = new MonS3APIDataWhereList();

                var rows = agenda.GetRows(foundTable, whatAll, whereAll);
                Log.Add($"Nalezeno položek: {rows.Count}");

                int count = 0;
                while (rows.Next())
                {
                    count++;
                    var entry = new Dictionary<string, object>
                    {
                        ["Cislo"] = rows.GetColByName("Cislo")?.AsString ?? "",
                        ["CisloPoloz"] = rows.GetColByName("CisloPoloz")?.AsString ?? "",
                        ["Popis"] = rows.GetColByName("Popis")?.AsString ?? "",
                        ["MJ"] = rows.GetColByName("MJ")?.AsString ?? "",
                        ["Mnozstvi"] = rows.GetColByName("Mnozstvi")?.AsString ?? "",
                        ["Cena"] = rows.GetColByName("Cena")?.AsString ?? "",
                        ["Celkem"] = rows.GetColByName("Celkem")?.AsString ?? ""
                    };

                    results.Add(entry);
                }

                Log.Add($"Načteno {count} položek");

                agenda.DisconnectData();
                main.UnLoadDLL();

                return results;
            }
            catch (Exception ex)
            {
                Log.Add($"Chyba: {ex.Message}");
                throw;
            }
        }

        // =====================================================================
        //  TEST VŠECH DŮLEŽITÝCH TABULEK
        // =====================================================================
        public Dictionary<string, object> TestAllTables()
        {
            Log.Clear();
            var result = new Dictionary<string, object>();
            var tablesInfo = new List<object>();

            try
            {
                Log.Add("=== Test dostupných tabulek ===");

                var main = new MonS3APIDataMain();
                main.LoadDLL(_dllPath, "MonS3ApiLight");
                main.SetDataPath(_dataPath);

                var program = main.GetProgramInstance();
                program.ConnectData();

                string passwordHash = program.TranslatePassword(_password);
                program.Login(passwordHash);

                program.DisconnectData();

                var agenda = main.GetAgendaInstance();
                agenda.SetAgenda(_agendaExt);
                agenda.ConnectData();

                // Testovat důležité tabulky
                string[] importantTables = {
                    "AdresarF", "ObjPrijHl", "ObjPrijPol", "Doklady", "DokladyPol",
                    "Faktury", "FakturyPol", "Objednavky", "Zasoby", "Ceniky"
                };

                foreach (var tableName in importantTables)
                {
                    try
                    {
                        var what = new MonS3APIDataWhatList();
                        what.AddAll();
                        var where = new MonS3APIDataWhereList();

                        var rows = agenda.GetRows(tableName, what, where, null, 0, 1);

                        tablesInfo.Add(new
                        {
                            table = tableName,
                            exists = true,
                            rowCount = rows.Count,
                            accessible = true
                        });

                        Log.Add($"✓ {tableName}: {rows.Count} záznamů");
                    }
                    catch (MonS3APIException ex) when (ex.ExceptionType == MonS3APIExceptionType.TableNotFound)
                    {
                        tablesInfo.Add(new
                        {
                            table = tableName,
                            exists = false,
                            error = "Tabulka nenalezena"
                        });

                        Log.Add($"✗ {tableName}: nenalezena");
                    }
                    catch (Exception ex)
                    {
                        tablesInfo.Add(new
                        {
                            table = tableName,
                            exists = false,
                            error = ex.Message
                        });

                        Log.Add($"✗ {tableName}: {ex.Message}");
                    }
                }

                agenda.DisconnectData();
                main.UnLoadDLL();

                result["success"] = true;
                result["tables"] = tablesInfo;

                return result;
            }
            catch (Exception ex)
            {
                Log.Add($"Chyba: {ex.Message}");

                result["success"] = false;
                result["error"] = ex.Message;

                return result;
            }
        }

        // =====================================================================
        //  POMOCNÉ METODY PRO DALŠÍ ENDPOINTY
        // =====================================================================

        public Dictionary<string, object> AnalyzeOrdersStructure(int year)
        {
            Log.Clear();
            var result = new Dictionary<string, object>();

            try
            {
                Log.Add($"=== Analýza struktury objednávek pro rok {year} ===");

                var orders = LoadOrdersForYear(year);

                if (orders.Count > 0)
                {
                    var firstOrder = orders[0];
                    result["sampleOrder"] = firstOrder;
                    result["columns"] = firstOrder.Keys.ToList();
                    result["orderCount"] = orders.Count;
                }
                else
                {
                    result["message"] = "Žádné objednávky pro analýzu";
                }

                result["success"] = true;
                return result;
            }
            catch (Exception ex)
            {
                Log.Add($"Chyba: {ex.Message}");

                result["success"] = false;
                result["error"] = ex.Message;
                return result;
            }
        }

        public Dictionary<string, object> TestOrderFilters(int year)
        {
            Log.Clear();
            var result = new Dictionary<string, object>();

            try
            {
                Log.Add($"=== Test filtrů objednávek pro rok {year} ===");

                // Zatím jen základní info
                var years = LoadYears();
                var currentYear = years.FirstOrDefault(y => (int)y["Year"] == year);

                result["year"] = year;
                result["yearInfo"] = currentYear;
                result["availableFilters"] = new List<string>
                {
                    "Druh = 'R' (příjemky)",
                    "Vyrizeno je prázdné",
                    "Datum v rozsahu roku"
                };
                result["note"] = "Filtry budou implementovány v hlavní službě";
                result["success"] = true;

                return result;
            }
            catch (Exception ex)
            {
                Log.Add($"Chyba: {ex.Message}");

                result["success"] = false;
                result["error"] = ex.Message;
                return result;
            }
        }

        public Dictionary<string, object> TestTableAccess(int year, string tableName)
        {
            Log.Clear();
            var result = new Dictionary<string, object>();

            try
            {
                Log.Add($"=== Test přístupu k tabulce {tableName} v roce {year} ===");

                var main = new MonS3APIDataMain();
                main.LoadDLL(_dllPath, "MonS3ApiLight");
                main.SetDataPath(_dataPath);

                var program = main.GetProgramInstance();
                program.ConnectData();

                string passwordHash = program.TranslatePassword(_password);
                program.Login(passwordHash);

                program.DisconnectData();

                var agenda = main.GetAgendaInstance();
                agenda.SetAgenda(_agendaExt);
                agenda.ConnectData();

                try
                {
                    var what = new MonS3APIDataWhatList();
                    what.AddAll();
                    var where = new MonS3APIDataWhereList();

                    var rows = agenda.GetRows(tableName, what, where, null, 0, 5); // Max 5 záznamů

                    var sampleData = new List<Dictionary<string, object>>();
                    int count = 0;

                    while (rows.Next() && count < 3)
                    {
                        count++;
                        var entry = new Dictionary<string, object>();

                        // Zkusíme načíst prvních 10 sloupců
                        for (int i = 1; i <= 10; i++)
                        {
                            try
                            {
                                var colName = $"Column{i}";
                                var value = rows.GetColByName(colName);
                                if (value?.AsString != "")
                                {
                                    entry[colName] = value?.AsString ?? "";
                                }
                            }
                            catch
                            {
                                break;
                            }
                        }

                        sampleData.Add(entry);
                    }

                    result["table"] = tableName;
                    result["year"] = year;
                    result["exists"] = true;
                    result["totalRows"] = rows.Count;
                    result["sampleCount"] = count;
                    result["sampleData"] = sampleData;
                    result["success"] = true;
                }
                catch (MonS3APIException ex) when (ex.ExceptionType == MonS3APIExceptionType.TableNotFound)
                {
                    result["table"] = tableName;
                    result["exists"] = false;
                    result["error"] = "Tabulka nenalezena";
                    result["success"] = false;
                }

                agenda.DisconnectData();
                main.UnLoadDLL();

                return result;
            }
            catch (Exception ex)
            {
                Log.Add($"Chyba: {ex.Message}");

                result["success"] = false;
                result["error"] = ex.Message;
                return result;
            }
        }

        public List<string> GetAvailableTables(int year)
        {
            Log.Clear();
            var tables = new List<string>();

            try
            {
                Log.Add($"=== Zjišťování dostupných tabulek pro rok {year} ===");

                // Pro jednoduchost vracíme pevný seznam
                tables.AddRange(new[]
                {
                    "AdresarF",
                    "ObjPrijHl",
                    "ObjPrijPol",
                    "Doklady",
                    "DokladyPol",
                    "Faktury",
                    "FakturyPol",
                    "Zasoby",
                    "Ceniky"
                });

                Log.Add($"Nalezeno {tables.Count} tabulek");
                return tables;
            }
            catch (Exception ex)
            {
                Log.Add($"Chyba: {ex.Message}");
                return tables;
            }
        }

        public Dictionary<string, object> RunPerformanceTest()
        {
            Log.Clear();
            var result = new Dictionary<string, object>();

            try
            {
                Log.Add("=== Performance test ===");

                var startTime = DateTime.Now;

                // 1. Test načítání roků
                var yearsStart = DateTime.Now;
                var years = LoadYears();
                var yearsTime = DateTime.Now - yearsStart;

                // 2. Test načítání adres
                var addressesStart = DateTime.Now;
                var addresses = LoadAddresses();
                var addressesTime = DateTime.Now - addressesStart;

                // 3. Test načítání objednávek
                var ordersStart = DateTime.Now;
                var orders = LoadOrders();
                var ordersTime = DateTime.Now - ordersStart;

                var totalTime = DateTime.Now - startTime;

                result["performance"] = new
                {
                    years = new
                    {
                        count = years.Count,
                        timeMs = yearsTime.TotalMilliseconds,
                        timePerItem = years.Count > 0 ? yearsTime.TotalMilliseconds / years.Count : 0
                    },
                    addresses = new
                    {
                        count = addresses.Count,
                        timeMs = addressesTime.TotalMilliseconds,
                        timePerItem = addresses.Count > 0 ? addressesTime.TotalMilliseconds / addresses.Count : 0
                    },
                    orders = new
                    {
                        count = orders.Count,
                        timeMs = ordersTime.TotalMilliseconds,
                        timePerItem = orders.Count > 0 ? ordersTime.TotalMilliseconds / orders.Count : 0
                    },
                    totalTimeMs = totalTime.TotalMilliseconds
                };

                result["success"] = true;
                Log.Add($"Performance test dokončen za {totalTime.TotalMilliseconds}ms");

                return result;
            }
            catch (Exception ex)
            {
                Log.Add($"Chyba: {ex.Message}");

                result["success"] = false;
                result["error"] = ex.Message;
                return result;
            }
        }
    }
}