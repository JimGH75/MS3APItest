unit MonS3APIUnit;

interface

uses
  SysUtils, Classes, Windows, IniFiles, ActiveX, Contnrs, DateUtils;

type
  //Typ chyby pøi práci s MonS3API.
  TMonS3APIExceptionType = (
    //Neznámá chyba.
    MonS3APIExceptionType_Unknown = 1,
    //Neznámá funkce.
    MonS3APIExceptionType_UnknownFunction = 2,
    //Neošetøená chyba databáze.
    MonS3APIExceptionType_UnknownDBError = 3,
    //Soubor neexistuje.
    MonS3APIExceptionType_FileNotFound = 4,
    //Název aplikace nebyl vyplnìn nebo obsahuje neplatné znaky.
    MonS3APIExceptionType_AppName = 5,
    //Nelze naèíst DLL knihovnu.
    MonS3APIExceptionType_LoadDll = 6,
    //Knihovna DLL nebyla naètena.
    MonS3APIExceptionType_DLLNotLoaded = 7,
    //Cesta k datùm nebyla vyplnìna nebo není platná.
    MonS3APIExceptionType_DataPath = 8,
    //Nejprve je potøeba se pøihlásit k datùm.
    MonS3APIExceptionType_NotConnected = 9,
    //Existuje otevøené pøipojení k datùm.
    MonS3APIExceptionType_AlreadyConnected = 10,
    //Je potøeba aktualizovat klienta na novìjší verzi.
    MonS3APIExceptionType_OldClient = 11,
    //Je potøeba aktualizovat program MoneyS3.
    MonS3APIExceptionType_OldAPI = 12,
    //Otevíraná databáze vyžaduje ke svému zobrazení novìjší verzi MonS3API.
    MonS3APIExceptionType_DBNewer = 13,
    //Otevíraná databáze vyžaduje ke svému zobrazení starší verzi MonS3API.
    MonS3APIExceptionType_DBElder = 14,
    //Uživatel nebyl pøihlášen.
    MonS3APIExceptionType_NotLogged = 15,
    //Nelze použít Hash SHA256.
    MonS3APIExceptionType_SHA256 = 16,
    //Uživatel je již pøihlášen.
    MonS3APIExceptionType_AlreadyLogged = 17,
    //Agenda nebyla vybrána.
    MonS3APIExceptionType_AgendaNotSelect = 18,
    //Transakce nebyla spuštìna.
    MonS3APIExceptionType_NoTransaction = 19,
    //Transakce je již spuštìna.
    MonS3APIExceptionType_TransactionAlreadyExists = 20,
    //Vypršel èasový timeout.
    MonS3APIExceptionType_Timeout = 21,
    //Tabulka nebyla nalezena.
    MonS3APIExceptionType_TableNotFound = 22,
    //Sloupec tabulky nebyl nalezen.
    MonS3APIExceptionType_ColNotFound = 23,
    //Pro požadovanou operaci nemáte potøebné oprávnìní.
    MonS3APIExceptionType_NoRights = 24,
    //Pro funkci GetRows je potøeba naplnit list what.
    MonS3APIExceptionType_SelectWhat = 25,
    //Funkce DeleteRows vyžaduje podmínku. Pro smazání všeho použijte DeleteAll.
    MonS3APIExceptionType_DeleteAll = 26,
    //Specifická chyba pro BFBase .dat tabulky. Pokud je "zamknut" záznam, tak nelze volat UpdateRows nebo DeleteRows.
    //(Napøíklad pokud má jiný uživatel otevøenou fakturu, tak pokud se zavolá úprava nebo smazání té faktury.)
    //SQLko tento problém nemá a lze "uzamèené" záznamy editovat.
    MonS3APIExceptionType_BFWriteLock = 27
  );

  //Informace o pøihlášení k datùm.
  TMonS3APILoginInformation = (
    //Neexistuje žádný uživatel a nebo existuje uživatel, ale nemá vyplnìné heslo.
    MonS3APILoginInformation_WithoutPassword = 1,
    //Lze se pøihlásit pouze pomocí windows jména, ale to tento uživatel nesplòuje.
    MonS3APILoginInformation_WindowsNO = 2,
    //Lze se pøihlásit jen pomocí windows jména a to tento uživatel splòuje.
    MonS3APILoginInformation_WindowsYES = 3,
    //Lze se pøihlásit pomocí windows jména a nebo na jiného uživatele pomocí hesla.
    MonS3APILoginInformation_WindowsPassword = 4,
    //Lze se pøihlásit pouze pomocí hesla.
    MonS3APILoginInformation_Password = 5
  );

  //Právo na pøístup.
  TMonS3APIRight = (
    //Není pøístup. Nelze naèíst žádné údaje.
    MonS3APIRight_None = 0,
    //Vrátí pouze základní údaje. Nelze dále èíst.
    MonS3APIRight_InfoOnly = 1,
    //Pouze ètení.
    MonS3APIRight_ReadOnly = 2,
    //Ètení i zápis.
    MonS3APIRight_ReadWrite = 3
  );

  //Datový typ sloupeèku.
  TMonS3APIColumnDataType = (
    MonS3APIColumnDataType_ID = 1,
    MonS3APIColumnDataType_String = 2,
    MonS3APIColumnDataType_Int = 3,
    MonS3APIColumnDataType_Int64 = 4,
    MonS3APIColumnDataType_Decimal = 5,
    MonS3APIColumnDataType_Boolean = 6,
    MonS3APIColumnDataType_Date = 7,
    MonS3APIColumnDataType_Time = 8,
    MonS3APIColumnDataType_DateTime = 9,
    MonS3APIColumnDataType_GUID = 10,
    MonS3APIColumnDataType_Blob = 11
  );

  //Typ podmínky.
  TMonS3APIDataWhereType = (
    //Otevøít závorku.
    MonS3APIDataWhereType_BracketStart = 1,
    //Zavøít závorku.
    MonS3APIDataWhereType_BracketEnd = 2,
    //Operátor AND.
    MonS3APIDataWhereType_OperatorAND = 3,
    //Operátor OR.
    MonS3APIDataWhereType_OperatorOR = 4,
    //Položka podmínky.
    MonS3APIDataWhereType_Where = 5
  );

  //Operátor podmínky.
  TMonS3APIDataWhereOperator = (
    //Rovná se.
    MonS3APIDataWhereOperator_Equals = 1,
    //Nerovná se.
    MonS3APIDataWhereOperator_NotEquals = 2,
    //Menší než. (Pouze èíselné hodnoty.)
    MonS3APIDataWhereOperator_LessThan = 3,
    //Menší rovno. (Pouze èíselné hodnoty.)
    MonS3APIDataWhereOperator_LessEq = 4,
    //Vìtší než. (Pouze èíselné hodnoty.)
    MonS3APIDataWhereOperator_GreaterThan = 5,
    //Vìtší rovno. (Pouze èíselné hodnoty.)
    MonS3APIDataWhereOperator_GreaterEq = 6,
    //Zaèíná. (Pouze øetìzce.)
    MonS3APIDataWhereOperator_BeginsWith = 7,
    //Nezaèíná. (Pouze øetìzce.)
    MonS3APIDataWhereOperator_NotBeginsWith = 8,
    //Konèí. (Pouze øetìzce.)
    MonS3APIDataWhereOperator_EndsWith = 9,
    //Nekonèí. (Pouze pro øetìzce).
    MonS3APIDataWhereOperator_NotEndsWith = 10,
    //Obsahuje. (Pouze øetìzce.)
    MonS3APIDataWhereOperator_Contains = 11,
    //Neobsahuje. (Pouze øetìzce.)
    MonS3APIDataWhereOperator_NotContains = 12,
    //Je ve výètu hodnot.
    MonS3APIDataWhereOperator_InValues = 13,
    //Není ve výètu hodnot.
    MonS3APIDataWhereOperator_NotInValues = 14,
    //Je mezi hodnotami.
    MonS3APIDataWhereOperator_Between = 15,
    //Není mezi hodnotami.
    MonS3APIDataWhereOperator_NotBetween = 16
  );

  //Typ øazení.
  TMonS3APIDataSortType = (
    //Vzestupnì. Tedy od nejmenšího po nejvìtší. A-Z.
    MonS3APIDataSortType_Ascending = 1,
    //Sestupnì. Tedy od nejvìtšího po nejmenší. Z-A.
    MonS3APIDataSortType_Descending = 2
  );

  //Typ knihovny.
  TMonS3APIDLLType = (
    //Základní knihovna.
    MonS3API = 1,
    //Knihovna jen pro ètení dat.
    MonS3Reader = 2
  );

  //Chyba pøi práci s MonS3API. Message obsahuje èitelný pøeklad typu ExceptionType.
  TMonS3APIException = class(Exception)
  public
    //Typ chyby pøi práci s MonS3API. Message obsahuje èitelný pøeklad tohoto typu.
    ExceptionType: TMonS3APIExceptionType;
    //Detailní informace o chybì. Nemusí být vyplnìno.
    Detail: string;

    //Vytvoøení chyby pøi práci s MonS3API.
    constructor Create(AExceptionType: TMonS3APIExceptionType; const ADetail: string); reintroduce;
  end;

var
  //Eventa na zmìnu textù chyby.
  MonS3APIException_OnGetMessage: TNotifyEvent = nil;

