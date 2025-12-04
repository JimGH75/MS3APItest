using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MonS3API
{
    /// <summary>
    /// Typ chyby při práci s MonS3API.
    /// </summary>
    public enum MonS3APIExceptionType
    {
        /// <summary>
        /// Neznámá chyba.
        /// </summary>
        Unknown = 1,
        /// <summary>
        /// Neznámá funkce.
        /// </summary>
        UnknownFunction = 2,
        /// <summary>
        /// Neošetřená chyba databáze.
        /// </summary>
        UnknownDBError = 3,
        /// <summary>
        /// Soubor neexistuje.
        /// </summary>
        FileNotFound = 4,
        /// <summary>
        /// Název aplikace nebyl vyplněn nebo obsahuje neplatné znaky.
        /// </summary>
        AppName = 5,
        /// <summary>
        /// Nelze načíst DLL knihovnu.
        /// </summary>
        LoadDll = 6,
        /// <summary>
        /// Knihovna DLL nebyla načtena.
        /// </summary>
        DLLNotLoaded = 7,
        /// <summary>
        /// Cesta k datům nebyla vyplněna nebo není platná.
        /// </summary>
        DataPath = 8,
        /// <summary>
        /// Nejprve je potřeba se přihlásit k datům.
        /// </summary>
        NotConnected = 9,
        /// <summary>
        /// Existuje otevřené připojení k datům.
        /// </summary>
        AlreadyConnected = 10,
        /// <summary>
        /// Je potřeba aktualizovat klienta na novější verzi.
        /// </summary>
        OldClient = 11,
        /// <summary>
        /// Je potřeba aktualizovat program MoneyS3.
        /// </summary>
        OldAPI = 12,
        /// <summary>
        /// Otevíraná databáze vyžaduje ke svému zobrazení novější verzi MonS3API.
        /// </summary>
        DBNewer = 13,
        /// <summary>
        /// Otevíraná databáze vyžaduje ke svému zobrazení starší verzi MonS3API.
        /// </summary>
        DBElder = 14,
        /// <summary>
        /// Uživatel nebyl přihlášen.
        /// </summary>
        NotLogged = 15,
        /// <summary>
        /// Nelze použít Hash SHA256.
        /// </summary>
        SHA256 = 16,
        /// <summary>
        /// Uživatel je již přihlášen.
        /// </summary>
        AlreadyLogged = 17,
        /// <summary>
        /// Agenda nebyla vybrána.
        /// </summary>
        AgendaNotSelect = 18,
        /// <summary>
        /// Transakce nebyla spuštěna.
        /// </summary>
        NoTransaction = 19,
        /// <summary>
        /// Transakce je již spuštěna.
        /// </summary>
        TransactionAlreadyExists = 20,
        /// <summary>
        /// Vypršel časový timeout.
        /// </summary>
        Timeout = 21,
        /// <summary>
        /// Tabulka nebyla nalezena.
        /// </summary>
        TableNotFound = 22,
        /// <summary>
        /// Sloupec tabulky nebyl nalezen.
        /// </summary>
        ColNotFound = 23,
        /// <summary>
        /// Pro požadovanou operaci nemáte potřebné oprávnění.
        /// </summary>
        NoRights = 24,
        /// <summary>
        /// Pro funkci GetRows je potřeba naplnit list what.
        /// </summary>
        SelectWhat = 25,
        /// <summary>
        /// Funkce DeleteRows vyžaduje podmínku. Pro smazání všeho použijte DeleteAll.
        /// </summary>
        DeleteAll = 26,
        /// <summary>
        /// Specifická chyba pro BFBase .dat tabulky. Pokud je "zamknut" záznam, tak nelze volat UpdateRows nebo DeleteRows.
        /// (Například pokud má jiný uživatel otevřenou fakturu, tak pokud se zavolá úprava nebo smazání té faktury.)
        /// SQLko tento problém nemá a lze "uzamčené" záznamy editovat.
        /// </summary>
        BFWriteLock = 27
    }

    /// <summary>
    /// Chyba při práci s MonS3API. Message obsahuje čitelný překlad typu ExceptionType.
    /// </summary>
    public class MonS3APIException : Exception
    {
        /// <summary>
        /// Typ chyby při práci s MonS3API. Message obsahuje čitelný překlad tohoto typu.
        /// </summary>
        public MonS3APIExceptionType ExceptionType;
        /// <summary>
        /// Detailní informace o chybě. Nemusí být vyplněno.
        /// </summary>
        public string Detail;
        /// <summary>
        /// Text chyby (také jako hodnota Message) - překlad typu ExceptionType.
        /// </summary>
        public string MessageStr;
        /// <summary>
        /// Eventa na změnu textů chyby.
        /// </summary>
        public static event EventHandler OnGetMessage;

        /// <summary>
        /// Vytvoření chyby při práci s MonS3API.
        /// </summary>
        /// <param name="ExceptionType">Typ chyby.</param>
        /// <param name="Detail">Detailní informace o chybě. Nemusí být vyplněno.</param>
        public MonS3APIException(MonS3APIExceptionType ExceptionType, string Detail)
        {
            this.ExceptionType = ExceptionType;
            this.Detail = Detail;

            switch (ExceptionType)
            {
                case MonS3APIExceptionType.Unknown: MessageStr = "Neznámá chyba."; break;
                case MonS3APIExceptionType.UnknownFunction: MessageStr = "Neznámá funkce."; break;
                case MonS3APIExceptionType.UnknownDBError: MessageStr = "Neošetřená chyba databáze."; break;
                case MonS3APIExceptionType.FileNotFound: MessageStr = "Soubor neexistuje."; break;
                case MonS3APIExceptionType.AppName: MessageStr = "Název aplikace nebyl vyplněn nebo obsahuje neplatné znaky."; break;
                case MonS3APIExceptionType.LoadDll: MessageStr = "Nelze načíst DLL knihovnu."; break;
                case MonS3APIExceptionType.DLLNotLoaded: MessageStr = "Knihovna DLL nebyla načtena."; break;               
                case MonS3APIExceptionType.DataPath: MessageStr = "Cesta k datům nebyla vyplněna nebo není platná."; break;
                case MonS3APIExceptionType.NotConnected: MessageStr = "Nejprve je potřeba se přihlásit k datům."; break;
                case MonS3APIExceptionType.AlreadyConnected: MessageStr = "Existuje otevřené připojení k datům."; break;
                case MonS3APIExceptionType.OldClient: MessageStr = "Je potřeba aktualizovat klienta na novější verzi."; break;
                case MonS3APIExceptionType.OldAPI: MessageStr = "Je potřeba aktualizovat program MoneyS3."; break;
                case MonS3APIExceptionType.DBNewer: MessageStr = "Otevíraná databáze vyžaduje ke svému zobrazení novější verzi MonS3API."; break;
                case MonS3APIExceptionType.DBElder: MessageStr = "Otevíraná databáze vyžaduje ke svému zobrazení starší verzi MonS3API."; break;
                case MonS3APIExceptionType.NotLogged: MessageStr = "Uživatel nebyl přihlášen."; break;
                case MonS3APIExceptionType.SHA256: MessageStr = "Nelze použít Hash SHA256."; break;
                case MonS3APIExceptionType.AlreadyLogged: MessageStr = "Uživatel je již přihlášen."; break;
                case MonS3APIExceptionType.AgendaNotSelect: MessageStr = "Agenda nebyla vybrána."; break;
                case MonS3APIExceptionType.NoTransaction: MessageStr = "Transakce nebyla spuštěna."; break;
                case MonS3APIExceptionType.TransactionAlreadyExists: MessageStr = "Transakce je již spuštěna."; break;
                case MonS3APIExceptionType.Timeout: MessageStr = "Vypršel časový timeout."; break;
                case MonS3APIExceptionType.TableNotFound: MessageStr = "Tabulka nebyla nalezena."; break;
                case MonS3APIExceptionType.ColNotFound: MessageStr = "Sloupec tabulky nebyl nalezen."; break;
                case MonS3APIExceptionType.NoRights: MessageStr = "Pro požadovanou operaci nemáte potřebné oprávnění."; break;
                case MonS3APIExceptionType.SelectWhat: MessageStr = "Pro funkci GetRows je potřeba naplnit list what."; break;
                case MonS3APIExceptionType.DeleteAll: MessageStr = "Funkce DeleteRows vyžaduje podmínku. Pro smazání všeho použijte DeleteAll."; break;
                case MonS3APIExceptionType.BFWriteLock: MessageStr = "Nelze zapsat do datového souboru, protože je zamknut uživatelským zámkem."; break;
            }

            if (OnGetMessage != null)
            {
                OnGetMessage.Invoke(this, null);
            }
        }

        /// <summary>
        /// Text chyby - překlad typu ExceptionType.
        /// </summary>
        public override string Message
        {
            get
            {
                return MessageStr;
            }
        }
    }

    /// <summary>
    /// Informace o přihlášení k datům.
    /// </summary>
    public enum MonS3APILoginInformation
    {
        /// <summary>
        /// Neexistuje žádný uživatel a nebo existuje uživatel, ale nemá vyplněné heslo.
        /// </summary>
        WithoutPassword = 1,
        /// <summary>
        /// Lze se přihlásit pouze pomocí windows jména, ale to tento uživatel nesplňuje.
        /// </summary>
        WindowsNO = 2,
        /// <summary>
        /// Lze se přihlásit jen pomocí windows jména a to tento uživatel splňuje.
        /// </summary>
        WindowsYES = 3,
        /// <summary>
        /// Lze se přihlásit pomocí windows jména a nebo na jiného uživatele pomocí hesla.
        /// </summary>
        WindowsPassword = 4,
        /// <summary>
        /// Lze se přihlásit pouze pomocí hesla.
        /// </summary>
        Password = 5
    }

    /// <summary>
    /// Hlavní třída pro práci s daty Money S3.
    /// </summary>
    public class MonS3APIDataMain
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        [DllImport("Kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>
        /// Funkce na komunikaci s 32bit dll knihovnou.
        /// </summary>
        /// <param name="input">Odkaz na vstupní data.</param>
        /// <returns>Odkaz na výstupní data</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 DLLFunc32Type(UInt32 input);
        /// <summary>
        /// Funkce na komunikaci s 64bit dll knihovnou.
        /// </summary>
        /// <param name="input">Odkaz na vstupní data.</param>
        /// <returns>Odkaz na výstupní data</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt64 DLLFunc64Type(UInt64 input);

        /// <summary>
        /// Funkce na komunikaci s 32bit dll knihovnou - uvolnění paměti.
        /// </summary>
        /// <param name="input">Odkaz na data.</param>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void DLLFunc32FreeType(UInt32 input);
        /// <summary>
        /// Funkce na komunikaci s 64bit dll knihovnou - uvolnění paměti.
        /// </summary>
        /// <param name="input">Odkaz na vstupní data.</param>
        /// <returns>Odkaz na výstupní data</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void DLLFunc64FreeType(UInt64 input);

        /// <summary>
        /// Funkce na komunikaci s 32bit dll knihovnou.
        /// </summary>
        private DLLFunc32Type DLLFunc32;
        /// <summary>
        /// Funkce na komunikaci s 64bit dll knihovnou.
        /// </summary>
        private DLLFunc64Type DLLFunc64;

        /// <summary>
        /// Funkce na komunikaci s 32bit dll knihovnou - uvolnění paměti.
        /// </summary>
        private DLLFunc32FreeType DLLFunc32Free;
        /// <summary>
        /// Funkce na komunikaci s 64bit dll knihovnou - uvolnění paměti.
        /// </summary>
        private DLLFunc64FreeType DLLFunc64Free;

        /// <summary>
        /// Handle knihovny.
        /// </summary>
        private IntPtr HandleDLL;
        /// <summary>
        /// Identifikátor tohoto klienta.
        /// </summary>
        private byte[] ClientGUID;

        /// <summary>
        /// Typ knihovny.
        /// </summary>
        public enum MonS3APIDLLType
        {
            /// <summary>
            /// Základní knihovna.
            /// </summary>
            MonS3API = 1,
            /// <summary>
            /// Knihovna jen pro čtení dat.
            /// </summary>
            MonS3Reader = 2
        }

        /// <summary>
        /// Hledá knihovnu MonS3API.dll
        /// </summary>
        /// <returns>Vrací celou cestu k MonS3API.dll a nebo prázdný string.</returns>
        public string FindDLL(MonS3APIDLLType DLLType = MonS3APIDLLType.MonS3API)
        {
            string res = "";

            if (string.IsNullOrEmpty(res))
            {
                res = FindDLLCheckCommon(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), DLLType);
            }

            if (string.IsNullOrEmpty(res) && Environment.Is64BitOperatingSystem)
            {
                res = FindDLLCheckCommon(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), DLLType);
            }

            if (string.IsNullOrEmpty(res))
            {
                res = FindDLLCheckProgram(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), DLLType);
            }

            if (string.IsNullOrEmpty(res) && Environment.Is64BitOperatingSystem)
            {
                res = FindDLLCheckProgram(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), DLLType);
            }

            return res;
        }

        /// <summary>
        /// Hledá knihovnu MonS3API.dll ve složce Common Files.
        /// </summary>
        /// <param name="DirPath">Celá cesta ke složce Common Files.</param>
        /// <returns>Vrací celou cestu k MonS3API.dll a nebo prázdný string.</returns>
        private string FindDLLCheckCommon(string DirPath, MonS3APIDLLType DLLType = MonS3APIDLLType.MonS3API)
        {
            string res = "";

            if (!string.IsNullOrEmpty(DirPath))
            {
                if (string.IsNullOrEmpty(res))
                {
                    res = FindDLLCheckCommon2(Path.Combine(DirPath, "Solitea"), DLLType);
                }

                if (string.IsNullOrEmpty(res))
                {
                    res = FindDLLCheckCommon2(Path.Combine(DirPath, "CIGLER SOFTWARE"), DLLType);
                }
            }

            return res;
        }

        /// <summary>
        /// Hledá knihovnu MonS3API.dll ve složce Solitea nebo CIGLER SOFTWARE v Common Files.
        /// </summary>
        /// <param name="DirPath">Cesta ke složce Solitea nebo CIGLER SOFTWARE v Common Files.</param>
        /// <returns>Vrací celou cestu k MonS3API.dll a nebo prázdný string.</returns>
        private string FindDLLCheckCommon2(string DirPath, MonS3APIDLLType DLLType = MonS3APIDLLType.MonS3API)
        {
            string res = "";

            if (!string.IsNullOrEmpty(DirPath))
            {
                if (string.IsNullOrEmpty(res))
                {
                    res = FindDLLCheckProgram2(DirPath, DLLType);
                }

                if (string.IsNullOrEmpty(res))
                {
                    res = FindDLLCheckCommon3(Path.Combine(DirPath, "Money S3", "setup.ini"), DLLType);
                }

                if (string.IsNullOrEmpty(res))
                {
                    res = FindDLLCheckCommon3(Path.Combine(DirPath, "Money S3", "start.ini"), DLLType);
                }
            }

            return res;
        }

        /// <summary>
        /// Hledá knihovnu MonS3API.dll - cesta dle ini souboru.
        /// </summary>
        /// <param name="IniFile">Celá cesta k ini souboru.</param>
        /// <returns>Vrací celou cestu k MonS3API.dll a nebo prázdný string.</returns>
        private string FindDLLCheckCommon3(string IniFile, MonS3APIDLLType DLLType = MonS3APIDLLType.MonS3API)
        {
            if (string.IsNullOrEmpty(IniFile) || !File.Exists(IniFile))
            {
                return "";
            }

            StringBuilder sb = new StringBuilder(65535);
            GetPrivateProfileString("InstParams", "LocalPath", "", sb, 65535, IniFile);

            return FindDLLCheckProgram2(sb.ToString(), DLLType);
        }

        /// <summary>
        /// Hledá knihovnu MonS3API.dll ve složce Program Files.
        /// </summary>
        /// <param name="DirPath">Celá cesta ke složce Program Files.</param>
        /// <returns>Vrací celou cestu k MonS3API.dll a nebo prázdný string.</returns>
        private string FindDLLCheckProgram(string DirPath, MonS3APIDLLType DLLType = MonS3APIDLLType.MonS3API)
        {
            string res = "";

            if (!string.IsNullOrEmpty(DirPath))
            {
                if (string.IsNullOrEmpty(res))
                {
                    res = FindDLLCheckProgram2(Path.Combine(DirPath, @"Solitea\Money S3"), DLLType);
                }

                if (string.IsNullOrEmpty(res))
                {
                    res = FindDLLCheckProgram2(Path.Combine(DirPath, @"CIGLER SOFTWARE\Money S3"), DLLType);
                }
            }

            return res;
        }

        /// <summary>
        /// Hledá knihovnu MonS3API.dll ve složce Solitea nebo CIGLER SOFTWARE v Program Files.
        /// </summary>
        /// <param name="DirPath">Celá cesta ke složce Solitea nebo CIGLER SOFTWARE v Program Files.</param>
        /// <returns>Vrací celou cestu k MonS3API.dll a nebo prázdný string.</returns>
        private string FindDLLCheckProgram2(string DirPath, MonS3APIDLLType DLLType = MonS3APIDLLType.MonS3API)
        {
            if (!string.IsNullOrEmpty(DirPath))
            {
                string path;
                if (DLLType == MonS3APIDLLType.MonS3API)
                {
                    if (Environment.Is64BitProcess)
                    {
                        path = Path.Combine(DirPath, "MonS3API_64.dll");
                    }
                    else
                    {
                        path = Path.Combine(DirPath, "MonS3API_32.dll");
                    }
                }
                else
                {
                    if (Environment.Is64BitProcess)
                    {
                        path = Path.Combine(DirPath, "MonS3Reader_64.dll");
                    }
                    else
                    {
                        path = Path.Combine(DirPath, "MonS3Reader_32.dll");
                    }
                }

                if (File.Exists(path))
                {
                    return path;
                }
                
            }

            return "";
        }

        /// <summary>
        /// Připojí se ke knihovně MonS3API.dll.
        /// </summary>
        /// <param name="FullFileName">Celá cesta k souboru MonS3API.dll</param>
        /// <param name="AppName">Název aplikace používající tuto funkčnost.</param>
        public void LoadDLL(string FullFileName, string AppName, MonS3APIDLLType DLLType = MonS3APIDLLType.MonS3API)
        {
            UnLoadDLL();

            if (string.IsNullOrWhiteSpace(FullFileName) || !File.Exists(FullFileName))
            {
                throw new MonS3APIException(MonS3APIExceptionType.FileNotFound, FullFileName);
            }

            HandleDLL = LoadLibrary(FullFileName);
            if (HandleDLL == IntPtr.Zero)
            {
                throw new MonS3APIException(MonS3APIExceptionType.LoadDll, FullFileName);
            }

            try
            {
                IntPtr functionAddress = GetProcAddress(HandleDLL, "DLLFunc");
                IntPtr functionAddressFree = GetProcAddress(HandleDLL, "DLLFuncFree");
                if (functionAddress == IntPtr.Zero || functionAddressFree == IntPtr.Zero)
                {
                    throw new MonS3APIException(MonS3APIExceptionType.LoadDll, FullFileName);
                }

                try
                {
                    if (Environment.Is64BitProcess)
                    {
                        DLLFunc64 = (DLLFunc64Type)Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(DLLFunc64Type));
                        DLLFunc64Free = (DLLFunc64FreeType)Marshal.GetDelegateForFunctionPointer(functionAddressFree, typeof(DLLFunc64FreeType));
                    }
                    else
                    {
                        DLLFunc32 = (DLLFunc32Type)Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(DLLFunc32Type));
                        DLLFunc32Free = (DLLFunc32FreeType)Marshal.GetDelegateForFunctionPointer(functionAddressFree, typeof(DLLFunc32FreeType));
                    }
                }
                catch
                {
                    throw new MonS3APIException(MonS3APIExceptionType.LoadDll, FullFileName);
                }

                ClientGUID = null;

                _MonS3APIParser parser = _CreateParser("LoadDLL");
                //Verze MonS3API!
                parser.AddCardinal(1);
                parser.AddStr(AppName);
                parser.AddStr(FullFileName);
                parser.AddStr(FindDLL(DLLType));
                parser.Call();

                ClientGUID = parser.GetData();
            }
            catch
            {
                UnLoadDLL();
                throw;
            }
        }

        /// <summary>
        /// Odpojí se od knihovny MonS3API.dll.
        /// </summary>
        public void UnLoadDLL()
        {
            if (HandleDLL == IntPtr.Zero)
            {
                return;
            }

            if (ClientGUID != null)
            {
                _MonS3APIParser parser = _CreateParser("UnLoadDLL", true);
                parser.Call();
            }

            FreeLibrary(HandleDLL);
            HandleDLL = IntPtr.Zero;
            DLLFunc32 = null;
            DLLFunc32Free = null;
            DLLFunc64 = null;
            DLLFunc64Free = null;
            ClientGUID = null;
        }

        ~MonS3APIDataMain()
        {
            UnLoadDLL();
        }

        /// <summary>
        /// Hledá složku s daty Money S3.
        /// </summary>
        /// <returns>Vrací celou cestu k adresáři Data a nebo prázdný string.</returns>
        public string FindData()
        {
            _MonS3APIParser parser = _CreateParser("FindData");
            parser.Call();
            return parser.GetStr();
        }

        /// <summary>
        /// Nastaví cestu k datům.
        /// </summary>
        /// <param name="FullPath">Celá cesta ke složce Data.</param>
        public void SetDataPath(string FullPath)
        {
            _MonS3APIParser parser = _CreateParser("SetDataPath");
            parser.AddStr(FullPath);
            parser.Call();
        }

        /// <summary>
        /// Získá instanci pro práci se společnými daty. Jedna instance smí být použita současně pouze v jednom vlákně.
        /// </summary>
        /// <returns>Instance pro práci se společnými daty.</returns>
        public MonS3APIDataProgram GetProgramInstance()
        {
            _MonS3APIParser parser = _CreateParser("GetProgramInstance");
            parser.Call();

            byte[] objectGUID = parser.GetData();

            return new MonS3APIDataProgram(this, objectGUID);
        }

        /// <summary>
        /// Získá instanci pro práci s daty agendy. Jedna instance smí být použita současně pouze v jednom vlákně a pro jednu agendu.
        /// </summary>
        /// <returns>Instance pro práci s daty agendy.</returns>
        public MonS3APIDataAgenda GetAgendaInstance()
        {
            _MonS3APIParser parser = _CreateParser("GetAgendaInstance");
            parser.Call();

            byte[] objectGUID = parser.GetData();

            return new MonS3APIDataAgenda(this, objectGUID);
        }

        /// <summary>
        /// Získá instanci pro práci s dokumenty.s3db. Jedna instance smí být použita současně pouze v jednom vlákně
        /// </summary>
        /// <returns>Instance pro práci s daty agendy.</returns>
        public MonS3APIDataDokumenty GetDokumentyInstance()
        {
            _MonS3APIParser parser = _CreateParser("GetDokumentyInstance");
            parser.Call();

            byte[] objectGUID = parser.GetData();

            return new MonS3APIDataDokumenty(this, objectGUID);
        }

        /// <summary>
        /// Vytvoří komunikaci s DLL knihovnou.
        /// </summary>
        /// <returns>Komunikační třída s DLL knihovnou.</returns>
        private _MonS3APIParser _CreateParser(string func, bool ignoreErrors = false)
        {
            return _CreateParser(func, null, ignoreErrors);
        }

        /// <summary>
        /// Vytvoří komunikaci s DLL knihovnou pro objekt.
        /// </summary>
        /// <returns>Komunikační třída s DLL knihovnou.</returns>
        public _MonS3APIParser _CreateParser(string func, byte[] objectGUID, bool ignoreErrors = false)
        {
            if (HandleDLL == IntPtr.Zero)
            {
                if (!ignoreErrors)
                {
                    throw new MonS3APIException(MonS3APIExceptionType.DLLNotLoaded, "");
                }
            }

            return new _MonS3APIParser(this, ClientGUID, objectGUID, func, ignoreErrors);
        }

        /// <summary>
        /// Vnitřní funkce pro komunikaci s DLL knihovnou.
        /// </summary>
        /// <param name="inp"></param>
        /// <returns></returns>
        public IntPtr _Call(IntPtr inp)
        {
            if (Environment.Is64BitProcess)
            {
                return (IntPtr)(Int64)DLLFunc64((UInt64)inp.ToInt64());
            }
            else
            {
                return (IntPtr)(Int32)DLLFunc32((UInt32)inp.ToInt32());
            }
        }

        /// <summary>
        /// Vnitřní funkce pro komunikaci s DLL knihovnou - uvolnění paměti.
        /// </summary>
        /// <param name="inp"></param>
        public void _CallFree(IntPtr inp)
        {
            if (Environment.Is64BitProcess)
            {
                DLLFunc64Free((UInt64)inp.ToInt64());
            }
            else
            {
                DLLFunc32Free((UInt32)inp.ToInt32());
            }
        }
    }

    /// <summary>
    /// Třída pro práci se společnými daty.
    /// </summary>
    public class MonS3APIDataProgram : MonS3APIDataBase
    {
        /// <summary>
        /// Vytvoření připojení ke společným datům.
        /// </summary>
        /// <param name="parent">Hlavní třída obsluhující DLL knihovnu.</param>
        /// <param name="objectGUID">GUID tohoto objektu.</param>
        public MonS3APIDataProgram(MonS3APIDataMain parent, byte[] objectGUID) : base(parent, objectGUID)
        {
        }

        /// <summary>
        /// Vrátí informaci o možnostech přihlášení uživatele.
        /// </summary>
        /// <returns>Možnosti přihlášení uživatele.</returns>
        public MonS3APILoginInformation GetLoginInformation()
        {
            _MonS3APIParser parser = _CreateParser("GetLoginInformation");
            parser.Call();

            return (MonS3APILoginInformation)parser.GetByte();
        }

        /// <summary>
        /// Zobrazí logovací dialog pro uživatele.
        /// </summary>
        /// <param name="parentForm">Okno, které bude parentem k vytvořenému dilogu. Nemusí být vyplněno, ale pak se to nebude chovat zcela modálně.</param>
        /// <returns>Vrací hash hesla uživatele.</returns>
        public string LoginDialog(IntPtr parentForm)
        {
            _MonS3APIParser parser = _CreateParser("LoginDialog");

            if (Environment.Is64BitProcess)
            {
                parser.AddInt64(parentForm.ToInt64());
            }
            else
            {
                parser.AddInt(parentForm.ToInt32());
            }

            parser.Call();

            string res = parser.GetStr();

            return res;
        }

        /// <summary>
        /// Přeloží heslo na hash použitelný v dalších funkcích.
        /// </summary>
        /// <param name="Password">Vstupní heslo.</param>
        /// <returns>Vrací hash.</returns>
        public string TranslatePassword(string Password)
        {
            _MonS3APIParser parser = _CreateParser("TranslatePassword");
            parser.AddStr(Password);
            parser.Call();

            string res = parser.GetStr();

            return res;
        }

        /// <summary>
        /// Přihlásí se do dat.
        /// 
        /// Heslo se nevyplňuje v případech:
        ///   WithoutPassword
        ///   WindowsYES
        ///   
        /// Heslo se nemusí vyplňovat v případech:
        ///   WindowsPassword
        ///   
        /// Heslo se musí vyplňovat v případech:
        ///   Password
        ///   
        /// Nelze se přihlásit:
        ///   WindowsNO
        /// 
        /// </summary>
        /// <param name="Password">Hash hesla.</param>
        /// <returns>Vrací, zda se povedlo přihlásit.</returns>
        public bool Login(string Password)
        {
            _MonS3APIParser parser = _CreateParser("Login");
            parser.AddStr(Password);
            parser.Call();

            bool res = parser.GetBool();

            return res;
        }

        /// <summary>
        /// Odhlásí uživatele.
        /// </summary>
        public void Logout()
        {
            _MonS3APIParser parser = _CreateParser("Logout");
            parser.Call();
        }

        /// <summary>
        /// Vrátí informace o přihlášeném uživateli.
        /// </summary>
        /// <returns></returns>
        public MonS3APIUserInfo GetUserInfo()
        {
            _MonS3APIParser parser = _CreateParser("GetUserInfo");
            parser.Call();

            string guid = parser.GetStr();
            int id = parser.GetInt();
            string jmeno = parser.GetStr();
            string configName = parser.GetStr();
            string poznamka = parser.GetStr();

            return new MonS3APIUserInfo(guid, id, jmeno, configName, poznamka);
        }

        /// <summary>
        /// Vrátí seznam práv uživatele.
        /// </summary>
        /// <returns></returns>
        public MonS3APIUserRightList GetUserRights()
        {
            _MonS3APIParser parser = _CreateParser("GetUserRights");
            parser.Call();

            UInt32 count = parser.GetArray();
            MonS3APIUserRightList list = new MonS3APIUserRightList();
            list.Capacity = (int)count;

            for (int i = 0; i < count; i++)
            {
                string rightName = parser.GetStr();
                string rightValue = parser.GetStr();

                MonS3APIUserRight item = new MonS3APIUserRight(rightName, rightValue);
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Vrátí seznam agend.
        /// </summary>
        public MonS3APIAgendaList GetListAgend()
        {
            _MonS3APIParser parser = _CreateParser("GetListAgend");
            parser.Call();

            UInt32 count = parser.GetArray();
            MonS3APIAgendaList list = new MonS3APIAgendaList();
            list.Capacity = (int)count;

            for (int i = 0; i < count; i++)
            {
                MonS3APIAgenda item = _ParseAgendaInfo(parser);
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Vrátí info o konkrétní agendě, bez načtení všech agend.
        /// </summary>
        /// <param name="agendaExt"></param>
        /// <returns></returns>
        public MonS3APIAgenda GetAgendaInfo(string agendaExt)
        {
            _MonS3APIParser parser = _CreateParser("ProgramGetAgendaInfo");
            parser.AddStr(agendaExt);
            parser.Call();

            return _ParseAgendaInfo(parser);
        }
    }

    /// <summary>
    /// Třída pro práci s daty agendy.
    /// </summary>
    public class MonS3APIDataAgenda : MonS3APIDataBase
    {
        /// <summary>
        /// Vytvoření připojení k datům agendy.
        /// </summary>
        /// <param name="parent">Hlavní třída obsluhující DLL knihovnu.</param>
        /// <param name="objectGUID">GUID tohoto objektu.</param>
        public MonS3APIDataAgenda(MonS3APIDataMain parent, byte[] objectGUID) : base(parent, objectGUID)
        {
        }

        /// <summary>
        /// Nastaví agendu k připojení.
        /// </summary>
        /// <param name="agenda">Vybraná agenda.</param>
        public void SetAgenda(string agendaExt)
        {
            _MonS3APIParser parser = _CreateParser("SetAgenda");
            parser.AddStr(agendaExt);
            parser.Call();
        }

        /// <summary>
        /// Vrátí info o aktuálně nastavené agendě.
        /// </summary>
        /// <param name="agendaExt"></param>
        /// <returns></returns>
        public MonS3APIAgenda GetAgendaInfo()
        {
            _MonS3APIParser parser = _CreateParser("AgendaGetAgendaInfo");
            parser.Call();

            return _ParseAgendaInfo(parser);
        }

        /// <summary>
        /// Vrátí všechny roky v agendě seřazené dle data.
        /// </summary>
        /// <returns>Všechny roky v agendě.</returns>
        public MonS3APIYearList GetYearList()
        {
            _MonS3APIParser parser = _CreateParser("GetYearList");
            parser.Call();

            UInt32 count = parser.GetArray();

            MonS3APIYearList list = new MonS3APIYearList();
            list.Capacity = (int)count;

            for (int i = 0; i < count; i++)
            {
                int year = parser.GetWord();
                DateTime dateFrom = parser.GetDate();
                DateTime dateTo = parser.GetDate();
                MonS3APIRight right = (MonS3APIRight)parser.GetByte();
                string ext = parser.GetStr();

                MonS3APIYear item = new MonS3APIYear(year, dateFrom, dateTo, right, ext);
                list.Add(item);
            }

            return list;
        }
    }

    /// <summary>
    /// Třída pro práci s dokumenty.
    /// </summary>
    public class MonS3APIDataDokumenty : MonS3APIDataBase
    {
        /// <summary>
        /// Vytvoření připojení k datům dokumenty.
        /// </summary>
        /// <param name="parent">Hlavní třída obsluhující DLL knihovnu.</param>
        /// <param name="objectGUID">GUID tohoto objektu.</param>
        public MonS3APIDataDokumenty(MonS3APIDataMain parent, byte[] objectGUID) : base(parent, objectGUID)
        {
        }

        /// <summary>
        /// Nastaví agendu k připojení.
        /// </summary>
        /// <param name="agenda">Vybraná agenda.</param>
        public void SetAgenda(string agendaExt)
        {
            _MonS3APIParser parser = _CreateParser("SetAgenda");
            parser.AddStr(agendaExt);
            parser.Call();
        }

    }

    /// <summary>
    /// Třída společných funkcí pro práci se společnými daty a daty agendy.
    /// </summary>
    public class MonS3APIDataBase
    {
        /// <summary>
        /// Hlavní třída obsluhující DLL knihovnu.
        /// </summary>
        private readonly MonS3APIDataMain Parent;
        /// <summary>
        /// GUID tohoto objektu.
        /// </summary>
        private readonly byte[] ObjectGUID;

        /// <summary>
        /// Nastavení:
        /// Pokud je true, tak při nenalezeném sloupečku hodí exception.
        /// Pokud je false (default), tak vrátí prázdnou hodnotu.
        /// </summary>
        public bool SettingErrorIfColNotFound = false;

        /// <summary>
        /// Počet milisekund po které se bude čekat na zamknutou tabulku.
        /// Po vypršení nastane exception.
        /// Default je -1, což je čekání do nekonečna.
        /// </summary>
        public int SettingsLockTimeout
        {
            get
            {
                _MonS3APIParser parser = _CreateParser("GetSettingsLockTimeout");
                parser.Call();
                return parser.GetInt();
            }
            set
            {
                _MonS3APIParser parser = _CreateParser("SetSettingsLockTimeout");
                parser.AddInt(value);
                parser.Call();
            }
        }

        /// <summary>
        /// Vytvoření připojení ke společným datům a nebo datům agendy.
        /// </summary>
        /// <param name="parent">Hlavní třída obsluhující DLL knihovnu.</param>
        /// <param name="objectGUID">GUID tohoto objektu.</param>
        public MonS3APIDataBase(MonS3APIDataMain parent, byte[] objectGUID)
        {
            this.Parent = parent;
            this.ObjectGUID = objectGUID;
        }

        ~MonS3APIDataBase()
        {
            _MonS3APIParser parser = _CreateParser("DestroyObject", true);
            parser.Call();
        }

        /// <summary>
        /// Připojí se k datům Money S3.
        /// Pokud není nějakou dobu potřeba přistupovat k datům, bylo by dobré se od nich opět odpojit.
        /// </summary>      
        public void ConnectData()
        {
            _MonS3APIParser parser = _CreateParser("ConnectData");
            parser.Call();
        }

        /// <summary>
        /// Odpojí se od dat.
        /// </summary>
        public void DisconnectData()
        {
            _MonS3APIParser parser = _CreateParser("DisconnectData");
            parser.Call();
        }

        /// <summary>
        /// Vrací seznam všech tabulek.
        /// </summary>
        /// <returns></returns>
        public MonS3APITableList GetTableList()
        {
            _MonS3APIParser parser = _CreateParser("GetTableList");
            parser.Call();

            UInt32 count = parser.GetArray();

            MonS3APITableList list = new MonS3APITableList();
            list.Capacity = (int)count;

            for (int i = 0; i < count; i++)
            {
                string name = parser.GetStr();
                MonS3APIRight right = (MonS3APIRight)parser.GetByte();
                MonS3APIRight partialRightRow = (MonS3APIRight)parser.GetByte();
                MonS3APIRight partialRightCol = (MonS3APIRight)parser.GetByte();

                MonS3APITable item = new MonS3APITable(name, right, partialRightRow, partialRightCol);
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Vrátí seznam všech sloupečků.
        /// </summary>
        /// <param name="TableName">Název tabulky.</param>
        /// <returns></returns>
        public MonS3APITableColumnList GetTableColumns(string TableName)
        {
            _MonS3APIParser parser = _CreateParser("GetTableColumns");
            parser.AddStr(TableName);
            parser.Call();

            UInt32 count = parser.GetArray();

            MonS3APITableColumnList list = new MonS3APITableColumnList();
            list.Capacity = (int)count;

            for (int i = 0; i < count; i++)
            {
                string name = parser.GetStr();
                MonS3APIColumnDataType dataType = (MonS3APIColumnDataType)parser.GetByte();
                int stringSize = (int)parser.GetCardinal();

                MonS3APITableColumn item = new MonS3APITableColumn(name, dataType, stringSize);
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Spustí transakci.
        /// Transakce by měla běžet pouze po nezbytně nutnou dobu.
        /// Nelze použít s knihovnou MonS3Reader.
        /// </summary>
        public void TransactionStart()
        {
            _MonS3APIParser parser = _CreateParser("TransactionStart");
            parser.Call();
        }

        /// <summary>
        /// Potvrdí změny a ukončí transakci.
        /// Pokud není aktivní transakce, tak dojde k chybě.
        /// Nelze použít s knihovnou MonS3Reader.
        /// </summary>
        public void TransactionCommit()
        {
            _MonS3APIParser parser = _CreateParser("TransactionCommit");
            parser.Call();
        }

        /// <summary>
        /// Potvrdí změny a ukončí transakci.
        /// Pokud není aktivní transakce, tak se funkce ignoruje.
        /// Nelze použít s knihovnou MonS3Reader.
        /// </summary>
        public void TransactionRollback()
        {
            _MonS3APIParser parser = _CreateParser("TransactionRollback", true);
            parser.Call();
        }

        /// <summary>
        /// Vrací, zda existuje řádek.
        /// Pokud je podmínka vyplněna, tak zda alespoň jeden řádek splňuje podmínku.
        /// Pokud pomínka není vyplněna, tak zda není tabulka prázdná.
        /// </summary>
        /// <param name="table">Název tabulky.</param>
        /// <param name="where">Podmínka. Může být null a nebo prázdná třída.</param>
        /// <returns>Zda existuje řádek.</returns>
        public bool GetRowExists(string table, MonS3APIDataWhereList where = null)
        {
            _MonS3APIParser parser = _CreateParser("GetRowExists");
            parser.AddStr(table);
            _AddWhere(parser, where);
            parser.Call();

            bool res = parser.GetBool();
            return res;
        }

        /// <summary>
        /// Vrací počet řádků.
        /// Pokud je podmínka vyplněna, tak počet řádků splňující podmínku.
        /// Pokud pomínka není vyplněna, tak počet všech řádků tabulky.
        /// </summary>
        /// <param name="table">Název tabulky.</param>
        /// <param name="where">Podmínka. Může být null a nebo prázdná třída.</param>
        /// <returns>Počet řádků</returns>
        public int GetRowCount(string table, MonS3APIDataWhereList where = null)
        {
            _MonS3APIParser parser = _CreateParser("GetRowCount");
            parser.AddStr(table);
            _AddWhere(parser, where);
            parser.Call();

            int res = (int)parser.GetCardinal();
            return res;
        }

        /// <summary>
        /// Vrací řádky.
        /// Pokud je podmínka vyplněna, tak všechny splňující podmínku.
        /// Pokud pomínka není vyplněna, tak všechny z tabulky.
        /// </summary>
        /// <param name="table">Název tabulky.</param>
        /// <param name="what">Jaké sloupečky se mají číst.</param>
        /// <param name="where">Podmínka. Může být null a nebo prázdná třída.</param>
        /// <param name="sort">Řazení. Může být null a nebo prázdná třída.</param>
        /// <param name="from">Přeskočení řádků.</param>
        /// <param name="limit">Maximální počet získaných řádků, pokud je hodnota -1, tak není limit nastaven.</param>
        /// <returns>Vrátí řádky.</returns>
        public MonS3APIDataSelectResult GetRows(string table, MonS3APIDataWhatList what, MonS3APIDataWhereList where = null, MonS3APIDataSortList sort = null, int from = 0, int limit = -1)
        {
            _MonS3APIParser parser = _CreateParser("GetRows");
            parser.AddStr(table);
            _AddWhat(parser, what);
            _AddWhere(parser, where);
            _AddSort(parser, sort);
            parser.AddInt(from);
            parser.AddInt(limit);
            parser.Call();

            return new MonS3APIDataSelectResult(this, parser);
        }

        /// <summary>
        /// Přidá řádek.
        /// Pro přidání více řádků doporučuji použít transakci.
        /// Nelze použít s knihovnou MonS3Reader.
        /// </summary>
        /// <param name="table">Název tabulky.</param>
        /// <param name="values">Nové hodnoty.</param>
        /// <returns>ID přidaného řádku</returns>
        public int AddRow(string table, MonS3APIDataValueList values)
        {
            _MonS3APIParser parser = _CreateParser("AddRow");
            parser.AddStr(table);
            _AddDataValues(parser, values);
            parser.Call();

            int res = parser.GetInt();
            return res;
        }

        /// <summary>
        /// Upraví řádky.
        /// Pokud je podmínka vyplněna, tak všechny splňující podmínku.
        /// Pokud pomínka není vyplněna, tak všechny v tabulce.
        /// Nelze použít s knihovnou MonS3Reader.
        /// </summary>
        /// <param name="table">Název tabulky.</param>
        /// <param name="values">Nové hodnoty.</param>
        /// <param name="where">Podmínka. Může být null a nebo prázdná třída.</param>
        /// <returns>Počet upravených řádků.</returns>
        public int UpdateRows(string table, MonS3APIDataValueList values, MonS3APIDataWhereList where = null)
        {
            _MonS3APIParser parser = _CreateParser("UpdateRows");
            parser.AddStr(table);
            _AddDataValues(parser, values);
            _AddWhere(parser, where);
            parser.Call();

            int res = parser.GetInt();
            return res;
        }

        /// <summary>
        /// Smaže řádky podle podmínky.
        /// Nelze použít s knihovnou MonS3Reader.
        /// </summary>
        /// <param name="table">Název tabulky.</param>
        /// <param name="where">Podmínka. Musí být vyplněna.</param>
        /// <returns>Počet smazaných řádků.</returns>
        public int DeleteRows(string table, MonS3APIDataWhereList where)
        {
            _MonS3APIParser parser = _CreateParser("DeleteRows");
            parser.AddStr(table);
            _AddWhere(parser, where);
            parser.Call();

            int res = parser.GetInt();
            return res;
        }

        /// <summary>
        /// Smaže všechny řádky v tabulce.
        /// Nelze použít s knihovnou MonS3Reader.
        /// </summary>
        /// <param name="table">Název tabulky.</param>
        /// <returns>Počet smazaných řádků.</returns>
        public int DeleteAll(string table)
        {
            _MonS3APIParser parser = _CreateParser("DeleteAll");
            parser.AddStr(table);
            parser.Call();

            int res = parser.GetInt();
            return res;
        }

        /// <summary>
        /// Vytvoří komunikaci s DLL knihovnou.
        /// </summary>
        /// <returns>Komunikační třída s DLL knihovnou.</returns>
        protected _MonS3APIParser _CreateParser(string func, bool ignoreErrors = false)
        {
            return Parent._CreateParser(func, ObjectGUID, ignoreErrors);
        }

        /// <summary>
        /// Přidá vybrané sloupečky v selectu do parseru.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="what"></param>
        private void _AddWhat(_MonS3APIParser parser, MonS3APIDataWhatList what)
        {
            parser.AddArray((UInt32)what.Count);
            foreach (MonS3APIDataWhat item in what)
            {
                parser.AddStr(item.Value);
            }
        }

        /// <summary>
        /// Přidá podmínky do parseru.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="where"></param>
        private void _AddWhere(_MonS3APIParser parser, MonS3APIDataWhereList where)
        {
            if (where == null)
            {
                parser.AddArray(0);
                return;
            }

            parser.AddArray((UInt32)where.Count);
            foreach (MonS3APIDataWhere item in where)
            {
                parser.AddByte((byte)item.Type);

                if (item.Type != MonS3APIDataWhereType.Where)
                {
                    continue;
                }

                parser.AddStr(item.Column);
                parser.AddByte((byte)item.Operator);

                _AddValues(parser, item.Values);
            }
        }

        /// <summary>
        /// Přidá řazení do parseru.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="sort"></param>
        private void _AddSort(_MonS3APIParser parser, MonS3APIDataSortList sort)
        {
            if (sort == null)
            {
                parser.AddArray(0);
                return;
            }

            parser.AddArray((UInt32)sort.Count);
            foreach (MonS3APIDataSort item in sort)
            {
                parser.AddStr(item.Column);
                parser.AddByte((byte)item.Type);
            }
        }

        /// <summary>
        /// Přidá hodnoty do parseru.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="values"></param>
        private void _AddDataValues(_MonS3APIParser parser, MonS3APIDataValueList values)
        {
            parser.AddArray((UInt32)values.Count);
            foreach (MonS3APIDataValue item in values)
            {
                parser.AddStr(item.Column);
                _AddValue(parser, item.Value);
            }
        }

        /// <summary>
        /// Přidá hodnoty do parseru.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="values"></param>
        private void _AddValues(_MonS3APIParser parser, MonS3APIValueList values)
        {
            parser.AddArray((UInt32)values.Count);
            foreach (MonS3APIValue item in values)
            {
                _AddValue(parser, item);
            }
        }

        /// <summary>
        /// Přidá hodnotu do parseru.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="value"></param>
        private void _AddValue(_MonS3APIParser parser, MonS3APIValue value)
        {
            switch (value.ValType)
            {
                case _MonS3APIParser.ValType.Int: parser.AddInt(value.AsInt); break;
                case _MonS3APIParser.ValType.Int64: parser.AddInt64(value.AsInt64); break;
                case _MonS3APIParser.ValType.Ext: parser.AddExt(value.AsDecimal); break;
                case _MonS3APIParser.ValType.Str: parser.AddStr(value.AsString); break;
                case _MonS3APIParser.ValType.Bool: parser.AddBool(value.AsBool); break;
                case _MonS3APIParser.ValType.Date: parser.AddDate(value.AsDate); break;
                case _MonS3APIParser.ValType.Time: parser.AddTime(new DateTime(2000, 1, 1).Add(value.AsTime)); break;
                case _MonS3APIParser.ValType.DateTime: parser.AddDateTime(value.AsDateTime); break;
                case _MonS3APIParser.ValType.Data: parser.AddData(value.AsData); break;
                default: parser.AddEmpty(); break;
            }
        }

        /// <summary>
        /// Přečte info o agendě.
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        public MonS3APIAgenda _ParseAgendaInfo(_MonS3APIParser parser)
        {
            string visibleName = parser.GetStr();
            string name = parser.GetStr();
            string ic = parser.GetStr();
            MonS3APIRight right = (MonS3APIRight)parser.GetByte();
            string ext = parser.GetStr();
            bool isDemo = parser.GetBool();
            string poznamka = parser.GetStr();

            return new MonS3APIAgenda(visibleName, name, ic, right, ext, isDemo, poznamka);
        }
    }

    /// <summary>
    /// Právo na přístup.
    /// </summary>
    public enum MonS3APIRight
    {
        /// <summary>
        /// Není přístup. Nelze načíst žádné údaje.
        /// </summary>
        None = 0,
        /// <summary>
        /// Vrátí pouze základní údaje. Nelze dále číst.
        /// </summary>
        InfoOnly = 1,
        /// <summary>
        /// Pouze čtení.
        /// </summary>
        ReadOnly = 2,
        /// <summary>
        /// Čtení i zápis.
        /// </summary>
        ReadWrite = 3
    }

    /// <summary>
    /// Agenda.
    /// </summary>
    public class MonS3APIAgenda
    {
        /// <summary>
        /// Zobrazovaný název. Pokud není vyplněn, tak je shodný s hodnotou Name.
        /// </summary>
        public readonly string VisibleName;
        /// <summary>
        /// Obchodní název.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// IČO.
        /// </summary>
        public readonly string IC;
        /// <summary>
        /// Právo na přístup k agendě.
        /// </summary>
        public readonly MonS3APIRight Right;
        /// <summary>
        /// Koncovka složky agendy (001, 002, sju, spa, spb).
        /// </summary>
        public readonly string Ext;
        /// <summary>
        /// Zda se jedná o demo agendu.
        /// Ty jsou identifikovány podle koncovky názvu složky.
        /// Avšak jsou uživatelé, kteří demo agendu vezmou, promažou a pak používají.
        /// </summary>
        public readonly bool IsDemo;
        /// <summary>
        /// Poznámka. Na záložce nastavení agendy je to položka "Jiné údaje".
        /// </summary>
        public readonly string Poznamka;

        public MonS3APIAgenda(string visibleName, string name, string ic, MonS3APIRight right, string ext, bool isDemo, string poznamka)
        {
            this.VisibleName = visibleName;
            this.Name = name;
            this.IC = ic;
            this.Right = right;
            this.Ext = ext;
            this.IsDemo = isDemo;
            this.Poznamka = poznamka;
        }
    }

    /// <summary>
    /// List agend.
    /// </summary>
    public class MonS3APIAgendaList : List<MonS3APIAgenda>
    {
        /// <summary>
        /// Vrátí agendu podle koncovky.
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public MonS3APIAgenda GetByExt(string ext)
        {
            ext = ext.ToLower();

            foreach (MonS3APIAgenda item in this)
            {
                if (item.Ext == ext)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Vrátí první agendu, která není demo.
        /// </summary>
        /// <returns></returns>
        public MonS3APIAgenda GetFirstNoDemo()
        {
            foreach (MonS3APIAgenda item in this)
            {
                if (!item.IsDemo)
                {
                    return item;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Rok agendy.
    /// </summary>
    public class MonS3APIYear
    {
        /// <summary>
        /// Číselné označení roku.
        /// </summary>
        public readonly int Year;
        /// <summary>
        /// Datum od.
        /// </summary>
        public readonly DateTime DateFrom;
        /// <summary>
        /// Datum do.
        /// </summary>
        public readonly DateTime DateTo;
        /// <summary>
        /// Práva na přístup.
        /// </summary>
        public readonly MonS3APIRight Right;
        /// <summary>
        /// Koncovka složky roku.
        /// </summary>
        public readonly string Ext;

        public MonS3APIYear(int year, DateTime dateFrom, DateTime dateTo, MonS3APIRight right, string ext)
        {
            this.Year = year;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.Right = right;
            this.Ext = ext;
        }
    }

    /// <summary>
    /// List roků agendy - seřazený dle datumů.
    /// </summary>
    public class MonS3APIYearList : List<MonS3APIYear>
    {
        /// <summary>
        /// Vrátí rok podle data.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public MonS3APIYear GetByDate(DateTime date)
        {
            foreach (MonS3APIYear item in this)
            {
                if (date >= item.DateFrom && date <= item.DateTo)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Vrátí rok podle aktuálního data.
        /// </summary>
        /// <returns></returns>
        public MonS3APIYear GetActual()
        {
            return GetByDate(DateTime.Now);
        }
    }

    /// <summary>
    /// Tabulka.
    /// </summary>
    public class MonS3APITable
    {
        /// <summary>
        /// Název tabulky.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Název tabulky - malými písmeny.
        /// </summary>
        public readonly string NameL;
        /// <summary>
        /// Právo na tabulku.
        /// </summary>
        public readonly MonS3APIRight Right;
        /// <summary>
        /// Částečná práva na řádky tabulky.
        /// ReadWrite - lze číst a zapisovat vše (také podle toho, co dovolí Right pro celou tabulku)
        /// ReadOnly - některé řádky dat nelze měnit (také podle toho, co dovolí Right pro celou tabulku), vznikne exception NoRights
        /// None - některé řádky nelze číst - budou automaticky vyfiltrovány - tabulka se bude chovat, jako by v ní takové řádky nebyly
        /// </summary>
        public readonly MonS3APIRight PartialRightRow;
        /// <summary>
        /// Částečná práva na sloupečky tabulky.
        /// ReadWrite - lze číst a zapisovat vše (také podle toho, co dovolí Right pro celou tabulku) 
        /// ReadOnly - některé sloupečky dat nelze měnit (také podle toho, co dovolí Right pro celou tabulku) - pokud se provede Update, tak se tento sloupec neaktualizuje, v případě přidání záznamu jeho hodnota bude prázdná
        /// None - sloupeček bude mít vždy prázdnou hodnotu, jako by nebyl vyplněn, dále platí, že nelze hodnota nastavit ani měnit jako v případě ReadOnly
        /// </summary>
        public readonly MonS3APIRight PartialRightCol;


        public MonS3APITable(string name, MonS3APIRight right, MonS3APIRight partialRightRow, MonS3APIRight partialRightCol)
        {
            this.Name = name;
            this.NameL = name.ToLower();
            this.Right = right;
            this.PartialRightRow = partialRightRow;
            this.PartialRightCol = partialRightCol;
        }
    }

    /// <summary>
    /// List všech tabulek.
    /// </summary>
    public class MonS3APITableList : List<MonS3APITable>
    {
        /// <summary>
        /// Vrátí tabulku podle názvu.
        /// </summary>
        /// <param name="name">Název tabulky.</param>
        /// <returns></returns>
        public MonS3APITable GetByName(string name)
        {
            name = name.ToLower();

            foreach (MonS3APITable item in this)
            {
                if (item.NameL == name)
                {
                    return item;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Datový typ sloupečku.
    /// </summary>
    public enum MonS3APIColumnDataType
    {
        ID = 1,
        String = 2,
        Int = 3,
        Int64 = 4,
        Decimal = 5,
        Boolean = 6,
        Date = 7,
        Time = 8,
        DateTime = 9,
        GUID = 10,
        Blob = 11
    }

    /// <summary>
    /// Sloupeček tabulky.
    /// </summary>
    public class MonS3APITableColumn
    {
        /// <summary>
        /// Název sloupečku.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Název sloupečku - malými písmeny.
        /// </summary>
        public readonly string NameL;
        /// <summary>
        /// Datový typ sloupečku.
        /// </summary>
        public readonly MonS3APIColumnDataType DataType;
        /// <summary>
        /// Počet znaků, pokud je typ String.
        /// </summary>
        public readonly int StringSize;

        public MonS3APITableColumn(string name, MonS3APIColumnDataType dataType, int stringSize)
        {
            this.Name = name;
            this.NameL = name.ToLower();
            this.DataType = dataType;
            this.StringSize = stringSize;
        }
    }

    /// <summary>
    /// Všechny sloupečky tabulky.
    /// </summary>
    public class MonS3APITableColumnList : List<MonS3APITableColumn>
    {
        /// <summary>
        /// Vrátí ID sloupeček.
        /// </summary>
        /// <returns></returns>
        public MonS3APITableColumn GetIDColumn()
        {
            foreach (MonS3APITableColumn item in this)
            {
                if (item.DataType == MonS3APIColumnDataType.ID)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Vrátí sloupeček podle názvu.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MonS3APITableColumn GetByName(string name)
        {
            name = name.ToLower();

            foreach (MonS3APITableColumn item in this)
            {
                if (item.NameL == name)
                {
                    return item;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Datová hodnota.
    /// </summary>
    public class MonS3APIValue
    {
        /// <summary>
        /// Typ hodnoty.
        /// </summary>
        private _MonS3APIParser.ValType valType = _MonS3APIParser.ValType.Empty;
        /// <summary>
        /// Číselná hodnota nebo boolean.
        /// </summary>
        private decimal valDecimal;
        /// <summary>
        /// Stringová hodnota.
        /// </summary>
        private string valStr;
        /// <summary>
        /// Hodnota data a času nebo jen data.
        /// </summary>
        private DateTime valDateTime;
        /// <summary>
        /// Hodnota času.
        /// </summary>
        private TimeSpan valTime;
        /// <summary>
        /// Blob.
        /// </summary>
        private byte[] valData;

        public void Clear()
        {
            valType = _MonS3APIParser.ValType.Empty;
        }

        public _MonS3APIParser.ValType ValType
        {
            get
            {
                return valType;
            }
        }

        public int AsInt
        {
            get
            {
                try
                {
                    switch (valType)
                    {
                        case _MonS3APIParser.ValType.Int:
                        case _MonS3APIParser.ValType.Int64:
                        case _MonS3APIParser.ValType.Ext: return (int)valDecimal;
                    }
                }
                catch
                {
                }

                return 0;
            }
            set
            {
                valType = _MonS3APIParser.ValType.Int;
                valDecimal = value;
            }
        }

        public Int64 AsInt64
        {
            get
            {
                try
                {
                    switch (valType)
                    {
                        case _MonS3APIParser.ValType.Int:
                        case _MonS3APIParser.ValType.Int64:
                        case _MonS3APIParser.ValType.Ext: return (Int64)valDecimal;
                    }
                }
                catch
                {
                }

                return 0;
            }
            set
            {
                valType = _MonS3APIParser.ValType.Int64;
                valDecimal = value;
            }
        }

        public decimal AsDecimal
        {
            get
            {
                switch (valType)
                {
                    case _MonS3APIParser.ValType.Int:
                    case _MonS3APIParser.ValType.Int64:
                    case _MonS3APIParser.ValType.Ext: return valDecimal;
                }

                return 0;
            }
            set
            {
                valType = _MonS3APIParser.ValType.Ext;
                valDecimal = value;
            }
        }

        public string AsString
        {
            get
            {
                if (valType == _MonS3APIParser.ValType.Str)
                {
                    return valStr;
                }

                return "";
            }
            set
            {
                valType = _MonS3APIParser.ValType.Str;
                valStr = value;
            }
        }

        public bool AsBool
        {
            get
            {
                if (valType == _MonS3APIParser.ValType.Bool)
                {
                    return valDecimal != 0;
                }

                return false;
            }
            set
            {
                valType = _MonS3APIParser.ValType.Bool;

                if (value)
                {
                    valDecimal = 1;
                }
                else
                {
                    valDecimal = 0;
                }
            }
        }

        public DateTime AsDate
        {
            get
            {
                switch (valType)
                {
                    case _MonS3APIParser.ValType.Date:
                    case _MonS3APIParser.ValType.DateTime: return valDateTime.Date;
                }

                return new DateTime();
            }
            set
            {
                valType = _MonS3APIParser.ValType.Date;
                valDateTime = value.Date;
            }
        }

        public TimeSpan AsTime
        {
            get
            {
                switch (valType)
                {
                    case _MonS3APIParser.ValType.Time: return valTime;
                    case _MonS3APIParser.ValType.DateTime: return valDateTime.TimeOfDay;
                }

                return new TimeSpan();
            }
            set
            {
                valType = _MonS3APIParser.ValType.Time;
                valTime = value;
            }
        }

        public DateTime AsDateTime
        {
            get
            {
                switch (valType)
                {
                    case _MonS3APIParser.ValType.Date: return valDateTime.Date;
                    case _MonS3APIParser.ValType.DateTime: return valDateTime;
                }

                return new DateTime();
            }
            set
            {
                valType = _MonS3APIParser.ValType.DateTime;
                valDateTime = value;
            }
        }

        public byte[] AsData
        {
            get
            {
                if (valType == _MonS3APIParser.ValType.Data)
                {
                    return valData;
                }

                return new byte[0];
            }
            set
            {
                valType = _MonS3APIParser.ValType.Data;
                valData = value;
            }
        }

        public static MonS3APIValue CreateEmpty()
        {
            return new MonS3APIValue();
        }

        public static MonS3APIValue CreateInt(int value)
        {
            return new MonS3APIValue() { AsInt = value};
        }

        public static MonS3APIValue CreateInt64(Int64 value)
        {
            return new MonS3APIValue() { AsInt64 = value };
        }

        public static MonS3APIValue CreateDecimal(Decimal value)
        {
            return new MonS3APIValue() { AsDecimal = value };
        }

        public static MonS3APIValue CreateString(string value)
        {
            return new MonS3APIValue() { AsString = value };
        }

        public static MonS3APIValue CreateBool(bool value)
        {
            return new MonS3APIValue() { AsBool = value };
        }

        public static MonS3APIValue CreateDate(DateTime value)
        {
            return new MonS3APIValue() { AsDate = value };
        }

        public static MonS3APIValue CreateTime(TimeSpan value)
        {
            return new MonS3APIValue() { AsTime = value };
        }

        public static MonS3APIValue CreateDateTime(DateTime value)
        {
            return new MonS3APIValue() { AsDateTime = value };
        }

        public static MonS3APIValue CreateData(byte[] value)
        {
            return new MonS3APIValue() { AsData = value };
        }

        public string ToSQLString()
        {
            switch (valType)
            {
                case _MonS3APIParser.ValType.Int: return AsInt.ToString();
                case _MonS3APIParser.ValType.Int64: return AsInt64.ToString();
                case _MonS3APIParser.ValType.Ext:
                    CultureInfo CInfo = new CultureInfo(CultureInfo.InvariantCulture.Name);
                    CInfo.NumberFormat.CurrencyDecimalDigits = 4;
                    CInfo.NumberFormat.CurrencyDecimalSeparator = ".";
                    CInfo.NumberFormat.CurrencyGroupSeparator = "";
                    CInfo.NumberFormat.CurrencySymbol = "";
                    CInfo.NumberFormat.NumberDecimalDigits = 4;
                    CInfo.NumberFormat.NumberDecimalSeparator = ".";
                    CInfo.NumberFormat.NumberGroupSeparator = "";
                    return AsDecimal.ToString(CInfo);
                case _MonS3APIParser.ValType.Str: return "'" + AsString + "'";
                case _MonS3APIParser.ValType.Bool:
                    if (AsBool)
                        return "1";
                    return "0";
                case _MonS3APIParser.ValType.Date: return "Date(" + AsDate.ToString("yyyy-MM-dd") + ")";
                case _MonS3APIParser.ValType.Time: return "Time(" + AsTime.ToString(@"hh\:mm\:ss\.fff") + ")"; 
                case _MonS3APIParser.ValType.DateTime: return "DateTime(" + AsDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + ")"; 
            }

            return "<empty>";
        }
    }

    /// <summary>
    /// List datových hodnot.
    /// </summary>
    public class MonS3APIValueList: List<MonS3APIValue>
    {
    }

    /// <summary>
    /// Sloupečky, které se budou načítat SELECTem.
    /// </summary>
    public class MonS3APIDataWhatList : List<MonS3APIDataWhat>
    {
        /// <summary>
        /// Přidá vše.
        /// </summary>
        public void AddAll()
        {
            Add(new MonS3APIDataWhat("*"));
        }
    }

    /// <summary>
    /// Sloupeček, který se bude načítat SELECTem.
    /// </summary>
    public class MonS3APIDataWhat
    {
        /// <summary>
        /// Sloupeček.
        /// </summary>
        public readonly string Value;

        public MonS3APIDataWhat(string value)
        {
            this.Value = value;
        }
    }

    /// <summary>
    /// Typ podmínky.
    /// </summary>
    public enum MonS3APIDataWhereType
    {
        /// <summary>
        /// Otevřít závorku.
        /// </summary>
        BracketStart = 1,
        /// <summary>
        /// Zavřít závorku.
        /// </summary>
        BracketEnd = 2,
        /// <summary>
        /// Operátor AND.
        /// </summary>
        OperatorAND = 3,
        /// <summary>
        /// Operátor OR.
        /// </summary>
        OperatorOR = 4,
        /// <summary>
        /// Položka podmínky.
        /// </summary>
        Where = 5
    }

    /// <summary>
    /// Operátor podmínky.
    /// </summary>
    public enum MonS3APIDataWhereOperator
    {
        /// <summary>
        /// Rovná se.
        /// </summary>
        Equals = 1,
        /// <summary>
        /// Nerovná se.
        /// </summary>
        NotEquals = 2,
        /// <summary>
        /// Menší než. (Pouze číselné hodnoty.)
        /// </summary>
        LessThan = 3,
        /// <summary>
        /// Menší rovno. (Pouze číselné hodnoty.)
        /// </summary>
        LessEq = 4,
        /// <summary>
        /// Větší než. (Pouze číselné hodnoty.)
        /// </summary>
        GreaterThan = 5,
        /// <summary>
        /// Větší rovno. (Pouze číselné hodnoty.)
        /// </summary>
        GreaterEq = 6,
        /// <summary>
        /// Začíná. (Pouze řetězce.)
        /// </summary>
        BeginsWith = 7,
        /// <summary>
        /// Nezačíná. (Pouze řetězce.)
        /// </summary>
        NotBeginsWith = 8,
        /// <summary>
        /// Končí. (Pouze řetězce.)
        /// </summary>
        EndsWith = 9,
        /// <summary>
        /// Nekončí. (Pouze pro řetězce).
        /// </summary>
        NotEndsWith = 10,
        /// <summary>
        /// Obsahuje. (Pouze řetězce.)
        /// </summary>
        Contains = 11,
        /// <summary>
        /// Neobsahuje. (Pouze řetězce.)
        /// </summary>
        NotContains = 12,
        /// <summary>
        /// Je ve výčtu hodnot.
        /// </summary>
        InValues = 13,
        /// <summary>
        /// Není ve výčtu hodnot.
        /// </summary>
        NotInValues = 14,
        /// <summary>
        /// Je mezi hodnotami.
        /// </summary>
        Between = 15,
        /// <summary>
        /// Není mezi hodnotami.
        /// </summary>
        NotBetween = 16
    }

    /// <summary>
    /// Podmínka.
    /// </summary>
    public class MonS3APIDataWhereList: List<MonS3APIDataWhere>
    {
        /// <summary>
        /// Otevřít závorku.
        /// </summary>
        public void AddBracketStart()
        {
            Add(new MonS3APIDataWhere(MonS3APIDataWhereType.BracketStart));
        }

        /// <summary>
        /// Zavřít závorku.
        /// </summary>
        public void AddBracketEnd()
        {
            Add(new MonS3APIDataWhere(MonS3APIDataWhereType.BracketEnd));
        }

        /// <summary>
        /// Operátor AND.
        /// </summary>
        public void AddOperatorAND()
        {
            Add(new MonS3APIDataWhere(MonS3APIDataWhereType.OperatorAND));
        }

        /// <summary>
        /// Operátor OR.
        /// </summary>
        public void AddOperatorOR()
        {
            Add(new MonS3APIDataWhere(MonS3APIDataWhereType.OperatorOR));
        }

        /// <summary>
        /// Položka podmínky.
        /// </summary>
        public void AddWhere(string column, MonS3APIDataWhereOperator op, MonS3APIValue value)
        {
            Add(new MonS3APIDataWhere(column, op, value));
        }

        /// <summary>
        /// Položka podmínky.
        /// </summary>
        public void AddWhere(string column, MonS3APIDataWhereOperator op, MonS3APIValueList values)
        {
            Add(new MonS3APIDataWhere(column, op, values));
        }

        /// <summary>
        /// Jen pro debug. Vypsání SQL příkazu.
        /// </summary>
        public string GetSQLCommandString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (MonS3APIDataWhere item in this)
            {
                switch (item.Type)
                {
                    case MonS3APIDataWhereType.BracketStart: sb.Append("("); break;
                    case MonS3APIDataWhereType.BracketEnd: sb.Append(")"); break;
                    case MonS3APIDataWhereType.OperatorAND: sb.Append(" AND "); break;
                    case MonS3APIDataWhereType.OperatorOR: sb.Append(" OR "); break;
                    case MonS3APIDataWhereType.Where:
                        sb.Append("[" + item.Column + "]");

                        switch (item.Operator)
                        {
                            case MonS3APIDataWhereOperator.Equals: sb.Append("=" + item.Values[0].ToSQLString()); break;
                            case MonS3APIDataWhereOperator.NotEquals: sb.Append("<>" + item.Values[0].ToSQLString()); break;
                            case MonS3APIDataWhereOperator.LessThan: sb.Append("<" + item.Values[0].ToSQLString()); break;
                            case MonS3APIDataWhereOperator.LessEq: sb.Append("<=" + item.Values[0].ToSQLString()); break;
                            case MonS3APIDataWhereOperator.GreaterThan: sb.Append(">" + item.Values[0].ToSQLString()); break;
                            case MonS3APIDataWhereOperator.GreaterEq: sb.Append(">=" + item.Values[0].ToSQLString()); break;
                            case MonS3APIDataWhereOperator.BeginsWith: 
                            case MonS3APIDataWhereOperator.NotBeginsWith:
                            case MonS3APIDataWhereOperator.EndsWith:
                            case MonS3APIDataWhereOperator.NotEndsWith:
                            case MonS3APIDataWhereOperator.Contains:
                            case MonS3APIDataWhereOperator.NotContains:
                                if (item.Operator == MonS3APIDataWhereOperator.NotBeginsWith || item.Operator == MonS3APIDataWhereOperator.NotEndsWith || item.Operator == MonS3APIDataWhereOperator.NotContains)
                                    sb.Append(" NOT");
                                sb.Append(" LIKE '");
                                if (item.Operator == MonS3APIDataWhereOperator.EndsWith || item.Operator == MonS3APIDataWhereOperator.NotEndsWith || item.Operator == MonS3APIDataWhereOperator.Contains || item.Operator == MonS3APIDataWhereOperator.NotContains)
                                    sb.Append("%");
                                sb.Append(item.Values[0].AsString);
                                if (item.Operator == MonS3APIDataWhereOperator.BeginsWith || item.Operator == MonS3APIDataWhereOperator.NotBeginsWith || item.Operator == MonS3APIDataWhereOperator.Contains || item.Operator == MonS3APIDataWhereOperator.NotContains)
                                    sb.Append("%");
                                sb.Append("'");
                                break;
                            case MonS3APIDataWhereOperator.InValues:
                            case MonS3APIDataWhereOperator.NotInValues:
                                if (item.Operator == MonS3APIDataWhereOperator.NotInValues)
                                    sb.Append(" NOT");
                                sb.Append(" IN(");

                                for (int i = 0; i < item.Values.Count; i++)
                                {
                                    if (i != 0)
                                        sb.Append(",");
                                    sb.Append(item.Values[i].ToSQLString());
                                }

                                sb.Append(")");
                                break;
                            case MonS3APIDataWhereOperator.Between:
                            case MonS3APIDataWhereOperator.NotBetween:
                                if (item.Operator == MonS3APIDataWhereOperator.NotBetween)
                                    sb.Append(" NOT");
                                sb.Append(" BETWEEN " + item.Values[0].ToSQLString() + " AND " + item.Values[1].ToSQLString());
                                break;
                        }

                        break;
                }
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// Položka podmínky.
    /// </summary>
    public class MonS3APIDataWhere
    {
        /// <summary>
        /// Typ.
        /// </summary>
        public readonly MonS3APIDataWhereType Type;
        /// <summary>
        /// Sloupec podmínky.
        /// </summary>
        public readonly string Column;
        /// <summary>
        /// Operátor podmínky.
        /// </summary>
        public readonly MonS3APIDataWhereOperator Operator;
        /// <summary>
        /// Hodnoty pomínky.
        /// </summary>
        public readonly MonS3APIValueList Values;

        public MonS3APIDataWhere(MonS3APIDataWhereType type)
        {
            this.Type = type;
        }

        public MonS3APIDataWhere(string column, MonS3APIDataWhereOperator op, MonS3APIValue value)
        {
            this.Type = MonS3APIDataWhereType.Where;
            this.Column = column;
            this.Operator = op;

            this.Values = new MonS3APIValueList();
            this.Values.Add(value);
        }

        public MonS3APIDataWhere(string column, MonS3APIDataWhereOperator op, MonS3APIValueList values)
        {
            this.Type = MonS3APIDataWhereType.Where;
            this.Column = column;
            this.Operator = op;
            this.Values = values;
        }
    }

    /// <summary>
    /// Typ řazení.
    /// </summary>
    public enum MonS3APIDataSortType
    {
        /// <summary>
        /// Vzestupně. Tedy od nejmenšího po největší. A-Z.
        /// </summary>
        Ascending = 1,
        /// <summary>
        /// Sestupně. Tedy od největšího po nejmenší. Z-A.
        /// </summary>
        Descending = 2
    }

    /// <summary>
    /// Řazení SELECTu.
    /// </summary>
    public class MonS3APIDataSortList : List<MonS3APIDataSort>
    {
        /// <summary>
        /// Přidá řazení.
        /// </summary>
        /// <param name="column">Sloupec.</param>
        /// <param name="type">Typ.</param>
        public void AddSort(string column, MonS3APIDataSortType type)
        {
            Add(new MonS3APIDataSort(column, type));
        }
    }

    /// <summary>
    /// Řazení SELECTu.
    /// </summary>
    public class MonS3APIDataSort
    {
        /// <summary>
        /// Sloupec.
        /// </summary>
        public readonly string Column;
        /// <summary>
        /// Typ řazení.
        /// </summary>
        public readonly MonS3APIDataSortType Type;

        public MonS3APIDataSort(string column, MonS3APIDataSortType type)
        {
            this.Column = column;
            this.Type = type;
        }
    }

    /// <summary>
    /// Data pro přidání nebo úpravu řádku.
    /// </summary>
    public class MonS3APIDataValueList: List<MonS3APIDataValue>
    { 
        /// <summary>
        /// Přidá hodnotu.
        /// </summary>
        /// <param name="column">Název sloupce.</param>
        /// <param name="value">Hodnota.</param>
        public void AddValue(string column, MonS3APIValue value)
        {
            Add(new MonS3APIDataValue(column, value));
        }
    }

    /// <summary>
    /// Hodnota pro přidání nebo úpravu řádku.
    /// </summary>
    public class MonS3APIDataValue
    {
        /// <summary>
        /// Název sloupce.
        /// </summary>
        public readonly string Column;
        /// <summary>
        /// Hodnota.
        /// </summary>
        public readonly MonS3APIValue Value;

        public MonS3APIDataValue(string column, MonS3APIValue value)
        {
            this.Column = column;
            this.Value = value;
        }
    }

    /// <summary>
    /// Výsledek SELECTu.
    /// </summary>
    public class MonS3APIDataSelectResult
    {
        /// <summary>
        /// Odkaz na třídu, která tuhle vytvořila.
        /// </summary>
        private readonly MonS3APIDataBase DataBase;
        /// <summary>
        /// Třída obsahující data.
        /// </summary>
        private readonly _MonS3APIParser Parser;
        /// <summary>
        /// Pozice v datech.
        /// </summary>
        private int Position;
        /// <summary>
        /// Interní seznam sloupečků.
        /// </summary>
        private readonly List<string> Columns;
        /// <summary>
        /// Rozparsovaný řádek.
        /// </summary>
        private MonS3APIValueList Values = new MonS3APIValueList();

        /// <summary>
        /// Počet získaných záznamů.
        /// </summary>
        public readonly int Count;


        public MonS3APIDataSelectResult(MonS3APIDataBase dataBase, _MonS3APIParser parser)
        {
            this.DataBase = dataBase;
            this.Parser = parser;

            this.Position = -1;
            this.Count = (int)parser.GetArray();

            if (this.Count == 0)
            {
                return;
            }

            int colCount = (int)parser.GetArray();
            this.Columns = new List<string>();
            this.Columns.Capacity = colCount;

            for (int i = 0; i < colCount; i++)
            {
                this.Columns.Add(parser.GetStr());
            }
        }

        /// <summary>
        /// Skočí na další záznam.
        /// </summary>
        /// <returns>Vrací, zda se to povedlo.</returns>
        public bool Next()
        {
            Values.Clear();

            Position++;
            if (Position >= Count)
            {
                return false;
            }

            for (int i = 0; i < Columns.Count; i++)
            {
                MonS3APIValue value = new MonS3APIValue();

                _MonS3APIParser.ValType valType = (_MonS3APIParser.ValType)Parser._ReadWord();
                switch (valType)
                {
                    case _MonS3APIParser.ValType.Int: value.AsInt = Parser._ReadInt(); break;
                    case _MonS3APIParser.ValType.Int64: value.AsInt64 = Parser._ReadInt64(); break;
                    case _MonS3APIParser.ValType.Ext: value.AsDecimal = Parser._ReadDecimal(); break;
                    case _MonS3APIParser.ValType.Str: value.AsString = Parser._ReadStr(); break;
                    case _MonS3APIParser.ValType.Bool: value.AsBool = Parser._ReadBool(); break;
                    case _MonS3APIParser.ValType.Date: value.AsDate = Parser._ReadDate(); break;
                    case _MonS3APIParser.ValType.Time: value.AsTime = Parser._ReadTime().TimeOfDay; break;
                    case _MonS3APIParser.ValType.DateTime: value.AsDateTime = Parser._ReadDateTime(); break;
                    case _MonS3APIParser.ValType.Data: value.AsData = Parser._ReadData(); break;
                }

                Values.Add(value);
            }

            return true;
        }

        /// <summary>
        /// Vrátí hodnotu sloupečku podle jména.
        /// </summary>
        /// <param name="column">Název sloupečku</param>
        /// <returns>Hodnota sloupečku.</returns>
        public MonS3APIValue GetColByName(string column)
        {
            column = column.ToLower();
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i] == column)
                {
                    return Values[i];
                }
            }

            if (DataBase.SettingErrorIfColNotFound)
            {
                throw new MonS3APIException(MonS3APIExceptionType.ColNotFound, "");
            }
            else
            {
                return new MonS3APIValue();
            }
        }
    }

    /// <summary>
    /// Informace o přihlášeném uživateli.
    /// </summary>
    public class MonS3APIUserInfo
    {
        /// <summary>
        /// GUID uživatele. Tento údaj bude pravděpodobně dostupný od verze Money 19.800 nebo později.
        /// Jakmile bude dostupný, upřednostněte tuto hodnotu.
        /// </summary>
        public readonly string GUID;
        /// <summary>
        /// Hodnota ID_Pristup z tabulky PrisUziv.
        /// Jedná se o IDčko (increment) uživatele.
        /// Doporučuji použít místo téhle hodnoty GUID.
        /// </summary>
        public readonly int ID;
        /// <summary>
        /// Jméno uživatele.
        /// </summary>
        public readonly string Jmeno;
        /// <summary>
        /// Jméno konfigurace.
        /// </summary>
        public readonly string ConfigName;
        /// <summary>
        /// Poznámka.
        /// </summary>
        public readonly string Poznamka;

        public MonS3APIUserInfo(string guid, int id, string jmeno, string configName, string poznamka)
        {
            this.GUID = guid;
            this.ID = id;
            this.Jmeno = jmeno;
            this.ConfigName = configName;
            this.Poznamka = poznamka;
        }
    }

    /// <summary>
    /// Právo přihlášeného uživatele.
    /// </summary>
    public class MonS3APIUserRight
    {
        /// <summary>
        /// Název práva.
        /// </summary>
        public readonly string RightName;
        /// <summary>
        /// Název práva - malými písmeny.
        /// </summary>
        public readonly string RightNameL;
        /// <summary>
        /// Hodnota práva.
        /// </summary>
        public readonly string RightValue;

        public MonS3APIUserRight(string rightName, string rightValue)
        {
            this.RightName = rightName;
            this.RightNameL = rightName.ToLower();
            this.RightValue = rightValue;
        }
    }

    /// <summary>
    /// Všechna práva přihlášeného uživatele.
    /// </summary>
    public class MonS3APIUserRightList : List<MonS3APIUserRight>
    {
        /// <summary>
        /// Vrátí právo podle názvu.
        /// </summary>
        /// <param name="rightName"></param>
        /// <returns></returns>
        public MonS3APIUserRight GetByName(string rightName)
        {
            rightName = rightName.ToLower();
            foreach (MonS3APIUserRight item in this)
            {
                if (item.RightNameL == rightName)
                {
                    return item;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Samotná komunikace s DLL knihovnou.
    /// </summary>
    public class _MonS3APIParser
    {
        /// <summary>
        /// Typ proměnné.
        /// </summary>
        public enum ValType
        {
            Empty = 1,
            Bool = 2,
            Byte = 3,
            Word = 4,
            Int = 5,
            Int64 = 6,
            Cardinal = 7,
            Ext = 8,
            Curr = 9,
            Str = 10,
            Date = 11,
            Time = 12,
            DateTime = 13,
            Data = 14,
            Array = 15
        }

        /// <summary>
        /// Hlavní třída obsluhující DLL knihovnu.
        /// </summary>
        private readonly MonS3APIDataMain Parent;
        /// <summary>
        /// Zda se mají ignorovat chyby.
        /// </summary>
        private readonly bool IgnoreErrors;
        /// <summary>
        /// Buffer na posílání dat.
        /// </summary>
        private MemoryStream OutputStream = new MemoryStream(1024);
        /// <summary>
        /// Nastavení pro konverzi čísel.
        /// </summary>
        private static CultureInfo CInfo;
        /// <summary>
        /// Vstupní buffer.
        /// </summary>
        private byte[] InputBuffer;
        /// <summary>
        /// //Aktuální pozice ve vstupním bufferu.
        /// </summary>
        private UInt32 InputPos;

        /// <summary>
        /// Vytvoří komunikaci s DLL knihovnou.
        /// </summary>
        /// <param name="parent">Hlavní třída obsluhující DLL knihovnu.</param>
        /// <param name="clientGUID">GUID instance pro komunikaci s DLL knihovnou.</param>
        /// <param name="objectGUID">GUID objektu - společná data nebo data agendy.</param>
        /// <param name="func">Název volané funkce.</param>
        /// <param name="ignoreErrors">Ignorování chyb.</param>
        public _MonS3APIParser(MonS3APIDataMain parent, byte[] clientGUID, byte[] objectGUID, string func, bool ignoreErrors)
        {
            this.Parent = parent;
            this.IgnoreErrors = ignoreErrors;

            if (CInfo == null)
            {
                CInfo = new CultureInfo(CultureInfo.InvariantCulture.Name);
                CInfo.NumberFormat.CurrencyDecimalDigits = 4;
                CInfo.NumberFormat.CurrencyDecimalSeparator = ".";
                CInfo.NumberFormat.CurrencyGroupSeparator = "";
                CInfo.NumberFormat.CurrencySymbol = "";
                CInfo.NumberFormat.NumberDecimalDigits = 4;
                CInfo.NumberFormat.NumberDecimalSeparator = ".";
                CInfo.NumberFormat.NumberGroupSeparator = "";
            }

            //Velikost pole.
            OutputStream.WriteByte(0);
            OutputStream.WriteByte(0);
            OutputStream.WriteByte(0);
            OutputStream.WriteByte(0);

            if (clientGUID == null)
            {
                AddData(new byte[16]);
            }
            else
            {
                AddData(clientGUID);
            }

            if (objectGUID == null)
            {
                AddData(new byte[16]);
            }
            else
            {
                AddData(objectGUID);
            }

            AddStr(func);
        }

        public int Skip(UInt32 size)
        {
            int old = (int)InputPos;
            InputPos += size;
            return old;
        }

        public bool GetBool()
        {
            Skip(sizeof(UInt16));
            return _ReadBool();
        }

        public byte GetByte()
        {
            Skip(sizeof(UInt16));
            return _ReadByte();
        }

        public UInt16 GetWord()
        {
            Skip(sizeof(UInt16));
            return _ReadWord();
        }

        public Int32 GetInt()
        {
            Skip(sizeof(UInt16));
            return _ReadInt();
        }

        public Int64 GetInt64()
        {
            Skip(sizeof(UInt16));
            return _ReadInt64();
        }

        public UInt32 GetCardinal()
        {
            Skip(sizeof(UInt16));
            return _ReadCardinal();
        }

        public Decimal GetDecimal()
        {
            Skip(sizeof(UInt16));
            return _ReadDecimal();
        }

        public string GetStr()
        {
            Skip(sizeof(UInt16));
            return _ReadStr();
        }

        public DateTime GetDate()
        {
            Skip(sizeof(UInt16));
            return _ReadDate();
        }

        public DateTime GetTime()
        {
            Skip(sizeof(UInt16));
            return _ReadTime();
        }

        public DateTime GetDateTime()
        {
            Skip(sizeof(UInt16));
            return _ReadDateTime();
        }

        public byte[] GetData()
        {
            Skip(sizeof(UInt16));
            return _ReadData();
        }

        public UInt32 GetArray()
        {
            Skip(sizeof(UInt16));
            return _ReadArray();
        }

        public bool _ReadBool()
        {
            return _ReadByte() == 255;
        }

        public byte _ReadByte()
        {
            return InputBuffer[Skip(sizeof(byte))];
        }

        public UInt16 _ReadWord()
        {
            return BitConverter.ToUInt16(InputBuffer, Skip(sizeof(UInt16)));
        }

        public Int32 _ReadInt()
        {
            return BitConverter.ToInt32(InputBuffer, Skip(sizeof(Int32)));
        }

        public Int64 _ReadInt64()
        {
            return BitConverter.ToInt64(InputBuffer, Skip(sizeof(Int64)));
        }

        public UInt32 _ReadCardinal()
        {
            return BitConverter.ToUInt32(InputBuffer, Skip(sizeof(UInt32)));
        }

        public Decimal _ReadDecimal()
        {
            string str = _ReadStr();
            return Decimal.Parse(str, CInfo);
        }

        public string _ReadStr()
        {
            UInt32 len = _ReadCardinal();

            if (len == 0)
            {
                return "";
            }

            return System.Text.Encoding.Unicode.GetString(InputBuffer, Skip(len), (int)len);
        }

        public DateTime _ReadDate()
        {
            int year = _ReadWord();
            int month = _ReadByte();
            int day = _ReadByte();

            if (year < 1900)
            {
                return new DateTime();
            }

            return new DateTime(year, month, day);
        }

        public DateTime _ReadTime()
        {
            int hour = _ReadByte();
            int min = _ReadByte();
            int sec = _ReadByte();
            int msec = _ReadWord();
            return new DateTime(1899, 12, 30, hour, min, sec, msec);
        }

        public DateTime _ReadDateTime()
        {
            int year = _ReadWord();
            int month = _ReadByte();
            int day = _ReadByte();
            int hour = _ReadByte();
            int min = _ReadByte();
            int sec = _ReadByte();
            int msec = _ReadWord();
            return new DateTime(year, month, day, hour, min, sec, msec);
        }

        public byte[] _ReadData()
        {
            UInt32 len = _ReadCardinal();
            if (len == 0)
            {
                return new byte[0];
            }

            byte[] buffer = new byte[len];
            Buffer.BlockCopy(InputBuffer, Skip(len), buffer, 0, (int)len);
            return buffer;
        }

        public UInt32 _ReadArray()
        {
            return _ReadCardinal();
        }

        public void AddEmpty()
        {
            _WriteWord((UInt16)ValType.Empty);
        }

        public void AddBool(bool val)
        {
            _WriteWord((UInt16)ValType.Bool);
            _WriteBool(val);
        }

        public void AddByte(byte val)
        {
            _WriteWord((UInt16)ValType.Byte);
            _WriteByte(val);
        }

        public void AddWord(UInt16 val)
        {
            _WriteWord((UInt16)ValType.Word);
            _WriteWord(val);
        }

        public void AddInt(Int32 val)
        {
            _WriteWord((UInt16)ValType.Int);
            _WriteInt(val);
        }

        public void AddInt64(Int64 val)
        {
            _WriteWord((UInt16)ValType.Int64);
            _WriteInt64(val);
        }

        public void AddCardinal(UInt32 val)
        {
            _WriteWord((UInt16)ValType.Cardinal);
            _WriteCardinal(val);
        }

        public void AddExt(Decimal val)
        {
            _WriteWord((UInt16)ValType.Ext);
            _WriteDecimal(val);
        }

        public void AddCurr(Decimal val)
        {
            _WriteWord((UInt16)ValType.Curr);
            _WriteDecimal(val);
        }

        public void AddStr(string val)
        {
            _WriteWord((UInt16)ValType.Str);
            _WriteStr(val);
        }

        public void AddDate(DateTime val)
        {
            _WriteWord((UInt16)ValType.Date);
            _WriteDate(val);
        }

        public void AddTime(DateTime val)
        {
            _WriteWord((UInt16)ValType.Time);
            _WriteTime(val);
        }

        public void AddDateTime(DateTime val)
        {
            _WriteWord((UInt16)ValType.DateTime);
            _WriteDateTime(val);
        }

        public void AddData(byte[] val)
        {
            _WriteWord((UInt16)ValType.Data);
            _WriteData(val);
        }

        public void AddArray(UInt32 count)
        {
            _WriteWord((UInt16)ValType.Array);
            _WriteArray(count);
        }

        public void _WriteBool(bool val)
        {
            if (val)
            {
                _WriteByte(255);
            }
            else
            {
                _WriteByte(0);
            }
        }

        public void _WriteByte(byte val)
        {
            OutputStream.WriteByte(val);
        }

        public void _WriteWord(UInt16 val)
        {
            OutputStream.Write(BitConverter.GetBytes(val), 0, sizeof(UInt16));
        }

        public void _WriteInt(Int32 val)
        {
            OutputStream.Write(BitConverter.GetBytes(val), 0, sizeof(Int32));
        }

        public void _WriteInt64(Int64 val)
        {
            OutputStream.Write(BitConverter.GetBytes(val), 0, sizeof(Int64));
        }

        public void _WriteCardinal(UInt32 val)
        {
            OutputStream.Write(BitConverter.GetBytes(val), 0, sizeof(UInt32));
        }

        public void _WriteDecimal(Decimal val)
        {
            _WriteStr(val.ToString(CInfo));
        }

        public void _WriteStr(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                _WriteCardinal(0);
                return;
            }

            byte[] buffer = System.Text.Encoding.Unicode.GetBytes(str);
            _WriteCardinal((UInt32)buffer.Length);
            OutputStream.Write(buffer, 0, buffer.Length);
        }

        public void _WriteDate(DateTime val)
        {
            _WriteWord((UInt16)val.Year);
            _WriteByte((byte)val.Month);
            _WriteByte((byte)val.Day);
        }

        public void _WriteTime(DateTime val)
        {
            _WriteByte((byte)val.Hour);
            _WriteByte((byte)val.Minute);
            _WriteByte((byte)val.Second);
            _WriteWord((UInt16)val.Millisecond);
        }

        public void _WriteDateTime(DateTime val)
        {
            _WriteDate(val);
            _WriteTime(val);
        }

        public void _WriteData(byte[] val)
        {
            _WriteCardinal((UInt32)val.Length);
            OutputStream.Write(val, 0, val.Length);
        }

        public void _WriteArray(UInt32 count)
        {
            _WriteCardinal(count);
        }

        public void Call()
        {
            UInt32 error;

            try
            {
                UInt32 len = (UInt32)OutputStream.Length;
                OutputStream.Seek(0, SeekOrigin.Begin);
                OutputStream.Write(BitConverter.GetBytes(len), 0, sizeof(UInt32));

                IntPtr res = System.Runtime.InteropServices.Marshal.AllocHGlobal((int)len);
                System.Runtime.InteropServices.Marshal.Copy(OutputStream.ToArray(), 0, res, (int)len);

                res = Parent._Call(res);

                byte[] buffer = new byte[sizeof(UInt32)];
                System.Runtime.InteropServices.Marshal.Copy(res, buffer, 0, sizeof(UInt32));
                UInt32 inputBufferSize = BitConverter.ToUInt32(buffer, 0);

                InputBuffer = new byte[inputBufferSize];
                System.Runtime.InteropServices.Marshal.Copy(res, InputBuffer, 0, (int)inputBufferSize);

                Parent._CallFree(res);

                InputPos = sizeof(UInt32);

                error = _ReadCardinal();
            }
            catch
            {
                if (IgnoreErrors)
                {
                    return;
                }

                throw;
            }

            //Bez chyby.
            if (IgnoreErrors || error == 0)
            {
                return;
            }

            string text = _ReadStr();
            throw new MonS3APIException((MonS3APIExceptionType)error, text);
        }
    }
}