type
  //Kvùli delphi 7.
  TBytes = array of byte;

  //Agenda.
  TMonS3APIAgenda = class
  private

    //Zobrazovaný název. Pokud není vyplnìn, tak je shodný s hodnotou Name.
    FVisibleName: string;
    //Obchodní název.
    FName: string;
    //IÈO.
    FIC: string;
    //Právo na pøístup k agendì.
    FRight: TMonS3APIRight;
    //Koncovka složky agendy (001, 002, sju, spa, spb).
    FExt: string;
    //Zda se jedná o demo agendu.
    //Ty jsou identifikovány podle koncovky názvu složky.
    //Avšak jsou uživatelé, kteøí demo agendu vezmou, promažou a pak používají.
    FIsDemo: boolean;
    //Poznámka. Na záložce nastavení agendy je to položka "Jiné údaje".
    FPoznamka: string;

  public

    constructor Create(const AVisibleName: string; const AName: string; const AIC: string; ARight: TMonS3APIRight; const AExt: string; AIsDemo: boolean; const APoznamka: string);

    //Zobrazovaný název. Pokud není vyplnìn, tak je shodný s hodnotou Name.
    property VisibleName: string read FVisibleName;
    //Obchodní název.
    property Name: string read FName;
    //IÈO.
    property IC: string read FIC;
    //Právo na pøístup k agendì.
    property Right: TMonS3APIRight read FRight;
    //Koncovka složky agendy (001, 002, sju, spa, spb).
    property Ext: string read FExt;
    //Zda se jedná o demo agendu.
    //Ty jsou identifikovány podle koncovky názvu složky.
    //Avšak jsou uživatelé, kteøí demo agendu vezmou, promažou a pak používají.
    property IsDemo: boolean read FIsDemo;
    //Poznámka. Na záložce nastavení agendy je to položka "Jiné údaje".
    property Poznamka: string read FPoznamka;

  end;

  //List agend.
  TMonS3APIAgendaList = class(TObjectList)
  protected
    function MyGetItem(AIndex: Integer): TMonS3APIAgenda;
    procedure MySetItem(AIndex: integer; AValue: TMonS3APIAgenda);
  public
    //Položky listu.
    property Items[Index: Integer]: TMonS3APIAgenda read MyGetItem write MySetItem; default;
    //Vrátí agendu podle koncovky.
    function GetByExt(AExt: string): TMonS3APIAgenda;
    //Vrátí první agendu, která není demo.
    function GetFirstNoDemo: TMonS3APIAgenda;
  end;

  //Rok agendy.
  TMonS3APIYear = class
  private
    //Èíselné oznaèení roku.
    FYear: integer;
    //Datum od.
    FDateFrom: TDateTime;
    //Datum do.
    FDateTo: TDateTime;
    //Práva na pøístup.
    FRight: TMonS3APIRight;
    //Koncovka složky roku.
    FExt: string;

  public

    constructor Create(AYear: integer; ADateFrom: TDateTime; ADateTo: TDateTime; ARight: TMonS3APIRight; const AExt: string);

    //Èíselné oznaèení roku.
    property Year: integer read FYear;
    //Datum od.
    property DateFrom: TDateTime read FDateFrom;
    //Datum do.
    property DateTo: TDateTime read FDateTo;
    //Práva na pøístup.
    property Right: TMonS3APIRight read FRight;
    //Koncovka složky roku.
    property Ext: string read FExt;

  end;

  //List rokù agendy - seøazený dle datumù.
  TMonS3APIYearList = class(TObjectList)
  protected
    function MyGetItem(AIndex: Integer): TMonS3APIYear;
    procedure MySetItem(AIndex: integer; AValue: TMonS3APIYear);
  public
    //Položky listu.
    property Items[Index: Integer]: TMonS3APIYear read MyGetItem write MySetItem; default;
    //Vrátí rok podle data.
    function GetByDate(ADate: TDateTime): TMonS3APIYear;
    //Vrátí rok podle aktuálního data.
    function GetActual: TMonS3APIYear;
  end;

  //Tabulka.
  TMonS3APITable = class
  private

    //Název tabulky.
    FName: string;
    //Název tabulky - malými písmeny.
    FNameL: string;
    //Právo na tabulku.
    FRight: TMonS3APIRight;
    //Èásteèná práva na øádky tabulky.
    //ReadWrite - lze èíst a zapisovat vše (také podle toho, co dovolí Right pro celou tabulku)
    //ReadOnly - nìkteré øádky dat nelze mìnit (také podle toho, co dovolí Right pro celou tabulku), vznikne exception NoRights
    //None - nìkteré øádky nelze èíst - budou automaticky vyfiltrovány - tabulka se bude chovat, jako by v ní takové øádky nebyly
    FPartialRightRow: TMonS3APIRight;
    //Èásteèná práva na sloupeèky tabulky.
    //ReadWrite - lze èíst a zapisovat vše (také podle toho, co dovolí Right pro celou tabulku)
    //ReadOnly - nìkteré sloupeèky dat nelze mìnit (také podle toho, co dovolí Right pro celou tabulku) - pokud se provede Update, tak se tento sloupec neaktualizuje, v pøípadì pøidání záznamu jeho hodnota bude prázdná
    //None - sloupeèek bude mít vždy prázdnou hodnotu, jako by nebyl vyplnìn, dále platí, že nelze hodnota nastavit ani mìnit jako v pøípadì ReadOnly
    FPartialRightCol: TMonS3APIRight;

  public

    constructor Create(const AName: string; ARight: TMonS3APIRight; APartialRightRow: TMonS3APIRight; APartialRightCol: TMonS3APIRight);

    //Název tabulky.
    property Name: string read FName;
    //Název tabulky - malými písmeny.
    property NameL: string read FNameL;
    //Právo na tabulku.
    property Right: TMonS3APIRight read FRight;
    //Èásteèná práva na øádky tabulky.
    //ReadWrite - lze èíst a zapisovat vše (také podle toho, co dovolí Right pro celou tabulku)
    //ReadOnly - nìkteré øádky dat nelze mìnit (také podle toho, co dovolí Right pro celou tabulku), vznikne exception NoRights
    //None - nìkteré øádky nelze èíst - budou automaticky vyfiltrovány - tabulka se bude chovat, jako by v ní takové øádky nebyly
    property PartialRightRow: TMonS3APIRight read FPartialRightRow;
    //Èásteèná práva na sloupeèky tabulky.
    //ReadWrite - lze èíst a zapisovat vše (také podle toho, co dovolí Right pro celou tabulku)
    //ReadOnly - nìkteré sloupeèky dat nelze mìnit (také podle toho, co dovolí Right pro celou tabulku) - pokud se provede Update, tak se tento sloupec neaktualizuje, v pøípadì pøidání záznamu jeho hodnota bude prázdná
    //None - sloupeèek bude mít vždy prázdnou hodnotu, jako by nebyl vyplnìn, dále platí, že nelze hodnota nastavit ani mìnit jako v pøípadì ReadOnly
    property PartialRightCol: TMonS3APIRight read FPartialRightCol;

  end;

  //List všech tabulek.
  TMonS3APITableList = class(TObjectList)
  protected
    function MyGetItem(AIndex: Integer): TMonS3APITable;
    procedure MySetItem(AIndex: integer; AValue: TMonS3APITable);
  public
    //Položky listu.
    property Items[Index: Integer]: TMonS3APITable read MyGetItem write MySetItem; default;
    //Vrátí tabulku podle názvu.
    function GetByName(AName: string): TMonS3APITable;
  end;

  //Sloupeèek tabulky.
  TMonS3APITableColumn = class
  private
    //Název sloupeèku.
    FName: string;
    //Název sloupeèku - malými písmeny.
    FNameL: string;
    //Datový typ sloupeèku.
    FDataType: TMonS3APIColumnDataType;
    //Poèet znakù, pokud je typ String.
    FStringSize: integer;

  public

    constructor Create(const AName: string; ADataType: TMonS3APIColumnDataType; AStringSize: integer);

    //Název sloupeèku.
    property Name: string read FName;
    //Název sloupeèku - malými písmeny.
    property NameL: string read FNameL;
    //Datový typ sloupeèku.
    property DataType: TMonS3APIColumnDataType read FDataType;
    //Poèet znakù, pokud je typ String.
    property StringSize: integer read FStringSize;

  end;

  //Všechny sloupeèky tabulky.
  TMonS3APITableColumnList = class(TObjectList)
  protected
    function MyGetItem(AIndex: Integer): TMonS3APITableColumn;
    procedure MySetItem(AIndex: integer; AValue: TMonS3APITableColumn);
  public
    //Položky listu.
    property Items[Index: Integer]: TMonS3APITableColumn read MyGetItem write MySetItem; default;
    //Vrátí ID sloupeèek.
    function GetIDColumn: TMonS3APITableColumn;
    //Vrátí sloupeèek podle názvu.
    function GetByName(AName: string): TMonS3APITableColumn;
  end;

  //Typ promìnné.
  _TMonS3APIParserValType = (
    _MonS3APIParserValType_Empty = 1,
    _MonS3APIParserValType_Bool = 2,
    _MonS3APIParserValType_Byte = 3,
    _MonS3APIParserValType_Word = 4,
    _MonS3APIParserValType_Int = 5,
    _MonS3APIParserValType_Int64 = 6,
    _MonS3APIParserValType_Cardinal = 7,
    _MonS3APIParserValType_Ext = 8,
    _MonS3APIParserValType_Curr = 9,
    _MonS3APIParserValType_Str = 10,
    _MonS3APIParserValType_Date = 11,
    _MonS3APIParserValType_Time = 12,
    _MonS3APIParserValType_DateTime = 13,
    _MonS3APIParserValType_Data = 14,
    _MonS3APIParserValType_Array = 15
  );

  //Samotná komunikace s DLL knihovnou.
  _TMonS3APIParser = class
  private

    //Hlavní tøída obsluhující DLL knihovnu.
    FParent: TObject;
    //Zda se mají ignorovat chyby.
    FIgnoreErrors: boolean;
    //Nastavení pro desetinná èásla.
    FFormatSettings: TFormatSettings;
    //Buffer na posílání dat.
    FOutputBuffer: TBytes;
    //Aktuální pozice ve výstupním bufferu.
    FOutputPos: Cardinal;
    //Vstupní buffer.
    FInputBuffer: TBytes;
    //Aktuální pozice ve vstupním bufferu.
    FInputPos: Cardinal;

    function Skip(ASize: Cardinal): integer;
    procedure CheckCapacity(Cap: Cardinal);
    function GetBool: boolean;
    function GetByte: byte;
    function GetWord: word;
    function GetInt: LongInt; //Int32
    function GetInt64: Int64;
    function GetCardinal: Cardinal;
    function GetExt: extended;
    function GetStr: WideString; //string
    function GetDate: TDateTime;
    function GetTime: TDateTime;
    function GetDateTime: TDateTime;
    function GetData: TBytes;
    function GetArray: Cardinal;
    function _ReadBool: boolean;
    function _ReadByte: byte;
    function _ReadWord: word;
    function _ReadInt: LongInt; //Int32
    function _ReadInt64: Int64;
    function _ReadCardinal: Cardinal;
    function _ReadExt: extended;
    function _ReadStr: WideString; //string
    function _ReadDate: TDateTime;
    function _ReadTime: TDateTime;
    function _ReadDateTime: TDateTime;
    function _ReadData: TBytes;
    function _ReadArray: Cardinal;
    procedure AddEmpty;
    procedure AddBool(AVal: boolean);
    procedure AddByte(AVal: byte);
    procedure AddWord(AVal: word);
    procedure AddInt(AVal: LongInt); //Int32
    procedure AddInt64(AVal: Int64);
    procedure AddCardinal(AVal: Cardinal);
    procedure AddExt(AVal: extended);
    procedure AddCurr(AVal: currency);
    procedure AddStr(const AVal: WideString); //string
    procedure AddDate(AVal: TDateTime);
    procedure AddTime(AVal: TDateTime);
    procedure AddDateTime(AVal: TDateTime);
    procedure AddData(const AVal: TBytes);
    procedure AddArray(ACount: Cardinal);
    procedure _WriteBool(AVal: boolean);
    procedure _WriteByte(AVal: byte);
    procedure _WriteWord(AVal: word);
    procedure _WriteInt(AVal: LongInt); //Int32
    procedure _WriteInt64(AVal: Int64);
    procedure _WriteCardinal(AVal: Cardinal);
    procedure _WriteExt(AVal: extended);
    procedure _WriteCurr(AVal: currency);
    procedure _WriteStr(const AVal: WideString); //string
    procedure _WriteDate(AVal: TDateTime);
    procedure _WriteTime(AVal: TDateTime);
    procedure _WriteDateTime(AVal: TDateTime);
    procedure _WriteData(const AVal: TBytes);
    procedure _WriteArray(ACount: Cardinal);
    procedure Call;

  public
    constructor Create(AParent: TObject; const AClientGUID: TBytes; const AObjectGUID: TBytes; const AFunc: string; AIgnoreErrors: boolean);
  end;

  //Datová hodnota.
  //Nemùže to být record kvùli Delphi 7.
  //Uvolòuje se vždy v objektu, ve kterém se použije - podmínka, atd.
  TMonS3APIValue = class
  private
    //Typ hodnoty.
    FValType: _TMonS3APIParserValType;
    //Hodnota.
    FValExt: extended;
    //Stringová hodnota.
    FValStr: string;
    //Blob.
    FValData: TBytes;

    function GetAsInt: integer;
    procedure SetAsInt(value: integer);
    function GetAsInt64: Int64;
    procedure SetAsInt64(value: Int64);
    function GetAsExt: extended;
    procedure SetAsExt(value: extended);
    function GetAsString: string;
    procedure SetAsString(const value: string);
    function GetAsBool: boolean;
    procedure SetAsBool(value: boolean);
    function GetAsDate: TDateTime;
    procedure SetAsDate(value: TDateTime);
    function GetAsTime: TDateTime;
    procedure SetAsTime(value: TDateTime);
    function GetAsDateTime: TDateTime;
    procedure SetAsDateTime(value: TDateTime);
    function GetAsData: TBytes;
    procedure SetAsData(value: TBytes);

  public

    constructor Create;
    procedure Clear;

    function CreateCopy: TMonS3APIValue;
    procedure CopyFrom(AValue: TMonS3APIValue);

    //Typ hodnoty.
    property ValType: _TMonS3APIParserValType read FValType;

    property AsInt: integer read GetAsInt write SetAsInt;
    property AsInt64: Int64 read GetAsInt64 write SetAsInt64;
    property AsExt: extended read GetAsExt write SetAsExt;
    property AsString: string read GetAsString write SetAsString;
    property AsBool: boolean read GetAsBool write SetAsBool;
    property AsDate: TDateTime read GetAsDate write SetAsDate;
    property AsTime: TDateTime read GetAsTime write SetAsTime;
    property AsDateTime: TDateTime read GetAsDateTime write SetAsDateTime;
    property AsData: TBytes read GetAsData write SetAsData;

    class function CreateEmpty: TMonS3APIValue;
    class function CreateInt(value: integer): TMonS3APIValue;
    class function CreateInt64(value: Int64): TMonS3APIValue;
    class function CreateExt(value: extended): TMonS3APIValue;
    class function CreateString(const value: string): TMonS3APIValue;
    class function CreateBool(value: boolean): TMonS3APIValue;
    class function CreateDate(value: TDateTime): TMonS3APIValue;
    class function CreateTime(value: TDateTime): TMonS3APIValue;
    class function CreateDateTime(value: TDateTime): TMonS3APIValue;
    class function CreateData(value: TBytes): TMonS3APIValue;

    function ToSQLString: string;
  end;

  //List datových hodnot.
  TMonS3APIValueList = class(TObjectList)
  protected
    function MyGetItem(AIndex: Integer): TMonS3APIValue;
    procedure MySetItem(AIndex: integer; AValue: TMonS3APIValue);
  public
    //Položky listu.
    property Items[Index: Integer]: TMonS3APIValue read MyGetItem write MySetItem; default;
    //Vytvoøí kopii.
    function CreateCopy: TMonS3APIValueList;
    //Naplní kopií.
    procedure CopyFrom(AList: TMonS3APIValueList);
  end;

  //Sloupeèek, který se bude naèítat SELECTem.
  TMonS3APIDataWhat = class
  private
    //Sloupeèek.
    FValue: string;
  public
    constructor Create(const AValue: string);

    //Sloupeèek.
    property Value: string read FValue;
  end;

  //Sloupeèky, které se budou naèítat SELECTem.
  TMonS3APIDataWhatList = class(TObjectList)
  protected
    function MyGetItem(AIndex: Integer): TMonS3APIDataWhat;
    procedure MySetItem(AIndex: integer; AValue: TMonS3APIDataWhat);
  public
    //Položky listu.
    property Items[Index: Integer]: TMonS3APIDataWhat read MyGetItem write MySetItem; default;
    //Pøidá vše.
    procedure AddAll;
  end;

  //Položka podmínky.
  TMonS3APIDataWhere = class
  private
    //Typ.
    FType: TMonS3APIDataWhereType;
    //Sloupec podmínky.
    FColumn: string;
    //Operátor podmínky.
    FOperator: TMonS3APIDataWhereOperator;
    //Hodnoty pomínky.
    FValues: TMonS3APIValueList;

  public

    constructor Create(AType: TMonS3APIDataWhereType); overload;
    constructor Create(const AColumn: string; AOperator: TMonS3APIDataWhereOperator; AValue: TMonS3APIValue); overload;
    constructor Create(const AColumn: string; AOperator: TMonS3APIDataWhereOperator; const AValues: Array of TMonS3APIValue); overload;
    constructor Create(const AColumn: string; AOperator: TMonS3APIDataWhereOperator; AValues: TMonS3APIValueList); overload;
    destructor Destroy; override;

    //Typ.
    property TType: TMonS3APIDataWhereType read FType;
    //Sloupec podmínky.
    property Column: string read FColumn;
    //Operátor podmínky.
    property OOperator: TMonS3APIDataWhereOperator read FOperator;
    //Hodnoty pomínky.
    property Values: TMonS3APIValueList read FValues;
  end;

  //Podmínka.
  TMonS3APIDataWhereList = class(TObjectList)
  protected
    function MyGetItem(AIndex: Integer): TMonS3APIDataWhere;
    procedure MySetItem(AIndex: integer; AValue: TMonS3APIDataWhere);
  public
    //Položky listu.
    property Items[Index: Integer]: TMonS3APIDataWhere read MyGetItem write MySetItem; default;
    //Otevøít závorku.
    procedure AddBracketStart;
    //Zavøít závorku.
    procedure AddBracketEnd;
    //Operátor AND.
    procedure AddOperatorAND;
    //Operátor OR.
    procedure AddOperatorOR;
    //Položka podmínky.
    procedure AddWhere(const AColumn: string; AOperator: TMonS3APIDataWhereOperator; AValue: TMonS3APIValue); overload;
    //Položka podmínky.
    procedure AddWhere(const AColumn: string; AOperator: TMonS3APIDataWhereOperator; const AValues: Array of TMonS3APIValue); overload;
    //Položka podmínky.
    procedure AddWhere(const AColumn: string; AOperator: TMonS3APIDataWhereOperator; AValues: TMonS3APIValueList); overload;
    //Jen pro debug. Vypsání SQL pøíkazu.
    function GetSQLCommandString: string;
  end;

  //Øazení SELECTu.
  TMonS3APIDataSort = class
  private
    //Sloupec.
    FColumn: string;
    //Typ øazení.
    FType: TMonS3APIDataSortType;
  public

    constructor Create(const AColumn: string; AType: TMonS3APIDataSortType);

    //Sloupec.
    property Column: string read FColumn;
    //Typ øazení.
    property TType: TMonS3APIDataSortType read FType;
  end;

  //Øazení SELECTu.
  TMonS3APIDataSortList = class(TObjectList)
  protected
    function MyGetItem(AIndex: Integer): TMonS3APIDataSort;
    procedure MySetItem(AIndex: integer; AValue: TMonS3APIDataSort);
  public
    //Položky listu.
    property Items[Index: Integer]: TMonS3APIDataSort read MyGetItem write MySetItem; default;
    //Pøidá øazení.
    procedure AddSort(const AColumn: string; AType: TMonS3APIDataSortType);
  end;

  //Hodnota pro pøidání nebo úpravu øádku.
  TMonS3APIDataValue = class
  private
    //Název sloupce.
    FColumn: string;
    //Hodnota.
    FValue: TMonS3APIValue;
  public
    constructor Create(const AColumn: string; AValue: TMonS3APIValue);
    destructor Destroy; override;

    //Název sloupce.
    property Column: string read FColumn;
    //Hodnota.
    property Value: TMonS3APIValue read FValue;
  end;

  //Data pro pøidání nebo úpravu øádku.
  TMonS3APIDataValueList = class(TObjectList)
  protected
    function MyGetItem(AIndex: Integer): TMonS3APIDataValue;
    procedure MySetItem(AIndex: integer; AValue: TMonS3APIDataValue);
  public
    //Položky listu.
    property Items[Index: Integer]: TMonS3APIDataValue read MyGetItem write MySetItem; default;
    //Pøidá hodnotu.
    procedure AddValue(const AColumn: string; AValue: TMonS3APIValue);
  end;

  //Výsledek SELECTu.
  TMonS3APIDataSelectResult = class
  private
    //Odkaz na tøídu, která tuhle vytvoøila.
    FDataBase: TObject;
    //Tøída obsahující data.
    FParser: _TMonS3APIParser;
    //Pozice v datech.
    FPosition: integer;
    //Interní seznam sloupeèkù.
    FColumns: TStringList;
    //Rozparsovaný øádek.
    FValues: TMonS3APIValueList;
    //Poèet získaných záznamù.
    FCount: integer;
    //Prázdná hodnota.
    FEmptyValue: TMonS3APIValue;

    //Pøipojí se na parser.
    procedure ConnectToParser(ADataBase: TObject; AParser: _TMonS3APIParser);
    //Pøeète hodnoty z parseru.
    procedure ReadParser;

  public

    constructor Create;
    destructor Destroy; override;

    //Poèet získaných záznamù.
    property Count: integer read FCount;

    //Skoèí na další záznam.
    function Next: boolean;
    //Vrátí hodnotu sloupeèku podle jména.
    function GetColByName(AColumn: string): TMonS3APIValue;
  end;

  //Informace o pøihlášeném uživateli.
  TMonS3APIUserInfo = class
  private
    //GUID uživatele. Tento údaj bude pravdìpodobnì dostupný od verze Money 19.800 nebo pozdìji.
    //Jakmile bude dostupný, upøednostnìte tuto hodnotu.
    FGUID: string;
    //Hodnota ID_Pristup z tabulky PrisUziv.
    //Jedná se o IDèko (increment) uživatele.
    //Doporuèuji použít místo téhle hodnoty GUID.
    FID: integer;
    //Jméno uživatele.
    FJmeno: string;
    //Jméno konfigurace.
    FConfigName: string;
    //Poznámka.
    FPoznamka: string;
  public

    constructor Create(const AGUID: string; AID: integer; const AJmeno: string; const AConfigName: string; const APoznamka: string);

    //GUID uživatele. Tento údaj bude pravdìpodobnì dostupný od verze Money 19.800 nebo pozdìji.
    //Jakmile bude dostupný, upøednostnìte tuto hodnotu.
    property GUID: string read FGUID;
    //Hodnota ID_Pristup z tabulky PrisUziv.
    //Jedná se o IDèko (increment) uživatele.
    //Doporuèuji použít místo téhle hodnoty GUID.
    property ID: integer read FID;
    //Jméno uživatele.
    property Jmeno: string read FJmeno;
    //Jméno konfigurace.
    property ConfigName: string read FConfigName;
    //Poznámka.
    property Poznamka: string read FPoznamka;
  end;

  //Právo pøihlášeného uživatele.
  TMonS3APIUserRight = class
  private
    //Název práva.
    FRightName: string;
    //Název práva - malými písmeny.
    FRightNameL: string;
    //Hodnota práva.
    FRightValue: string;
  public

    constructor Create(const ARightName: string; const ARightValue: string);

    //Název práva.
    property RightName: string read FRightName;
    //Název práva - malými písmeny.
    property RightNameL: string read FRightNameL;
    //Hodnota práva.
    property RightValue: string read FRightValue;
  end;

  //Všechna práva pøihlášeného uživatele.
  TMonS3APIUserRightList = class(TObjectList)
  protected
    function MyGetItem(AIndex: Integer): TMonS3APIUserRight;
    procedure MySetItem(AIndex: integer; AValue: TMonS3APIUserRight);
  public
    //Položky listu.
    property Items[Index: Integer]: TMonS3APIUserRight read MyGetItem write MySetItem; default;
    //Vrátí právo podle názvu.
    function GetByName(ARightName: string): TMonS3APIUserRight;
  end;

  //Tøída spoleèných funkcí pro práci se spoleènými daty a daty agendy.
  TMonS3APIDataBase = class
  private

    //Hlavní tøída obsluhující DLL knihovnu.
    FParent: TObject;
    //GUID tohoto objektu.
    FObjectGUID: TBytes;

    //Nastavení:
    //Pokud je true, tak pøi nenalezeném sloupeèku hodí exception.
    //Pokud je false (default), tak vrátí prázdnou hodnotu.
    FSettingErrorIfColNotFound: boolean;

    function GetSettingsLockTimeout: integer;
    procedure SetSettingsLockTimeout(AValue: integer);

    //Vytvoøí komunikaci s DLL knihovnou.
    function _CreateParser(const AFunc: string; AIgnoreErrors: boolean = false): _TMonS3APIParser;
    //Pøidá vybrané sloupeèky v selectu do parseru.
    procedure _AddWhat(AParser: _TMonS3APIParser; AWhat: TMonS3APIDataWhatList);
    //Pøidá podmínky do parseru.
    procedure _AddWhere(AParser: _TMonS3APIParser; AWhere: TMonS3APIDataWhereList);
    //Pøidá øazení do parseru.
    procedure _AddSort(AParser: _TMonS3APIParser; ASort: TMonS3APIDataSortList);
    //Pøidá hodnoty do parseru.
    procedure _AddDataValues(AParser: _TMonS3APIParser; AValues: TMonS3APIDataValueList);
    //Pøidá hodnoty do parseru.
    procedure _AddValues(AParser: _TMonS3APIParser; AValues: TMonS3APIValueList);
    //Pøidá hodnotu do parseru.
    procedure _AddValue(AParser: _TMonS3APIParser; AValue: TMonS3APIValue);
    //Pøeète info o agendì.
    function _ParseAgendaInfo(AParser: _TMonS3APIParser): TMonS3APIAgenda;

  public

    constructor Create(AParent: TObject; const AObjectGUID: TBytes);
    destructor Destroy; override;

    //Nastavení:
    //Pokud je true, tak pøi nenalezeném sloupeèku hodí exception.
    //Pokud je false (default), tak vrátí prázdnou hodnotu.
    property SettingErrorIfColNotFound: boolean read FSettingErrorIfColNotFound write FSettingErrorIfColNotFound;
    //Poèet milisekund po které se bude èekat na zamknutou tabulku.
    //Po vypršení nastane exception.
    //Default je -1, což je èekání do nekoneèna.
    property SettingsLockTimeout: integer read GetSettingsLockTimeout write SetSettingsLockTimeout;

    //Pøipojí se k datùm Money S3.
    //Pokud není nìjakou dobu potøeba pøistupovat k datùm, bylo by dobré se od nich opìt odpojit.
    procedure ConnectData;
    //Odpojí se od dat.
    procedure DisconnectData;
    //Vrací seznam všech tabulek.
    procedure GetTableList(AOutput: TMonS3APITableList);
    //Vrátí seznam všech sloupeèkù.
    procedure GetTableColumns(const ATable: string; AOutput: TMonS3APITableColumnList);
    //Spustí transakci.
    //Transakce by mìla bìžet pouze po nezbytnì nutnou dobu.
    //Nelze použít s knihovnou MonS3Reader
    procedure TransactionStart;
    //Potvrdí zmìny a ukonèí transakci.
    //Pokud není aktivní transakce, tak dojde k chybì.
    //Nelze použít s knihovnou MonS3Reader
    procedure TransactionCommit;
    //Potvrdí zmìny a ukonèí transakci.
    //Pokud není aktivní transakce, tak se funkce ignoruje.
    //Nelze použít s knihovnou MonS3Reader
    procedure TransactionRollback;
    //Vrací, zda existuje øádek.
    //Pokud je podmínka vyplnìna, tak zda alespoò jeden øádek splòuje podmínku.
    //Pokud pomínka není vyplnìna, tak zda není tabulka prázdná.
    function GetRowExists(const ATable: string; AWhere: TMonS3APIDataWhereList = nil): boolean;
    //Vrací poèet øádkù.
    //Pokud je podmínka vyplnìna, tak poèet øádkù splòující podmínku.
    //Pokud pomínka není vyplnìna, tak poèet všech øádkù tabulky.
    function GetRowCount(const ATable: string; AWhere: TMonS3APIDataWhereList = nil): integer;
    //Vrací øádky.
    //Pokud je podmínka vyplnìna, tak všechny splòující podmínku.
    //Pokud pomínka není vyplnìna, tak všechny z tabulky.
    procedure GetRows(const ATable: string; AWhat: TMonS3APIDataWhatList; AOutput: TMonS3APIDataSelectResult; AWhere: TMonS3APIDataWhereList = nil; ASort: TMonS3APIDataSortList = nil; AFrom: integer = 0; ALimit: integer = -1);
    //Pøidá øádek.
    //Pro pøidání více øádkù doporuèuji použít transakci.
    //Nelze použít s knihovnou MonS3Reader
    function AddRow(const ATable: string; AValues: TMonS3APIDataValueList): integer;
    //Upraví øádky.
    //Pokud je podmínka vyplnìna, tak všechny splòující podmínku.
    //Pokud pomínka není vyplnìna, tak všechny v tabulce.
    //Nelze použít s knihovnou MonS3Reader
    function UpdateRows(const ATable: string; AValues: TMonS3APIDataValueList; AWhere: TMonS3APIDataWhereList = nil): integer;
    //Smaže øádky podle podmínky.
    //Nelze použít s knihovnou MonS3Reader
    function DeleteRows(const ATable: string; AWhere: TMonS3APIDataWhereList): integer;
    //Smaže všechny øádky v tabulce.
    //Nelze použít s knihovnou MonS3Reader
    function DeleteAll(const ATable: string): integer;
  end;

  //Tøída pro práci se spoleènými daty.
  TMonS3APIDataProgram = class(TMonS3APIDataBase)
  public

    //Vrátí informaci o možnostech pøihlášení uživatele.
    function GetLoginInformation: TMonS3APILoginInformation;
    //Zobrazí logovací dialog pro uživatele.
    function LoginDialog(AParentForm: HWND): string;
    //Pøeloží heslo na hash použitelný v dalších funkcích.
    function TranslatePassword(const APassword: string): string;
    //Pøihlásí se do dat.
    //
    //Heslo se nevyplòuje v pøípadech:
    //  WithoutPassword
    //  WindowsYES
    //
    //Heslo se nemusí vyplòovat v pøípadech:
    //  WindowsPassword
    //
    //Heslo se musí vyplòovat v pøípadech:
    //  Password
    //
    //Nelze se pøihlásit:
    //  WindowsNO
    //
    function Login(const APassword: string): boolean;
    //Odhlásí uživatele.
    procedure Logout;
    //Vrátí seznam agend.
    procedure GetListAgend(AOutput: TMonS3APIAgendaList);
    //Vrátí info o konkrétní agendì, bez naètení všech agend.
    function GetAgendaInfo(const AAgendaExt: string): TMonS3APIAgenda;
    //Vrátí informace o pøihlášeném uživateli.
    function GetUserInfo: TMonS3APIUserInfo;
    //Vrátí seznam práv uživatele.
    procedure GetUserRights(AOutput: TMonS3APIUserRightList);
  end;

  //Tøída pro práci s daty agendy.
  TMonS3APIDataAgenda = class(TMonS3APIDataBase)
  public
    //Nastaví agendu k pøipojení.
    procedure SetAgenda(const AAgendaExt: string);
    //Vrátí info o aktuálnì nastavené agendì.
    function GetAgendaInfo: TMonS3APIAgenda;
    //Vrátí všechny roky v agendì seøazené dle data.
    procedure GetYearList(AOutput: TMonS3APIYearList);
  end;

  //Tøída pro práci s daty dokumentù.
  TMonS3APIDataDokumenty = class(TMonS3APIDataBase)
  public
    //Nastaví agendu k pøipojení.
    procedure SetAgenda(const AAgendaExt: string);
  end;

  //Hlavní tøída pro práci s daty Money S3.
  TMonS3APIDataMain = class
  private

    //Handle knihovny.
    FHandleDLL: HMODULE;
    //Identifikátor tohoto klienta.
    FClientGUID: TBytes;

    //Funkce na komunikaci s dll knihovnou.
    FDLLFunc: function(input: Pointer): Pointer; stdcall;
    //Funkce na komunikaci s 32bit dll knihovnou - uvolnìní pamìti.
    FDLLFuncFree: procedure(input: Pointer); stdcall;

    //Vytvoøí komunikaci s DLL knihovnou.
    function _CreateParser(const AFunc: string; AIgnoreErrors: boolean = false): _TMonS3APIParser; overload;
    //Vytvoøí komunikaci s DLL knihovnou pro objekt.
    function _CreateParser(const AFunc: string; const AObjectGUID: TBytes; AIgnoreErrors: boolean = false): _TMonS3APIParser; overload;
    //Vnitøní funkce pro komunikaci s DLL knihovnou.
    function _Call(AInp: Pointer): Pointer;
    //Vnitøní funkce pro komunikaci s DLL knihovnou - uvolnìní pamìti.
    procedure _CallFree(AInp: Pointer);

  public

    destructor Destroy; override;

    //Hledá knihovnu MonS3API.dll
    //Vrací celou cestu k MonS3API.dll a nebo prázdný string.
    //Podle parametru aMonS3APIDLLType se urèí jestli pùjde o MonS3API nebo MonS3Reader.
    function FindDLL(aMonS3APIDLLType: TMonS3APIDLLType = MonS3API): string;
    //Pøipojí se ke knihovnì MonS3API.dll.
    //Podle parametru aMonS3APIDLLType se urèí jestli pùjde o MonS3API nebo MonS3Reader.
    procedure LoadDLL(const AFullFileName: string; const AAppName: string; aMonS3APIDLLType: TMonS3APIDLLType = MonS3API);
    //Odpojí se od knihovny MonS3API.dll.
    procedure UnLoadDLL;
    //Hledá složku s daty Money S3.
    function FindData: string;
    //Nastaví cestu k datùm.
    procedure SetDataPath(const AFullPath: string);
    //Získá instanci pro práci se spoleènými daty. Jedna instance smí být použita souèasnì pouze v jednom vláknì.
    function GetProgramInstance: TMonS3APIDataProgram;
    //Získá instanci pro práci s daty agendy. Jedna instance smí být použita souèasnì pouze v jednom vláknì a pro jednu agendu.
    function GetAgendaInstance: TMonS3APIDataAgenda;
    //Získá instanci pro práci s daty dokumentù. Jedna instance smí být použita souèasnì pouze v jednom vláknì.
    function GetDokumentyInstance: TMonS3APIDataDokumenty;
  end;

implementation

var
  EmptyGUID: TBytes;

var
  IsWindows64Loaded: boolean = false;
  IsWindows64: boolean = false;

//Stejnì jako TOSVersion.Architecture, ale funkèní i pro Delphi 7.
function GetIsWindows64: boolean;
const
  PROCESSOR_ARCHITECTURE_AMD64 = 9;
var
  WHandle: HMODULE;
  WFunc: procedure(var lpSystemInformation: TSystemInfo); stdcall;
  WSysInfo: TSystemInfo;
begin
  if not IsWindows64Loaded then
  begin
    WHandle := LoadLibrary(kernel32);
    if WHandle <> 0 then
    begin
      try
        WFunc := GetProcAddress(WHandle, 'GetNativeSystemInfo');
        if Assigned(WFunc) then
        begin
          ZeroMemory(@WSysInfo, SizeOf(WSysInfo));
          WFunc(WSysInfo);

          IsWindows64 := WSysInfo.wProcessorArchitecture = PROCESSOR_ARCHITECTURE_AMD64;
        end;
      finally
        FreeLibrary(WHandle);
      end;
    end;

    IsWindows64Loaded := true;
  end;
  Result := IsWindows64;
end;

//Ètení DLL kvùli Delphi 7.
const
  FOLDERID_ProgramFilesCommon: TGUID = '{F7F1ED05-9F6D-47A2-AAAE-29D317C6F066}';
  FOLDERID_ProgramFilesCommonX86: TGUID = '{DE974D24-D9C6-4D3E-BF91-F4455120B917}';
  FOLDERID_ProgramFiles: TGUID = '{905e63b6-c1bf-494e-b29c-65b732d3d21a}';
  FOLDERID_ProgramFilesX86: TGUID = '{7C5A40EF-A0FB-4BFC-874A-C0F2E0B9FA8E}';

function GetKnownFolderPath(ID: TGUID): string;
var
  WHandle: HMODULE;
  WFunc: function(const rfid: TIID; dwFlags: DWORD; hToken: THandle; var ppszPath: LPWSTR): HRESULT; stdcall;
  WPath: PWideChar;
begin
  Result := '';

  WHandle := LoadLibrary('shell32.dll');
  if WHandle <> 0 then
  begin
    try
      WFunc := GetProcAddress(WHandle, 'SHGetKnownFolderPath');
      if Assigned(WFunc) then
      begin
        WPath := nil;
        try
          WFunc(ID, 0, 0, WPath);
          Result := WPath;
        finally
          CoTaskMemFree(WPath);
        end;
      end;
    finally
      FreeLibrary(WHandle);
    end;
  end;
end;

constructor TMonS3APIException.Create(AExceptionType: TMonS3APIExceptionType; const ADetail: string);
begin
  inherited Create('');

  ExceptionType := AExceptionType;
  Detail := ADetail;

  case ExceptionType of
    MonS3APIExceptionType_Unknown: Message := 'Neznámá chyba.';
    MonS3APIExceptionType_UnknownFunction: Message := 'Neznámá funkce.';
    MonS3APIExceptionType_UnknownDBError: Message := 'Neošetøená chyba databáze.';
    MonS3APIExceptionType_FileNotFound: Message := 'Soubor neexistuje.';
    MonS3APIExceptionType_AppName: Message := 'Název aplikace nebyl vyplnìn nebo obsahuje neplatné znaky.';
    MonS3APIExceptionType_LoadDll: Message := 'Nelze naèíst DLL knihovnu.';
    MonS3APIExceptionType_DLLNotLoaded: Message := 'Knihovna DLL nebyla naètena.';
    MonS3APIExceptionType_DataPath: Message := 'Cesta k datùm nebyla vyplnìna nebo není platná.';
    MonS3APIExceptionType_NotConnected: Message := 'Nejprve je potøeba se pøihlásit k datùm.';
    MonS3APIExceptionType_AlreadyConnected: Message := 'Existuje otevøené pøipojení k datùm.';
    MonS3APIExceptionType_OldClient: Message := 'Je potøeba aktualizovat klienta na novìjší verzi.';
    MonS3APIExceptionType_OldAPI: Message := 'Je potøeba aktualizovat program MoneyS3.';
    MonS3APIExceptionType_DBNewer: Message := 'Otevíraná databáze vyžaduje ke svému zobrazení novìjší verzi MonS3API.';
    MonS3APIExceptionType_DBElder: Message := 'Otevíraná databáze vyžaduje ke svému zobrazení starší verzi MonS3API.';
    MonS3APIExceptionType_NotLogged: Message := 'Uživatel nebyl pøihlášen.';
    MonS3APIExceptionType_SHA256: Message := 'Nelze použít Hash SHA256.';
    MonS3APIExceptionType_AlreadyLogged: Message := 'Uživatel je již pøihlášen.';
    MonS3APIExceptionType_AgendaNotSelect: Message := 'Agenda nebyla vybrána.';
    MonS3APIExceptionType_NoTransaction: Message := 'Transakce nebyla spuštìna.';
    MonS3APIExceptionType_TransactionAlreadyExists: Message := 'Transakce je již spuštìna.';
    MonS3APIExceptionType_Timeout: Message := 'Vypršel èasový timeout.';
    MonS3APIExceptionType_TableNotFound: Message := 'Tabulka nebyla nalezena.';
    MonS3APIExceptionType_ColNotFound: Message := 'Sloupec tabulky nebyl nalezen.';
    MonS3APIExceptionType_NoRights: Message := 'Pro požadovanou operaci nemáte potøebné oprávnìní.';
    MonS3APIExceptionType_SelectWhat: Message := 'Pro funkci GetRows je potøeba naplnit list what.';
    MonS3APIExceptionType_DeleteAll: Message := 'Funkce DeleteRows vyžaduje podmínku. Pro smazání všeho použijte DeleteAll.';
    MonS3APIExceptionType_BFWriteLock: Message := 'Nelze zapsat do datového souboru, protože je zamknut uživatelským zámkem.';
  end;

  if Assigned(MonS3APIException_OnGetMessage) then
  begin
    MonS3APIException_OnGetMessage(Self);
  end;
end;

constructor TMonS3APIAgenda.Create(const AVisibleName: string; const AName: string; const AIC: string; ARight: TMonS3APIRight; const AExt: string; AIsDemo: boolean; const APoznamka: string);
begin
  FVisibleName := AVisibleName;
  FName := AName;
  FIC := AIC;
  FRight := ARight;
  FExt := AExt;
  FIsDemo := AIsDemo;
  FPoznamka := APoznamka;
end;

function TMonS3APIAgendaList.MyGetItem(AIndex: Integer): TMonS3APIAgenda;
begin
  Result := Get(AIndex);
end;

procedure TMonS3APIAgendaList.MySetItem(AIndex: integer; AValue: TMonS3APIAgenda);
begin
  Put(AIndex, AValue);
end;

function TMonS3APIAgendaList.GetByExt(AExt: string): TMonS3APIAgenda;
var
  i: integer;
begin
  AExt := LowerCase(AExt);

  for i := 0 to Count-1 do
  begin
    if Self[i].FExt = AExt then
    begin
      Result := Self[i];
      exit;
    end;
  end;

  Result := nil;
end;

function TMonS3APIAgendaList.GetFirstNoDemo: TMonS3APIAgenda;
var
  i: integer;
begin
  for i := 0 to Count-1 do
  begin
    if not Self[i].IsDemo then
    begin
      Result := Self[i];
      exit;
    end;
  end;

  Result := nil;
end;

constructor TMonS3APIYear.Create(AYear: integer; ADateFrom: TDateTime; ADateTo: TDateTime; ARight: TMonS3APIRight; const AExt: string);
begin
  FYear := AYear;
  FDateFrom := ADateFrom;
  FDateTo := ADateTo;
  FRight := ARight;
  FExt := AExt;
end;

function TMonS3APIYearList.MyGetItem(AIndex: Integer): TMonS3APIYear;
begin
  Result := Get(AIndex);
end;

procedure TMonS3APIYearList.MySetItem(AIndex: integer; AValue: TMonS3APIYear);
begin
  Put(AIndex, AValue);
end;

function TMonS3APIYearList.GetByDate(ADate: TDateTime): TMonS3APIYear;
var
  i: integer;
begin
  for i := 0 to Count-1 do
  begin
    if (date >= Self[i].DateFrom) and (date <= Self[i].DateTo) then
    begin
      Result := Self[i];
      exit;
    end;
  end;

  Result := nil;
end;

function TMonS3APIYearList.GetActual: TMonS3APIYear;
begin
  Result := GetByDate(Now);
end;

constructor TMonS3APITable.Create(const AName: string; ARight: TMonS3APIRight; APartialRightRow: TMonS3APIRight; APartialRightCol: TMonS3APIRight);
begin
  FName := AName;
  FNameL := LowerCase(AName);
  FRight := ARight;
  FPartialRightRow := APartialRightRow;
  FPartialRightCol := APartialRightCol;
end;

function TMonS3APITableList.MyGetItem(AIndex: Integer): TMonS3APITable;
begin
  Result := Get(AIndex);
end;

procedure TMonS3APITableList.MySetItem(AIndex: integer; AValue: TMonS3APITable);
begin
  Put(AIndex, AValue);
end;

function TMonS3APITableList.GetByName(AName: string): TMonS3APITable;
var
  i: integer;
begin
  AName := LowerCase(AName);

  for i := 0 to Count-1 do
  begin
    if Self[i].NameL = AName then
    begin
      Result := Self[i];
      exit;
    end;
  end;

  Result := nil;
end;

constructor TMonS3APITableColumn.Create(const AName: string; ADataType: TMonS3APIColumnDataType; AStringSize: integer);
begin
  FName := AName;
  FNameL := LowerCase(AName);
  FDataType := ADataType;
  FStringSize := AStringSize;
end;

function TMonS3APITableColumnList.MyGetItem(AIndex: Integer): TMonS3APITableColumn;
begin
  Result := Get(AIndex);
end;

procedure TMonS3APITableColumnList.MySetItem(AIndex: integer; AValue: TMonS3APITableColumn);
begin
  Put(AIndex, AValue);
end;

function TMonS3APITableColumnList.GetIDColumn: TMonS3APITableColumn;
var
  i: integer;
begin
  for i := 0 to Count-1 do
  begin
    if Self[i].DataType = MonS3APIColumnDataType_ID then
    begin
      Result := Self[i];
      exit;
    end;
  end;

  Result := nil;
end;

function TMonS3APITableColumnList.GetByName(AName: string): TMonS3APITableColumn;
var
  i: integer;
begin
  AName := LowerCase(AName);

  for i := 0 to Count-1 do
  begin
    if Self[i].NameL = AName then
    begin
      Result := Self[i];
      exit;
    end;
  end;

  Result := nil;
end;

constructor TMonS3APIValue.Create;
begin
  FValType := _MonS3APIParserValType_Empty;
end;

procedure TMonS3APIValue.Clear;
begin
  FValType := _MonS3APIParserValType_Empty;
end;

function TMonS3APIValue.CreateCopy: TMonS3APIValue;
begin
  Result := TMonS3APIValue.Create;
  Result.CopyFrom(Self);
end;

procedure TMonS3APIValue.CopyFrom(AValue: TMonS3APIValue);
begin
  FValType := AValue.FValType;
  FValExt := AValue.FValExt;
  FValStr := AValue.FValStr;
  FValData := AValue.FValData;
end;

function TMonS3APIValue.GetAsInt: integer;
begin
  try
    case FValType of
      _MonS3APIParserValType_Int,
      _MonS3APIParserValType_Int64,
      _MonS3APIParserValType_Ext: Result := Trunc(FValExt);
      else
        Result := 0;
    end;
  except
    Result := 0;
  end;
end;

procedure TMonS3APIValue.SetAsInt(value: integer);
begin
  FValType := _MonS3APIParserValType_Int;
  FValExt := value;
end;

function TMonS3APIValue.GetAsInt64: Int64;
begin
  try
    case FValType of
      _MonS3APIParserValType_Int,
      _MonS3APIParserValType_Int64,
      _MonS3APIParserValType_Ext: Result := Trunc(FValExt);
      else
        Result := 0;
    end;
  except
    Result := 0;
  end;
end;

procedure TMonS3APIValue.SetAsInt64(value: Int64);
begin
  FValType := _MonS3APIParserValType_Int64;
  FValExt := value;
end;

function TMonS3APIValue.GetAsExt: extended;
begin
  case FValType of
    _MonS3APIParserValType_Int,
    _MonS3APIParserValType_Int64,
    _MonS3APIParserValType_Ext: Result := FValExt;
    else
      Result := 0;
  end;
end;

procedure TMonS3APIValue.SetAsExt(value: extended);
begin
  FValType := _MonS3APIParserValType_Ext;
  FValExt := value;
end;

function TMonS3APIValue.GetAsString: string;
begin
  if FValType = _MonS3APIParserValType_Str then
  begin
    Result := FValStr;
  end
  else
  begin
    Result := '';
  end;
end;

procedure TMonS3APIValue.SetAsString(const value: string);
begin
  FValType := _MonS3APIParserValType_Str;
  FValStr := value;
end;

function TMonS3APIValue.GetAsBool: boolean;
begin
  if FValType = _MonS3APIParserValType_Bool then
  begin
    Result := FValExt <> 0;
  end
  else
  begin
    Result := false;
  end;
end;

procedure TMonS3APIValue.SetAsBool(value: boolean);
begin
  FValType := _MonS3APIParserValType_Bool;

  if value then
  begin
    FValExt := 1;
  end
  else
  begin
    FValExt := 0;
  end;
end;

function TMonS3APIValue.GetAsDate: TDateTime;
begin
  case FValType of
    _MonS3APIParserValType_Date,
    _MonS3APIParserValType_DateTime: Result := DateOf(FValExt);
    else
      Result := 0;
  end;
end;

procedure TMonS3APIValue.SetAsDate(value: TDateTime);
begin
  FValType := _MonS3APIParserValType_Date;
  FValExt := DateOf(value);
end;

function TMonS3APIValue.GetAsTime: TDateTime;
begin
  case FValType of
    _MonS3APIParserValType_Time,
    _MonS3APIParserValType_DateTime: Result := TimeOf(FValExt);
    else
      Result := 0;
  end;
end;

procedure TMonS3APIValue.SetAsTime(value: TDateTime);
begin
  FValType := _MonS3APIParserValType_Time;
  FValExt := TimeOf(value);
end;

function TMonS3APIValue.GetAsDateTime: TDateTime;
begin
  case FValType of
    _MonS3APIParserValType_Date,
    _MonS3APIParserValType_Time,
    _MonS3APIParserValType_DateTime: Result := FValExt;
    else
      Result := 0;
  end;
end;

procedure TMonS3APIValue.SetAsDateTime(value: TDateTime);
begin
  FValType := _MonS3APIParserValType_DateTime;
  FValExt := value;
end;

function TMonS3APIValue.GetAsData: TBytes;
begin
  if FValType = _MonS3APIParserValType_Data then
  begin
    Result := FValData;
  end
  else
  begin
    SetLength(Result, 0);
  end;
end;

procedure TMonS3APIValue.SetAsData(value: TBytes);
begin
  FValType := _MonS3APIParserValType_Data;
  FValData := value;
end;


class function TMonS3APIValue.CreateEmpty: TMonS3APIValue;
begin
  Result := TMonS3APIValue.Create;
  Result.Clear;
end;

class function TMonS3APIValue.CreateInt(value: integer): TMonS3APIValue;
begin
  Result := TMonS3APIValue.Create;
  Result.AsInt := value;
end;

class function TMonS3APIValue.CreateInt64(value: Int64): TMonS3APIValue;
begin
  Result := TMonS3APIValue.Create;
  Result.AsInt64 := value;
end;

class function TMonS3APIValue.CreateExt(value: extended): TMonS3APIValue;
begin
  Result := TMonS3APIValue.Create;
  Result.AsExt := value;
end;

class function TMonS3APIValue.CreateString(const value: string): TMonS3APIValue;
begin
  Result := TMonS3APIValue.Create;
  Result.AsString := value;
end;

class function TMonS3APIValue.CreateBool(value: boolean): TMonS3APIValue;
begin
  Result := TMonS3APIValue.Create;
  Result.AsBool := value;
end;

class function TMonS3APIValue.CreateDate(value: TDateTime): TMonS3APIValue;
begin
  Result := TMonS3APIValue.Create;
  Result.AsDate := value;
end;

class function TMonS3APIValue.CreateTime(value: TDateTime): TMonS3APIValue;
begin
  Result := TMonS3APIValue.Create;
  Result.AsTime := value;
end;

class function TMonS3APIValue.CreateDateTime(value: TDateTime): TMonS3APIValue;
begin
  Result := TMonS3APIValue.Create;
  Result.AsDateTime := value;
end;

class function TMonS3APIValue.CreateData(value: TBytes): TMonS3APIValue;
begin
  Result := TMonS3APIValue.Create;
  Result.AsData := value;
end;

function TMonS3APIValue.ToSQLString: string;
var
  WFormatSettings: TFormatSettings;
begin
  case FValType of
    _MonS3APIParserValType_Int: Result := IntToStr(AsInt);
    _MonS3APIParserValType_Int64: Result := IntToStr(AsInt64);
    _MonS3APIParserValType_Ext:
      begin
        GetLocaleFormatSettings(LOCALE_SYSTEM_DEFAULT, WFormatSettings);
        WFormatSettings.DecimalSeparator := '.';
        WFormatSettings.ThousandSeparator := #0;
        WFormatSettings.CurrencyString := '';
        WFormatSettings.CurrencyFormat := 1;
        WFormatSettings.CurrencyDecimals := 4;
        Result := FloatToStr(AsExt, WFormatSettings);
      end;
    _MonS3APIParserValType_Str: Result := '''' + AsString + '''';
    _MonS3APIParserValType_Bool:
      begin
        if AsBool then
        begin
          Result := '1';
        end
        else
        begin
          Result := '0';
        end;
      end;
    _MonS3APIParserValType_Date: Result := 'Date(' + FormatDateTime('yyyy-mm-dd', AsDate) + ')';
    _MonS3APIParserValType_Time: Result := 'Time(' + FormatDateTime('hh:nn:ss.zzz', AsTime) + ')';
    _MonS3APIParserValType_DateTime: Result := 'DateTime(' + FormatDateTime('yyyy-mm-dd hh:nn:ss.zzz', AsDateTime) + ')';
    else
      Result := '<empty>';
  end;
end;

function TMonS3APIValueList.MyGetItem(AIndex: Integer): TMonS3APIValue;
begin
  Result := Get(AIndex);
end;

procedure TMonS3APIValueList.MySetItem(AIndex: integer; AValue: TMonS3APIValue);
begin
  Put(AIndex, AValue);
end;

function TMonS3APIValueList.CreateCopy: TMonS3APIValueList;
begin
  Result := TMonS3APIValueList.Create;
  Result.CopyFrom(Self);
end;

procedure TMonS3APIValueList.CopyFrom(AList: TMonS3APIValueList);
var
  i: integer;
begin
  Capacity := AList.Count;
  for i := 0 to AList.Count-1 do
  begin
    Add(AList[i].CreateCopy);
  end;
end;

constructor TMonS3APIDataWhat.Create(const AValue: string);
begin
  FValue := AValue;
end;

function TMonS3APIDataWhatList.MyGetItem(AIndex: Integer): TMonS3APIDataWhat;
begin
  Result := Get(AIndex);
end;

procedure TMonS3APIDataWhatList.MySetItem(AIndex: integer; AValue: TMonS3APIDataWhat);
begin
  Put(AIndex, AValue);
end;

procedure TMonS3APIDataWhatList.AddAll;
begin
  Add(TMonS3APIDataWhat.Create('*'));
end;

constructor TMonS3APIDataWhere.Create(AType: TMonS3APIDataWhereType);
begin
  FValues := TMonS3APIValueList.Create;
  FType := AType;
end;

constructor TMonS3APIDataWhere.Create(const AColumn: string; AOperator: TMonS3APIDataWhereOperator; AValue: TMonS3APIValue);
begin
  FValues := TMonS3APIValueList.Create;
  FType := MonS3APIDataWhereType_Where;
  FColumn := AColumn;
  FOperator := AOperator;
  FValues.Add(AValue);
end;

constructor TMonS3APIDataWhere.Create(const AColumn: string; AOperator: TMonS3APIDataWhereOperator; const AValues: Array of TMonS3APIValue);
var
  i: integer;
begin
  FValues := TMonS3APIValueList.Create;
  FType := MonS3APIDataWhereType_Where;
  FColumn := AColumn;
  FOperator := AOperator;

  for i := 0 to Length(AValues)-1 do
  begin
    FValues.Add(AValues[i]);
  end;
end;

constructor TMonS3APIDataWhere.Create(const AColumn: string; AOperator: TMonS3APIDataWhereOperator; AValues: TMonS3APIValueList);
begin
  FValues := TMonS3APIValueList.Create;
  FType := MonS3APIDataWhereType_Where;
  FColumn := AColumn;
  FOperator := AOperator;

  FValues.CopyFrom(AValues);
end;

destructor TMonS3APIDataWhere.Destroy;
begin
  FValues.Free;
  inherited;
end;

function TMonS3APIDataWhereList.MyGetItem(AIndex: Integer): TMonS3APIDataWhere;
begin
  Result := Get(AIndex);
end;

procedure TMonS3APIDataWhereList.MySetItem(AIndex: integer; AValue: TMonS3APIDataWhere);
begin
  Put(AIndex, AValue);
end;

procedure TMonS3APIDataWhereList.AddBracketStart;
begin
  Add(TMonS3APIDataWhere.Create(MonS3APIDataWhereType_BracketStart));
end;

procedure TMonS3APIDataWhereList.AddBracketEnd;
begin
  Add(TMonS3APIDataWhere.Create(MonS3APIDataWhereType_BracketEnd));
end;

procedure TMonS3APIDataWhereList.AddOperatorAND;
begin
  Add(TMonS3APIDataWhere.Create(MonS3APIDataWhereType_OperatorAND));
end;

procedure TMonS3APIDataWhereList.AddOperatorOR;
begin
  Add(TMonS3APIDataWhere.Create(MonS3APIDataWhereType_OperatorOR));
end;

procedure TMonS3APIDataWhereList.AddWhere(const AColumn: string; AOperator: TMonS3APIDataWhereOperator; AValue: TMonS3APIValue);
begin
  Add(TMonS3APIDataWhere.Create(AColumn, AOperator, AValue));
end;

procedure TMonS3APIDataWhereList.AddWhere(const AColumn: string; AOperator: TMonS3APIDataWhereOperator; const AValues: Array of TMonS3APIValue);
begin
  Add(TMonS3APIDataWhere.Create(AColumn, AOperator, AValues));
end;

procedure TMonS3APIDataWhereList.AddWhere(const AColumn: string; AOperator: TMonS3APIDataWhereOperator; AValues: TMonS3APIValueList);
begin
  Add(TMonS3APIDataWhere.Create(AColumn, AOperator, AValues));
end;

function TMonS3APIDataWhereList.GetSQLCommandString: string;
var
  i: integer;
  ii: integer;
  WItem: TMonS3APIDataWhere;
begin
  Result := '';

  for i := 0 to Count-1 do
  begin
    WItem := Self[i];

    case WItem.FType of
      MonS3APIDataWhereType_BracketStart: Result := Result + '(';
      MonS3APIDataWhereType_BracketEnd: Result := Result + ')';
      MonS3APIDataWhereType_OperatorAND: Result := Result + ' AND ';
      MonS3APIDataWhereType_OperatorOR: Result := Result + ' OR ';
      MonS3APIDataWhereType_Where:
        begin
          Result := Result + '[' + WItem.Column + ']';

          case WItem.OOperator of
            MonS3APIDataWhereOperator_Equals: Result := Result + '=' + WItem.FValues[0].ToSQLString;
            MonS3APIDataWhereOperator_NotEquals: Result := Result + '<>' + WItem.FValues[0].ToSQLString;
            MonS3APIDataWhereOperator_LessThan: Result := Result + '<' + WItem.FValues[0].ToSQLString;
            MonS3APIDataWhereOperator_LessEq: Result := Result + '<=' + WItem.FValues[0].ToSQLString;
            MonS3APIDataWhereOperator_GreaterThan: Result := Result + '>' + WItem.FValues[0].ToSQLString;
            MonS3APIDataWhereOperator_GreaterEq: Result := Result + '>=' + WItem.FValues[0].ToSQLString;
            MonS3APIDataWhereOperator_BeginsWith,
            MonS3APIDataWhereOperator_NotBeginsWith,
            MonS3APIDataWhereOperator_EndsWith,
            MonS3APIDataWhereOperator_NotEndsWith,
            MonS3APIDataWhereOperator_Contains,
            MonS3APIDataWhereOperator_NotContains:
              begin
                if WItem.FOperator in [MonS3APIDataWhereOperator_NotBeginsWith, MonS3APIDataWhereOperator_NotEndsWith, MonS3APIDataWhereOperator_NotContains] then
                begin
                  Result := Result + ' NOT';
                end;
                Result := Result + ' LIKE ''';
                if WItem.FOperator in [MonS3APIDataWhereOperator_EndsWith, MonS3APIDataWhereOperator_NotEndsWith, MonS3APIDataWhereOperator_Contains, MonS3APIDataWhereOperator_NotContains] then
                begin
                  Result := Result + '%';
                end;
                Result := Result + WItem.FValues[0].AsString;
                if WItem.FOperator in [MonS3APIDataWhereOperator_BeginsWith, MonS3APIDataWhereOperator_NotBeginsWith, MonS3APIDataWhereOperator_Contains, MonS3APIDataWhereOperator_NotContains] then
                begin
                  Result := Result + '%';
                end;
               Result := Result + '''';
              end;
            MonS3APIDataWhereOperator_InValues,
            MonS3APIDataWhereOperator_NotInValues:
              begin
                if WItem.FOperator = MonS3APIDataWhereOperator_NotInValues then
                begin
                  Result := Result + ' NOT';
                end;

                Result := Result + ' IN(';

                for ii := 0 to WItem.FValues.Count-1 do
                begin
                  if ii <> 0 then
                  begin
                    Result := Result + ',';
                  end;

                  Result := Result + WItem.FValues[ii].ToSQLString;
                end;

                Result := Result + ')';
              end;
            MonS3APIDataWhereOperator_Between,
            MonS3APIDataWhereOperator_NotBetween:
              begin
                if WItem.FOperator = MonS3APIDataWhereOperator_NotBetween then
                begin
                  Result := Result + ' NOT';
                end;

                Result := Result + ' BETWEEN ' + WItem.FValues[0].ToSQLString + ' AND ' + WItem.FValues[1].ToSQLString;
              end;
          end;
        end;
    end;
  end;
end;

constructor TMonS3APIDataSort.Create(const AColumn: string; AType: TMonS3APIDataSortType);
begin
  FColumn := AColumn;
  FType := AType;
end;

function TMonS3APIDataSortList.MyGetItem(AIndex: Integer): TMonS3APIDataSort;
begin
  Result := Get(AIndex);
end;

procedure TMonS3APIDataSortList.MySetItem(AIndex: integer; AValue: TMonS3APIDataSort);
begin
  Put(AIndex, AValue);
end;

procedure TMonS3APIDataSortList.AddSort(const AColumn: string; AType: TMonS3APIDataSortType);
begin
  Add(TMonS3APIDataSort.Create(AColumn, AType));
end;

constructor TMonS3APIDataValue.Create(const AColumn: string; AValue: TMonS3APIValue);
begin
  FColumn := AColumn;
  FValue := AValue;
end;

destructor TMonS3APIDataValue.Destroy;
begin
  FValue.Free;
  inherited;
end;

function TMonS3APIDataValueList.MyGetItem(AIndex: Integer): TMonS3APIDataValue;
begin
  Result := Get(AIndex);
end;

procedure TMonS3APIDataValueList.MySetItem(AIndex: integer; AValue: TMonS3APIDataValue);
begin
  Put(AIndex, AValue);
end;

procedure TMonS3APIDataValueList.AddValue(const AColumn: string; AValue: TMonS3APIValue);
begin
  Add(TMonS3APIDataValue.Create(AColumn, AValue));
end;

constructor TMonS3APIDataSelectResult.Create;
begin
  FColumns := TStringList.Create;
  FValues := TMonS3APIValueList.Create;
  FEmptyValue := TMonS3APIValue.Create;
end;

destructor TMonS3APIDataSelectResult.Destroy;
begin
  FColumns.Free;
  FValues.Free;
  FParser.Free;
  FEmptyValue.Free;
  inherited;
end;

procedure TMonS3APIDataSelectResult.ConnectToParser(ADataBase: TObject; AParser: _TMonS3APIParser);
begin
  //Uvolnit pøedchozí, pokud byl nastaven.
  FreeAndNil(FParser);

  FDataBase := ADataBase;
  FParser := AParser;

  FColumns.Clear;
  FPosition := -1;
end;

procedure TMonS3APIDataSelectResult.ReadParser;
var
  WColCount: integer;
  i: integer;
begin
  FCount := FParser.GetArray;

  if FCount = 0 then
  begin
    exit;
  end;

  WColCount := FParser.GetArray;
  FColumns.Capacity := WColCount;

  for i := 0 to WColCount-1 do
  begin
    FColumns.Add(FParser.GetStr);
  end;
end;

function TMonS3APIDataSelectResult.Next: boolean;
var
  i: integer;
  WValue: TMonS3APIValue;
  WValType: _TMonS3APIParserValType;
begin
  Result := false;
  FValues.Count := 0;
  inc(FPosition);

  if (FPosition >= FCount) then
  begin
    exit;
  end;

  for i := 0 to FColumns.Count-1 do
  begin
    WValue := TMonS3APIValue.Create;
    FValues.Add(WValue);

    WValType := _TMonS3APIParserValType(FParser._ReadWord);
    case WValType of
      _MonS3APIParserValType_Int: WValue.AsInt := FParser._ReadInt;
      _MonS3APIParserValType_Int64: WValue.AsInt64 := FParser._ReadInt64;
      _MonS3APIParserValType_Ext: WValue.AsExt := FParser._ReadExt;
      _MonS3APIParserValType_Str: WValue.AsString := FParser._ReadStr;
      _MonS3APIParserValType_Bool: WValue.AsBool := FParser._ReadBool;
      _MonS3APIParserValType_Date: WValue.AsDate := FParser._ReadDate;
      _MonS3APIParserValType_Time: WValue.AsTime := FParser._ReadTime;
      _MonS3APIParserValType_DateTime: WValue.AsDateTime := FParser._ReadDateTime;
      _MonS3APIParserValType_Data: WValue.AsData := FParser._ReadData;
    end;
  end;

  Result := true;
end;

function TMonS3APIDataSelectResult.GetColByName(AColumn: string): TMonS3APIValue;
var
  WIdx: integer;
begin
  AColumn := LowerCase(AColumn);

  WIdx := FColumns.IndexOf(AColumn);
  if WIdx = -1 then
  begin
    if TMonS3APIDataBase(FDataBase).FSettingErrorIfColNotFound then
    begin
      raise TMonS3APIException.Create(MonS3APIExceptionType_ColNotFound, '');
    end
    else
    begin
      Result := FEmptyValue;
      exit;
    end;
  end;

  Result := FValues[WIdx];
end;

constructor TMonS3APIUserInfo.Create(const AGUID: string; AID: integer; const AJmeno: string; const AConfigName: string; const APoznamka: string);
begin
  FGUID := AGUID;
  FID := AID;
  FJmeno := AJmeno;
  FConfigName := AConfigName;
  FPoznamka := APoznamka;
end;

constructor TMonS3APIUserRight.Create(const ARightName: string; const ARightValue: string);
begin
  FRightName := ARightName;
  FRightNameL := LowerCase(ARightName);
  FRightValue := ARightValue;
end;

function TMonS3APIUserRightList.MyGetItem(AIndex: Integer): TMonS3APIUserRight;
begin
  Result := Get(AIndex);
end;

procedure TMonS3APIUserRightList.MySetItem(AIndex: integer; AValue: TMonS3APIUserRight);
begin
  Put(AIndex, AValue);
end;

function TMonS3APIUserRightList.GetByName(ARightName: string): TMonS3APIUserRight;
var
  i: integer;
begin
  ARightName := LowerCase(ARightName);

  for i := 0 to Count-1 do
  begin
    if Self[i].RightNameL = ARightName then
    begin
      Result := Self[i];
      exit;
    end;
  end;

  Result := nil;
end;

destructor TMonS3APIDataMain.Destroy;
begin
  UnLoadDLL;
  inherited;
end;

function TMonS3APIDataMain.FindDLL(aMonS3APIDLLType: TMonS3APIDLLType = MonS3API): string;

  //Hledá knihovnu MonS3API.dll ve složce Solitea nebo CIGLER SOFTWARE v Program Files.
  function FindDLLCheckProgram2(const ADirPath: string): string;
  var
    WPath: string;
  begin
    Result := '';

    if ADirPath = '' then
    begin
      exit;
    end;

    if aMonS3APIDLLType = MonS3API then
    begin
      {$IFDEF WIN64}
        WPath := IncludeTrailingPathDelimiter(ADirPath) + 'MonS3API_64.dll';
      {$ELSE}
        WPath := IncludeTrailingPathDelimiter(ADirPath) + 'MonS3API_32.dll';
      {$ENDIF}
    end
    else
    begin
      {$IFDEF WIN64}
        WPath := IncludeTrailingPathDelimiter(ADirPath) + 'MonS3Reader_64.dll';
      {$ELSE}
        WPath := IncludeTrailingPathDelimiter(ADirPath) + 'MonS3Reader_32.dll';
      {$ENDIF}
    end;

    if FileExists(WPath) then
    begin
      Result := WPath;
    end;
  end;

  //Hledá knihovnu MonS3API.dll ve složce Program Files.
  function FindDLLCheckProgram(const ADirPath: string): string;
  begin
    Result := '';

    if ADirPath = '' then
    begin
      exit;
    end;

    if Result = '' then
    begin
      Result := FindDLLCheckProgram2(IncludeTrailingPathDelimiter(ADirPath) + 'Solitea\Money S3');
    end;

    if Result = '' then
    begin
      Result := FindDLLCheckProgram2(IncludeTrailingPathDelimiter(ADirPath) + 'CIGLER SOFTWARE\Money S3');
    end;
  end;

  //Hledá knihovnu MonS3API.dll - cesta dle ini souboru.
  function FindDLLCheckCommon3(const AIniFile: string): string;
  var
    WIni: TIniFile;
  begin
    Result := '';

    if (AIniFile = '') or not FileExists(AIniFile) then
    begin
      exit;
    end;

    WIni := TIniFile.Create(AIniFile);
    try
      Result := FindDLLCheckProgram2(WIni.ReadString('InstParams', 'LocalPath', ''));
    finally
      WIni.Free;
    end;
  end;

  //Hledá knihovnu MonS3API.dll ve složce Solitea nebo CIGLER SOFTWARE v Common Files.
  function FindDLLCheckCommon2(const ADirPath: string): string;
  begin
    Result := '';

    if ADirPath = '' then
    begin
      exit;
    end;

    //nejdøíve pokus o nalezení MonS3API.dll pøímo ve složce Common Files\Solitea nebo Common Files\CIGLER SOFTWARE 
    if Result = '' then
    begin
      Result := FindDLLCheckProgram2(ADirPath);
    end;

    if Result = '' then
    begin
      Result := FindDLLCheckCommon3(IncludeTrailingPathDelimiter(ADirPath) + 'Money S3\setup.ini');
    end;

    if Result = '' then
    begin
      Result := FindDLLCheckCommon3(IncludeTrailingPathDelimiter(ADirPath) + 'Money S3\start.ini');
    end;
  end;

  //Hledá knihovnu MonS3API.dll ve složce Common Files.
  function FindDLLCheckCommon(const ADirPath: string): string;
  begin
    Result := '';

    if ADirPath = '' then
    begin
      exit;
    end;

    if Result = '' then
    begin
      Result := FindDLLCheckCommon2(IncludeTrailingPathDelimiter(ADirPath) + 'Solitea');
    end;


    if Result = '' then
    begin
      Result := FindDLLCheckCommon2(IncludeTrailingPathDelimiter(ADirPath) + 'CIGLER SOFTWARE');
    end;
  end;

begin
  Result := '';

  if Result = ''  then
  begin
    Result := FindDLLCheckCommon(GetKnownFolderPath(FOLDERID_ProgramFilesCommon));
  end;

  if (Result = '') and GetIsWindows64 then
  begin
    Result := FindDLLCheckCommon(GetKnownFolderPath(FOLDERID_ProgramFilesCommonX86));
  end;

  if Result = '' then
  begin
    Result := FindDLLCheckProgram(GetKnownFolderPath(FOLDERID_ProgramFiles));
  end;

  if (Result = '') and GetIsWindows64 then
  begin
    Result := FindDLLCheckProgram(GetKnownFolderPath(FOLDERID_ProgramFilesX86));
  end;
end;

procedure TMonS3APIDataMain.LoadDLL(const AFullFileName: string; const AAppName: string; aMonS3APIDLLType: TMonS3APIDLLType = MonS3API);
var
  WParser: _TMonS3APIParser;
begin
  UnLoadDLL;

  if (AFullFileName = '') or not FileExists(AFullFileName) then
  begin
    raise TMonS3APIException.Create(MonS3APIExceptionType_FileNotFound, AFullFileName);
  end;

  FHandleDLL := LoadLibrary(PChar(AFullFileName));
  if FHandleDLL = 0 then
  begin
    raise TMonS3APIException.Create(MonS3APIExceptionType_LoadDll, AFullFileName);
  end;

  try
    FDLLFunc := GetProcAddress(FHandleDLL, 'DLLFunc');
    FDLLFuncFree := GetProcAddress(FHandleDLL, 'DLLFuncFree');

    if not Assigned(FDLLFunc) or not Assigned(FDLLFuncFree) then
    begin
      raise TMonS3APIException.Create(MonS3APIExceptionType_LoadDll, AFullFileName);
    end;

    FClientGUID := nil;
    WParser := _CreateParser('LoadDLL');
    try
      //Verze MonS3API!
      WParser.AddCardinal(1);
      WParser.AddStr(AAppName);
      WParser.AddStr(AFullFileName);
      WParser.AddStr(FindDLL(aMonS3APIDLLType));
      WParser.Call;

      FClientGUID := WParser.GetData;
    finally
      WParser.Free;
    end;
  except
    UnLoadDLL;
    raise;
  end;
end;

procedure TMonS3APIDataMain.UnLoadDLL;
var
  WParser: _TMonS3APIParser;
begin
  if FHandleDLL = 0 then
  begin
    exit;
  end;

  if Assigned(FClientGUID) then
  begin
    WParser := _CreateParser('UnLoadDLL', true);
    try
      WParser.Call;
    finally
      WParser.Free;
    end;
  end;

  FreeLibrary(FHandleDLL);
  FHandleDLL := 0;
  FDLLFunc := nil;
  FDLLFuncFree := nil;
  FClientGUID := nil;
end;

function TMonS3APIDataMain.FindData: string;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('FindData');
  try
    WParser.Call;
    Result := WParser.GetStr;
  finally
    WParser.Free;
  end;
end;

procedure TMonS3APIDataMain.SetDataPath(const AFullPath: string);
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('SetDataPath');
  try
    WParser.AddStr(AFullPath);
    WParser.Call;
  finally
    WParser.Free;
  end;
end;

function TMonS3APIDataMain.GetProgramInstance: TMonS3APIDataProgram;
var
  WParser: _TMonS3APIParser;
  WObjectGUID: TBytes;
begin
  WParser := _CreateParser('GetProgramInstance');
  try
    WParser.Call;
    WObjectGUID := WParser.GetData;
  finally
    WParser.Free;
  end;

  Result := TMonS3APIDataProgram.Create(self, WObjectGUID);
end;

function TMonS3APIDataMain.GetAgendaInstance: TMonS3APIDataAgenda;
var
  WParser: _TMonS3APIParser;
  WObjectGUID: TBytes;
begin
  WParser := _CreateParser('GetAgendaInstance');
  try
    WParser.Call;
    WObjectGUID := WParser.GetData;
  finally
    WParser.Free;
  end;

  Result := TMonS3APIDataAgenda.Create(self, WObjectGUID);
end;

function TMonS3APIDataMain.GetDokumentyInstance: TMonS3APIDataDokumenty;
var
  WParser: _TMonS3APIParser;
  WObjectGUID: TBytes;
begin
  WParser := _CreateParser('GetDokumentyInstance');
  try
    WParser.Call;
    WObjectGUID := WParser.GetData;
  finally
    WParser.Free;
  end;

  Result := TMonS3APIDataDokumenty.Create(self, WObjectGUID);
end;

function TMonS3APIDataMain._CreateParser(const AFunc: string; AIgnoreErrors: boolean = false): _TMonS3APIParser;
begin
  Result := _CreateParser(AFunc, nil, AIgnoreErrors);
end;

function TMonS3APIDataMain._CreateParser(const AFunc: string; const AObjectGUID: TBytes; AIgnoreErrors: boolean = false): _TMonS3APIParser;
begin
  if FHandleDLL = 0 then
  begin
    if not AIgnoreErrors then
    begin
      raise TMonS3APIException.Create(MonS3APIExceptionType_DLLNotLoaded, '');
    end;
  end;

  Result := _TMonS3APIParser.Create(Self, FClientGUID, AObjectGUID, AFunc, AIgnoreErrors);
end;

function TMonS3APIDataMain._Call(AInp: Pointer): Pointer;
begin
  Result := FDLLFunc(AInp);
end;

procedure TMonS3APIDataMain._CallFree(AInp: Pointer);
begin
  FDLLFuncFree(AInp);
end;

constructor TMonS3APIDataBase.Create(AParent: TObject; const AObjectGUID: TBytes);
begin
  FParent := AParent;
  FObjectGUID := AObjectGUID;
end;

destructor TMonS3APIDataBase.Destroy;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('DestroyObject', true);
  try
    WParser.Call;
  finally
    WParser.Free;
  end;

  inherited;
end;

function TMonS3APIDataBase.GetSettingsLockTimeout: integer;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('GetSettingsLockTimeout');
  try
    WParser.Call;
    Result := WParser.GetInt;
  finally
    WParser.Free;
  end;
end;

procedure TMonS3APIDataBase.SetSettingsLockTimeout(AValue: integer);
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('SetSettingsLockTimeout');
  try
    WParser.AddInt(AValue);
    WParser.Call;
  finally
    WParser.Free;
  end;
end;

procedure TMonS3APIDataBase.ConnectData;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('ConnectData');
  try
    WParser.Call;
  finally
    WParser.Free;
  end;
end;

procedure TMonS3APIDataBase.DisconnectData;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('DisconnectData');
  try
    WParser.Call;
  finally
    WParser.Free;
  end;
end;

procedure TMonS3APIDataBase.GetTableList(AOutput: TMonS3APITableList);
var
  WParser: _TMonS3APIParser;
  WCount: integer;
  i: integer;
  WName: string;
  WRight: TMonS3APIRight;
  WPartialRightRow: TMonS3APIRight;
  WPartialRightCol: TMonS3APIRight;
begin
  WParser := _CreateParser('GetTableList');
  try
    WParser.Call;

    WCount := WParser.GetArray;
    AOutput.Capacity := WCount;

    for i := 0 to WCount-1 do
    begin
      WName := WParser.GetStr;
      WRight := TMonS3APIRight(WParser.GetByte);
      WPartialRightRow := TMonS3APIRight(WParser.GetByte);
      WPartialRightCol := TMonS3APIRight(WParser.GetByte);

      AOutput.Add(TMonS3APITable.Create(WName, WRight, WPartialRightRow, WPartialRightCol));
    end;
  finally
    WParser.Free;
  end;
end;

procedure TMonS3APIDataBase.GetTableColumns(const ATable: string; AOutput: TMonS3APITableColumnList);
var
  WParser: _TMonS3APIParser;
  WCount: integer;
  i: integer;
  WName: string;
  WDataType: TMonS3APIColumnDataType;
  WStringSize: integer;
begin
  WParser := _CreateParser('GetTableColumns');
  try
    WParser.AddStr(ATable);
    WParser.Call;

    WCount := WParser.GetArray;
    AOutput.Capacity := WCount;

    for i := 0 to WCount-1 do
    begin
      WName := WParser.GetStr;
      WDataType := TMonS3APIColumnDataType(WParser.GetByte);
      WStringSize := WParser.GetCardinal;

      AOutput.Add(TMonS3APITableColumn.Create(WName, WDataType, WStringSize));
    end;
  finally
    WParser.Free;
  end;
end;

procedure TMonS3APIDataBase.TransactionStart;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('TransactionStart');
  try
    WParser.Call;
  finally
    WParser.Free;
  end;
end;

procedure TMonS3APIDataBase.TransactionCommit;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('TransactionCommit');
  try
    WParser.Call;
  finally
    WParser.Free;
  end;
end;

procedure TMonS3APIDataBase.TransactionRollback;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('TransactionRollback', true);
  try
    WParser.Call;
  finally
    WParser.Free;
  end;
end;

function TMonS3APIDataBase.GetRowExists(const ATable: string; AWhere: TMonS3APIDataWhereList = nil): boolean;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('GetRowExists');
  try
    WParser.AddStr(ATable);
    _AddWhere(WParser, AWhere);
    WParser.Call;

    Result := WParser.GetBool;
  finally
    WParser.Free;
  end;
end;

function TMonS3APIDataBase.GetRowCount(const ATable: string; AWhere: TMonS3APIDataWhereList = nil): integer;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('GetRowCount');
  try
    WParser.AddStr(ATable);
    _AddWhere(WParser, AWhere);
    WParser.Call;

    Result := WParser.GetCardinal;
  finally
    WParser.Free;
  end;
end;

procedure TMonS3APIDataBase.GetRows(const ATable: string; AWhat: TMonS3APIDataWhatList; AOutput: TMonS3APIDataSelectResult; AWhere: TMonS3APIDataWhereList = nil; ASort: TMonS3APIDataSortList = nil; AFrom: integer = 0; ALimit: integer = -1);
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('GetRows');
  AOutput.ConnectToParser(Self, WParser);

  WParser.AddStr(ATable);
  _AddWhat(WParser, AWhat);
  _AddWhere(WParser, AWhere);
  _AddSort(WParser, ASort);
  WParser.AddInt(AFrom);
  WParser.AddInt(ALimit);
  WParser.Call;

  AOutput.ReadParser;
end;

function TMonS3APIDataBase.AddRow(const ATable: string; AValues: TMonS3APIDataValueList): integer;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('AddRow');
  try
    WParser.AddStr(ATable);
    _AddDataValues(WParser, AValues);
    WParser.Call;

    Result := WParser.GetInt;
  finally
    WParser.Free;
  end;
end;

function TMonS3APIDataBase.UpdateRows(const ATable: string; AValues: TMonS3APIDataValueList; AWhere: TMonS3APIDataWhereList = nil): integer;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('UpdateRows');
  try
    WParser.AddStr(ATable);
    _AddDataValues(WParser, AValues);
    _AddWhere(WParser, AWhere);
    WParser.Call;

    Result := WParser.GetInt;
  finally
    WParser.Free;
  end;
end;

function TMonS3APIDataBase.DeleteRows(const ATable: string; AWhere: TMonS3APIDataWhereList): integer;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('DeleteRows');
  try
    WParser.AddStr(ATable);
    _AddWhere(WParser, AWhere);
    WParser.Call;

    Result := WParser.GetInt;
  finally
    WParser.Free;
  end;
end;

function TMonS3APIDataBase.DeleteAll(const ATable: string): integer;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('DeleteAll');
  try
    WParser.AddStr(ATable);
    WParser.Call;

    Result := WParser.GetInt;
  finally
    WParser.Free;
  end;
end;

function TMonS3APIDataBase._CreateParser(const AFunc: string; AIgnoreErrors: boolean = false): _TMonS3APIParser;
begin
  Result := TMonS3APIDataMain(FParent)._CreateParser(AFunc, FObjectGUID, AIgnoreErrors);
end;

procedure TMonS3APIDataBase._AddWhat(AParser: _TMonS3APIParser; AWhat: TMonS3APIDataWhatList);
var
  i: integer;
begin
  AParser.AddArray(AWhat.Count);
  for i := 0 to AWhat.Count-1 do
  begin
    AParser.AddStr(AWhat[i].Value);
  end;
end;

procedure TMonS3APIDataBase._AddWhere(AParser: _TMonS3APIParser; AWhere: TMonS3APIDataWhereList);
var
  i: integer;
  WItem: TMonS3APIDataWhere;
begin
  if not Assigned(AWhere) then
  begin
    AParser.AddArray(0);
    exit;
  end;

  AParser.AddArray(AWhere.Count);
  for i := 0 to AWhere.Count-1 do
  begin
    WItem := AWhere[i];

    AParser.AddByte(byte(WItem.TType));

    if WItem.TType <> MonS3APIDataWhereType_Where then
    begin
      continue;
    end;

    AParser.AddStr(WItem.Column);
    AParser.AddByte(byte(WItem.OOperator));

    _AddValues(AParser, WItem.Values);
  end;
end;

procedure TMonS3APIDataBase._AddSort(AParser: _TMonS3APIParser; ASort: TMonS3APIDataSortList);
var
  i: integer;
  WItem: TMonS3APIDataSort;
begin
  if not Assigned(ASort) then
  begin
    AParser.AddArray(0);
    exit;
  end;

  AParser.AddArray(ASort.Count);
  for i := 0 to ASort.Count-1 do
  begin
    WItem := ASort[i];
    AParser.AddStr(WItem.Column);
    AParser.AddByte(byte(WItem.TType));
  end;
end;

procedure TMonS3APIDataBase._AddDataValues(AParser: _TMonS3APIParser; AValues: TMonS3APIDataValueList);
var
  i: integer;
  WItem: TMonS3APIDataValue;
begin
  AParser.AddArray(AValues.Count);
  for i := 0 to AValues.Count-1 do
  begin
    WItem := AValues[i];
    AParser.AddStr(WItem.Column);
    _AddValue(AParser, WItem.Value);
  end;
end;

procedure TMonS3APIDataBase._AddValues(AParser: _TMonS3APIParser; AValues: TMonS3APIValueList);
var
  i: integer;
begin
  AParser.AddArray(AValues.Count);
  for i := 0 to AValues.Count-1 do
  begin
    _AddValue(AParser, AValues[i]);
  end;
end;

procedure TMonS3APIDataBase._AddValue(AParser: _TMonS3APIParser; AValue: TMonS3APIValue);
begin
  case AValue.ValType of
    _MonS3APIParserValType_Int: AParser.AddInt(AValue.AsInt);
    _MonS3APIParserValType_Int64: AParser.AddInt64(AValue.AsInt64);
    _MonS3APIParserValType_Ext: AParser.AddExt(AValue.AsExt);
    _MonS3APIParserValType_Str: AParser.AddStr(AValue.AsString);
    _MonS3APIParserValType_Bool: AParser.AddBool(AValue.AsBool);
    _MonS3APIParserValType_Date: AParser.AddDate(AValue.AsDate);
    _MonS3APIParserValType_Time: AParser.AddTime(AValue.AsTime);
    _MonS3APIParserValType_DateTime: AParser.AddDateTime(AValue.AsDateTime);
    _MonS3APIParserValType_Data: AParser.AddData(AValue.AsData);
    else
      AParser.AddEmpty;
  end;
end;

function TMonS3APIDataBase._ParseAgendaInfo(AParser: _TMonS3APIParser): TMonS3APIAgenda;
var
  WVisibleName: string;
  WName: string;
  WIC: string;
  WRight: TMonS3APIRight;
  WExt: string;
  WIsDemo: boolean;
  WPoznamka: string;
begin
  WVisibleName := AParser.GetStr;
  WName := AParser.GetStr;
  WIC := AParser.GetStr;
  WRight := TMonS3APIRight(AParser.GetByte);
  WExt := AParser.GetStr;
  WIsDemo := AParser.GetBool;
  WPoznamka := AParser.GetStr;

  Result := TMonS3APIAgenda.Create(WVisibleName, WName, WIC, WRight, WExt, WIsDemo, WPoznamka);
end;

function TMonS3APIDataProgram.GetLoginInformation: TMonS3APILoginInformation;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('GetLoginInformation');
  try
    WParser.Call;
    Result := TMonS3APILoginInformation(WParser.GetByte);
  finally
    WParser.Free;
  end;
end;

function TMonS3APIDataProgram.LoginDialog(AParentForm: HWND): string;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('LoginDialog');
  try
    {$IFDEF WIN64}
      WParser.AddInt64(AParentForm);
    {$ELSE}
      WParser.AddInt(AParentForm);
    {$ENDIF}

    WParser.Call;

    Result := WParser.GetStr;
  finally
    WParser.Free;
  end;
end;

function TMonS3APIDataProgram.TranslatePassword(const APassword: string): string;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('TranslatePassword');
  try
    WParser.AddStr(APassword);
    WParser.Call;

    Result := WParser.GetStr;
  finally
    WParser.Free;
  end;
end;

function TMonS3APIDataProgram.Login(const APassword: string): boolean;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('Login');
  try
    WParser.AddStr(APassword);
    WParser.Call;

    Result := WParser.GetBool;
  finally
    WParser.Free;
  end;
end;

procedure TMonS3APIDataProgram.Logout;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('Logout');
  try
    WParser.Call;
  finally
    WParser.Free;
  end;
end;

procedure TMonS3APIDataProgram.GetListAgend(AOutput: TMonS3APIAgendaList);
var
  WParser: _TMonS3APIParser;
  WCount: integer;
  i: integer;
begin
  WParser := _CreateParser('GetListAgend');
  try
    WParser.Call;

    WCount := WParser.GetArray;
    AOutput.Capacity := WCount;

    for i := 0 to WCount-1 do
    begin
      AOutput.Add(_ParseAgendaInfo(WParser));
    end;
  finally
    WParser.Free;
  end;
end;

function TMonS3APIDataProgram.GetAgendaInfo(const AAgendaExt: string): TMonS3APIAgenda;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('ProgramGetAgendaInfo');
  try
    WParser.AddStr(AAgendaExt);
    WParser.Call;

    Result := _ParseAgendaInfo(WParser);
  finally
    WParser.Free;
  end;
end;

function TMonS3APIDataProgram.GetUserInfo: TMonS3APIUserInfo;
var
  WParser: _TMonS3APIParser;
  WGUID: string;
  WID: integer;
  WJmeno: string;
  WConfigName: string;
  WPoznamka: string;
begin
  WParser := _CreateParser('GetUserInfo');
  try
    WParser.Call;

    WGUID := WParser.GetStr;
    WID := WParser.GetInt;
    WJmeno := WParser.GetStr;
    WConfigName := WParser.GetStr;
    WPoznamka := WParser.GetStr;

    Result := TMonS3APIUserInfo.Create(WGUID, WID, WJmeno, WConfigName, WPoznamka);
  finally
    WParser.Free;
  end;
end;

procedure TMonS3APIDataProgram.GetUserRights(AOutput: TMonS3APIUserRightList);
var
  WParser: _TMonS3APIParser;
  WCount: integer;
  i: integer;
  WRightName: string;
  WRightValue: string;
begin
  WParser := _CreateParser('GetUserRights');
  try
    WParser.Call;

    WCount := WParser.GetArray;
    AOutput.Capacity := WCount;

    for i := 0 to WCount-1 do
    begin
      WRightName := WParser.GetStr;
      WRightValue := WParser.GetStr;

      AOutput.Add(TMonS3APIUserRight.Create(WRightName, WRightValue));
    end;
  finally
    WParser.Free;
  end;
end;

procedure TMonS3APIDataAgenda.SetAgenda(const AAgendaExt: string);
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('SetAgenda');
  try
    WParser.AddStr(AAgendaExt);
    WParser.Call;
  finally
    WParser.Free;
  end;
end;

function TMonS3APIDataAgenda.GetAgendaInfo: TMonS3APIAgenda;
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('AgendaGetAgendaInfo');
  try
    WParser.Call;

    Result := _ParseAgendaInfo(WParser);
  finally
    WParser.Free;
  end;
end;

procedure TMonS3APIDataAgenda.GetYearList(AOutput: TMonS3APIYearList);
var
  WParser: _TMonS3APIParser;
  WCount: integer;
  i: integer;
  WYear: integer;
  WDateFrom: TDateTime;
  WDateTo: TDateTime;
  WRight: TMonS3APIRight;
  WExt: string;
begin
  WParser := _CreateParser('GetYearList');
  try
    WParser.Call;

    WCount := WParser.GetArray;
    AOutput.Capacity := WCount;
    for i := 0 to WCount-1 do
    begin
      WYear := WParser.GetWord;
      WDateFrom := WParser.GetDate;
      WDateTo := WParser.GetDate;
      WRight := TMonS3APIRight(WParser.GetByte);
      WExt := WParser.GetStr;

      AOutput.Add(TMonS3APIYear.Create(WYear, WDateFrom, WDateTo, WRight, WExt));
    end;
  finally
    WParser.Free;
  end;
end;

procedure TMonS3APIDataDokumenty.SetAgenda(const AAgendaExt: string);
var
  WParser: _TMonS3APIParser;
begin
  WParser := _CreateParser('SetAgenda');
  try
    WParser.AddStr(AAgendaExt);
    WParser.Call;
  finally
    WParser.Free;
  end;
end;

constructor _TMonS3APIParser.Create(AParent: TObject; const AClientGUID: TBytes; const AObjectGUID: TBytes; const AFunc: string; AIgnoreErrors: boolean);
begin
  FParent := AParent;
  FIgnoreErrors := AIgnoreErrors;

  //Nastavit formát desetinných èísel.
  GetLocaleFormatSettings(LOCALE_SYSTEM_DEFAULT, FFormatSettings);
  FFormatSettings.DecimalSeparator := '.';
  FFormatSettings.ThousandSeparator := #0;
  FFormatSettings.CurrencyString := '';
  FFormatSettings.CurrencyFormat := 1;
  FFormatSettings.CurrencyDecimals := 4;

  SetLength(FOutputBuffer, 1024);

  //Velikost pole.
  _WriteCardinal(0);

  if Assigned(AClientGUID) then
  begin
    AddData(AClientGUID);
  end
  else
  begin
    AddData(EmptyGUID);
  end;

  if Assigned(AObjectGUID) then
  begin
    AddData(AObjectGUID);
  end
  else
  begin
    AddData(EmptyGUID);
  end;

  AddStr(AFunc);
end;

function _TMonS3APIParser.Skip(ASize: Cardinal): integer;
begin
  Result := FInputPos;
  inc(FInputPos, ASize);
end;

procedure _TMonS3APIParser.CheckCapacity(Cap: Cardinal);
begin
  inc(Cap, FOutputPos);
  if Cap > Cardinal(Length(FOutputBuffer)) then
  begin
    SetLength(FOutputBuffer, Cap + 1024);
  end;
end;

function _TMonS3APIParser.GetBool: boolean;
begin
  Skip(SizeOf(word));
  Result := _ReadBool;
end;

function _TMonS3APIParser.GetByte: byte;
begin
  Skip(SizeOf(word));
  Result := _ReadByte;
end;

function _TMonS3APIParser.GetWord: word;
begin
  Skip(SizeOf(word));
  Result := _ReadWord;
end;

function _TMonS3APIParser.GetInt: LongInt; //Int32
begin
  Skip(SizeOf(word));
  Result := _ReadInt;
end;

function _TMonS3APIParser.GetInt64: Int64;
begin
  Skip(SizeOf(word));
  Result := _ReadInt64;
end;

function _TMonS3APIParser.GetCardinal: Cardinal;
begin
  Skip(SizeOf(word));
  Result := _ReadCardinal;
end;

function _TMonS3APIParser.GetExt: extended;
begin
  Skip(SizeOf(word));
  Result := _ReadExt;
end;

function _TMonS3APIParser.GetStr: WideString; //string
begin
  Skip(SizeOf(word));
  Result := _ReadStr;
end;

function _TMonS3APIParser.GetDate: TDateTime;
begin
  Skip(SizeOf(word));
  Result := _ReadDate;
end;

function _TMonS3APIParser.GetTime: TDateTime;
begin
  Skip(SizeOf(word));
  Result := _ReadTime;
end;

function _TMonS3APIParser.GetDateTime: TDateTime;
begin
  Skip(SizeOf(word));
  Result := _ReadDateTime;
end;

function _TMonS3APIParser.GetData: TBytes;
begin
  Skip(SizeOf(word));
  Result := _ReadData;
end;

function _TMonS3APIParser.GetArray: Cardinal;
begin
  Skip(SizeOf(word));
  Result := _ReadArray;
end;

function _TMonS3APIParser._ReadBool: boolean;
begin
  Result := _ReadByte = 255;
end;

function _TMonS3APIParser._ReadByte: byte;
begin
  Result := FInputBuffer[Skip(SizeOf(byte))];
end;

function _TMonS3APIParser._ReadWord: word;
begin
  CopyMemory(@Result, @FInputBuffer[Skip(SizeOf(Result))], SizeOf(Result));
end;

function _TMonS3APIParser._ReadInt: LongInt; //Int32
begin
  CopyMemory(@Result, @FInputBuffer[Skip(SizeOf(Result))], SizeOf(Result));
end;

function _TMonS3APIParser._ReadInt64: Int64;
begin
  CopyMemory(@Result, @FInputBuffer[Skip(SizeOf(Result))], SizeOf(Result));
end;

function _TMonS3APIParser._ReadCardinal: Cardinal;
begin
  CopyMemory(@Result, @FInputBuffer[Skip(SizeOf(Result))], SizeOf(Result));
end;

function _TMonS3APIParser._ReadExt: extended;
var
  WStr: string;
begin
  WStr := _ReadStr;
  Result := StrToFloat(WStr, FFormatSettings);
end;

function _TMonS3APIParser._ReadStr: WideString; //string
var
  WLen: integer;
begin
  WLen := _ReadCardinal;

  if WLen = 0 then
  begin
    Result := '';
    exit;
  end;

  SetLength(Result, WLen div SizeOf(WideChar));
  CopyMemory(@Result[1], @FInputBuffer[Skip(WLen)], WLen);
end;

function _TMonS3APIParser._ReadDate: TDateTime;
var
  WYear: word;
  WMonth: byte;
  WDay: byte;
begin
  WYear := _ReadWord;
  WMonth := _ReadByte;
  WDay := _ReadByte;

  if WYear < 1900 then
  begin
    Result := 0;
  end
  else
  begin
    Result := EncodeDate(WYear, WMonth, WDay);
  end;
end;

function _TMonS3APIParser._ReadTime: TDateTime;
var
  WHour: byte;
  WMin: byte;
  WSec: byte;
  WMSec: word;
begin
  WHour := _ReadByte;
  WMin := _ReadByte;
  WSec := _ReadByte;
  WMSec := _ReadWord;
  Result := EncodeTime(WHour, WMin, WSec, WMSec);
end;

function _TMonS3APIParser._ReadDateTime: TDateTime;
var
  WYear: word;
  WMonth: byte;
  WDay: byte;
  WHour: byte;
  WMin: byte;
  WSec: byte;
  WMSec: word;
begin
  WYear := _ReadWord;
  WMonth := _ReadByte;
  WDay := _ReadByte;
  WHour := _ReadByte;
  WMin := _ReadByte;
  WSec := _ReadByte;
  WMSec := _ReadWord;
  Result := EncodeDateTime(WYear, WMonth, WDay, WHour, WMin, WSec, WMSec);
end;

function _TMonS3APIParser._ReadData: TBytes;
var
  WLen: integer;
begin
  WLen := _ReadCardinal;

  if WLen = 0 then
  begin
    SetLength(Result, 0);
    exit;
  end;

  SetLength(Result, WLen);
  CopyMemory(@Result[0], @FInputBuffer[Skip(WLen)], WLen);
end;

function _TMonS3APIParser._ReadArray: Cardinal;
begin
  Result := _ReadCardinal;
end;

procedure _TMonS3APIParser.AddEmpty;
begin
  CheckCapacity(SizeOf(word));
  _WriteWord(word(_MonS3APIParserValType_Empty));
end;

procedure _TMonS3APIParser.AddBool(AVal: boolean);
begin
  CheckCapacity(SizeOf(word) + SizeOf(AVal));
  _WriteWord(word(_MonS3APIParserValType_Bool));
  _WriteBool(AVal);
end;

procedure _TMonS3APIParser.AddByte(AVal: byte);
begin
  CheckCapacity(SizeOf(word) + SizeOf(AVal));
  _WriteWord(word(_MonS3APIParserValType_Byte));
  _WriteByte(AVal);
end;

procedure _TMonS3APIParser.AddWord(AVal: word);
begin
  CheckCapacity(SizeOf(word) + SizeOf(AVal));
  _WriteWord(word(_MonS3APIParserValType_Word));
  _WriteWord(AVal);
end;

procedure _TMonS3APIParser.AddInt(AVal: LongInt); //Int32
begin
  CheckCapacity(SizeOf(word) + SizeOf(AVal));
  _WriteWord(word(_MonS3APIParserValType_Int));
  _WriteInt(AVal);
end;

procedure _TMonS3APIParser.AddInt64(AVal: Int64);
begin
  CheckCapacity(SizeOf(word) + SizeOf(AVal));
  _WriteWord(word(_MonS3APIParserValType_Int64));
  _WriteInt64(AVal);
end;

procedure _TMonS3APIParser.AddCardinal(AVal: Cardinal);
begin
  CheckCapacity(SizeOf(word) + SizeOf(AVal));
  _WriteWord(word(_MonS3APIParserValType_Cardinal));
  _WriteCardinal(AVal);
end;

procedure _TMonS3APIParser.AddExt(AVal: extended);
begin
  CheckCapacity(SizeOf(word));
  _WriteWord(word(_MonS3APIParserValType_Ext));
  _WriteExt(AVal);
end;

procedure _TMonS3APIParser.AddCurr(AVal: currency);
begin
  CheckCapacity(SizeOf(word));
  _WriteWord(word(_MonS3APIParserValType_Curr));
  _WriteCurr(AVal);
end;

procedure _TMonS3APIParser.AddStr(const AVal: WideString); //string
begin
  CheckCapacity(SizeOf(word));
  _WriteWord(word(_MonS3APIParserValType_Str));
  _WriteStr(AVal);
end;

procedure _TMonS3APIParser.AddDate(AVal: TDateTime);
begin
  CheckCapacity(SizeOf(word) + SizeOf(word) + SizeOf(byte) + SizeOf(byte));
  _WriteWord(word(_MonS3APIParserValType_Date));
  _WriteDate(AVal);
end;

procedure _TMonS3APIParser.AddTime(AVal: TDateTime);
begin
  CheckCapacity(SizeOf(word) + SizeOf(byte) + SizeOf(byte) + SizeOf(byte) + SizeOf(word));
  _WriteWord(word(_MonS3APIParserValType_Time));
  _WriteTime(AVal);
end;

procedure _TMonS3APIParser.AddDateTime(AVal: TDateTime);
begin
  CheckCapacity(SizeOf(word) + SizeOf(word) + SizeOf(byte) + SizeOf(byte) + SizeOf(byte) + SizeOf(byte) + SizeOf(byte) + SizeOf(word));
  _WriteWord(word(_MonS3APIParserValType_DateTime));
  _WriteDateTime(AVal);
end;

procedure _TMonS3APIParser.AddData(const AVal: TBytes);
begin
  CheckCapacity(SizeOf(word) + SizeOf(cardinal) + Length(AVal));
  _WriteWord(word(_MonS3APIParserValType_Data));
  _WriteData(AVal);
end;

procedure _TMonS3APIParser.AddArray(ACount: Cardinal);
begin
  CheckCapacity(SizeOf(word) + SizeOf(cardinal));
  _WriteWord(word(_MonS3APIParserValType_Array));
  _WriteArray(ACount);
end;

procedure _TMonS3APIParser._WriteBool(AVal: boolean);
begin
  if AVal then
  begin
    _WriteByte(255);
  end
  else
  begin
    _WriteByte(0);
  end;
end;

procedure _TMonS3APIParser._WriteByte(AVal: byte);
begin
  CopyMemory(@FOutputBuffer[FOutputPos], @AVal, SizeOf(AVal));
  inc(FOutputPos, SizeOf(AVal));
end;

procedure _TMonS3APIParser._WriteWord(AVal: word);
begin
  CopyMemory(@FOutputBuffer[FOutputPos], @AVal, SizeOf(AVal));
  inc(FOutputPos, SizeOf(AVal));
end;

procedure _TMonS3APIParser._WriteInt(AVal: LongInt); //Int32
begin
  CopyMemory(@FOutputBuffer[FOutputPos], @AVal, SizeOf(AVal));
  inc(FOutputPos, SizeOf(AVal));
end;

procedure _TMonS3APIParser._WriteInt64(AVal: Int64);
begin
  CopyMemory(@FOutputBuffer[FOutputPos], @AVal, SizeOf(AVal));
  inc(FOutputPos, SizeOf(AVal));
end;

procedure _TMonS3APIParser._WriteCardinal(AVal: Cardinal);
begin
  CopyMemory(@FOutputBuffer[FOutputPos], @AVal, SizeOf(AVal));
  inc(FOutputPos, SizeOf(AVal));
end;

procedure _TMonS3APIParser._WriteExt(AVal: extended);
begin
  _WriteStr(FloatToStr(AVal, FFormatSettings));
end;

procedure _TMonS3APIParser._WriteCurr(AVal: currency);
begin
  _WriteStr(CurrToStr(AVal, FFormatSettings));
end;

procedure _TMonS3APIParser._WriteStr(const AVal: WideString); //string
var
  WLen: integer;
begin
  WLen := Length(AVal) * SizeOf(WideChar);
  CheckCapacity(SizeOf(Cardinal) + WLen);
  _WriteCardinal(WLen);

  if WLen = 0 then
  begin
    exit;
  end;

  CopyMemory(@FOutputBuffer[FOutputPos], @AVal[1], WLen);
  inc(FOutputPos, WLen);
end;

procedure _TMonS3APIParser._WriteDate(AVal: TDateTime);
var
  WYear: word;
  WMonth: word;
  WDay: word;
begin
  DecodeDate(AVal, WYear, WMonth, WDay);
  _WriteWord(WYear);
  _WriteByte(WMonth);
  _WriteByte(WDay);
end;

procedure _TMonS3APIParser._WriteTime(AVal: TDateTime);
var
  WHour: word;
  WMin: word;
  WSec: word;
  WMSec: word;
begin
  DecodeTime(AVal, WHour, WMin, WSec, WMSec);
  _WriteByte(WHour);
  _WriteByte(WMin);
  _WriteByte(WSec);
  _WriteWord(WMSec);
end;

procedure _TMonS3APIParser._WriteDateTime(AVal: TDateTime);
begin
  _WriteDate(AVal);
  _WriteTime(AVal);
end;

procedure _TMonS3APIParser._WriteData(const AVal: TBytes);
var
  WLen: integer;
begin
  WLen := Length(AVal);
  _WriteCardinal(WLen);

  if WLen = 0 then
  begin
    exit;
  end;

  CopyMemory(@FOutputBuffer[FOutputPos], @AVal[0], WLen);
  inc(FOutputPos, WLen);
end;

procedure _TMonS3APIParser._WriteArray(ACount: Cardinal);
begin
  _WriteCardinal(ACount);
end;

procedure _TMonS3APIParser.Call;
var
  WRes: Pointer;
  WCount: Cardinal;
  WError: Cardinal;
  WText: string;
begin
  try
    CopyMemory(@FOutputBuffer[0], @FOutputPos, SizeOf(FOutputPos));

    WRes := TMonS3APIDataMain(FParent)._Call(@FOutputBuffer[0]);

    CopyMemory(@WCount, WRes, SizeOf(WCount));
    SetLength(FInputBuffer, WCount);
    CopyMemory(@FInputBuffer[0], WRes, WCount);

    TMonS3APIDataMain(FParent)._CallFree(WRes);

    FInputPos := SizeOf(WCount);

    WError := _ReadCardinal;
  except
    if FIgnoreErrors then
    begin
      exit;
    end;

    raise;
  end;

  //Bez chyby.
  if FIgnoreErrors or (WError = 0) then
  begin
    exit;
  end;

  WText := _ReadStr;
  raise TMonS3APIException.Create(TMonS3APIExceptionType(WError), WText);
end;

initialization
  SetLength(EmptyGUID, 16);
  ZeroMemory(@EmptyGUID[0], 16);

end.
