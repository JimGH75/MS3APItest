unit MonS3APITestUnit;

interface

uses
  SysUtils, Classes,

  MonS3APIUnit;

const
  CMonS3APITest_DLLFile32 = 'D:\MonS3API\API\MonS3API_32.dll';
  CMonS3APITest_DLLFile64 = 'D:\MonS3API\API\MonS3API_64.dll';
  CMonS3APITest_DataFolder = 'D:\MonS3API\Data\';
  CMonS3APITest_Password = 'a'; //NIKDY NEUKLÁDEJTE takto HESLA, viz dokumentace
  CMonS3APITest_AppName = 'Testovaci Aplikace';

type
  TMonS3APITest = class
  public
    procedure Run;
  end;

  //Základní pøipojení k DLL knihovnì, datùm, pøihlášení se.
  TMonS3APITest1 = class
  public
    procedure Run;
  end;

  //Získání struktury tabulek a pøeètení dat.
  TMonS3APITest2 = class
  public
    procedure Run;
  end;

  //Opakovaná práce s daty.
  TMonS3APITest3 = class
  public
    procedure Run;
  end;

  //Více vláken.
  TMonS3APITest4 = class
  private
    FMain: TMonS3APIDataMain;
    procedure CreateThread;
    procedure ThreadWork;
    procedure DoWork(AProgram: TMonS3APIDataProgram);
  public
    procedure Run;
  end;

  //Složité podmínky a øazení.
  TMonS3APITest5 = class
  public
    procedure Run;
  end;

  //Zda øádek existuje.
  TMonS3APITest6 = class
  public
    procedure Run;
  end;

  //Poèet øádkù.
  TMonS3APITest7 = class
  public
    procedure Run;
  end;

  //Pøidání øádku.
  //Nelze použít s knihovnou MonS3Reader
  TMonS3APITest8 = class
  public
    procedure Run;
  end;

  //Aktualizace øádku.
  //Nelze použít s knihovnou MonS3Reader
  TMonS3APITest9 = class
  public
    procedure Run;
  end;

  //Smazání øádku.
  //Nelze použít s knihovnou MonS3Reader
  TMonS3APITest10 = class
  public
    procedure Run;
  end;

  //Transakce.
  //Nelze použít s knihovnou MonS3Reader
  TMonS3APITest11 = class
  private
    procedure AddRow(AProgram: TMonS3APIDataProgram; const APopis: string);
  public
    procedure Run;
  end;

  //Pøipojení se k agendì.
  TMonS3APITest12 = class
  public
    procedure Run;
  end;

  //Získat seznam rokù agendy.
  TMonS3APITest13 = class
  public
    procedure Run;
  end;

  //Práce s daty agendy.
  TMonS3APITest14 = class
  public
    procedure Run;
  end;

  //Transakce v agendì.
  //Nelze použít s knihovnou MonS3Reader
  TMonS3APITest15 = class
  private
    procedure AddRow(AAgenda: TMonS3APIDataAgenda; const APopis: string);
  public
    procedure Run;
  end;

  //Práce s jednou agendou ve vláknech.
  TMonS3APITest16 = class
  private
    FMain: TMonS3APIDataMain;
    procedure CreateThread;
    procedure ThreadWork;
    procedure DoWork(AAgenda: TMonS3APIDataAgenda);
  public
    procedure Run;
  end;

  //Práce s více agendami najednou.
  TMonS3APITest17 = class
  public
    procedure Run;
  end;

  //Select z Dokumenty.s3db.
  TMonS3APITest18 = class
  public
    procedure Run;
  end;

implementation

function GetDLLFile: string;
begin
  {$IFDEF WIN64}
    Result := CMonS3APITest_DLLFile64;
  {$ELSE}
    Result := CMonS3APITest_DLLFile32;
  {$ENDIF}
end;

procedure TMonS3APITest.Run;
var
  WTest1: TMonS3APITest1;
  WTest2: TMonS3APITest2;
  WTest3: TMonS3APITest3;
  WTest4: TMonS3APITest4;
  WTest5: TMonS3APITest5;
  WTest6: TMonS3APITest6;
  WTest7: TMonS3APITest7;
  WTest8: TMonS3APITest8;
  WTest9: TMonS3APITest9;
  WTest10: TMonS3APITest10;
  WTest11: TMonS3APITest11;
  WTest12: TMonS3APITest12;
  WTest13: TMonS3APITest13;
  WTest14: TMonS3APITest14;
  WTest15: TMonS3APITest15;
  WTest16: TMonS3APITest16;
  WTest17: TMonS3APITest17;
  WTest18: TMonS3APITest18;
begin
  WTest1 := TMonS3APITest1.Create;
  try
    WTest1.Run;
  finally
    WTest1.Free;
  end;

  WTest2 := TMonS3APITest2.Create;
  try
    WTest2.Run;
  finally
    WTest2.Free;
  end;

  WTest3 := TMonS3APITest3.Create;
  try
    WTest3.Run;
  finally
    WTest3.Free;
  end;

  WTest4 := TMonS3APITest4.Create;
  try
    WTest4.Run;
  finally
    WTest4.Free;
  end;

  WTest5 := TMonS3APITest5.Create;
  try
    WTest5.Run;
  finally
    WTest5.Free;
  end;

  WTest6 := TMonS3APITest6.Create;
  try
    WTest6.Run;
  finally
    WTest6.Free;
  end;

  WTest7 := TMonS3APITest7.Create;
  try
    WTest7.Run;
  finally
    WTest7.Free;
  end;

  WTest8 := TMonS3APITest8.Create;
  try
    WTest8.Run;
  finally
    WTest8.Free;
  end;

  WTest9 := TMonS3APITest9.Create;
  try
    WTest9.Run;
  finally
    WTest9.Free;
  end;

  WTest10 := TMonS3APITest10.Create;
  try
    WTest10.Run;
  finally
    WTest10.Free;
  end;

  WTest11 := TMonS3APITest11.Create;
  try
    WTest11.Run;
  finally
    WTest11.Free;
  end;

  WTest12 := TMonS3APITest12.Create;
  try
    WTest12.Run;
  finally
    WTest12.Free;
  end;

  WTest13 := TMonS3APITest13.Create;
  try
    WTest13.Run;
  finally
    WTest13.Free;
  end;

  WTest14 := TMonS3APITest14.Create;
  try
    WTest14.Run;
  finally
    WTest14.Free;
  end;

  WTest15 := TMonS3APITest15.Create;
  try
    WTest15.Run;
  finally
    WTest15.Free;
  end;

  WTest16 := TMonS3APITest16.Create;
  try
    WTest16.Run;
  finally
    WTest16.Free;
  end;

  WTest17 := TMonS3APITest17.Create;
  try
    WTest17.Run;
  finally
    WTest17.Free;
  end;

  WTest18 := TMonS3APITest18.Create;
  try
    WTest18.Run;
  finally
    WTest18.Free;
  end
end;

procedure TMonS3APITest1.Run;
var
  WMain: TMonS3APIDataMain;
  WDLLFile: string;
  WDataFolder: string;
  WProgram: TMonS3APIDataProgram;
  WLogInfo: TMonS3APILoginInformation;
  WPass: string;
  WUserInfo: TMonS3APIUserInfo;
  WUserRights: TMonS3APIUserRightList;
begin
  //Instance pro práci s daty.
  //Ideálnì pro jedny data používat jednu instanci.
  //Pro více vláken se využije tato jedna instance, ze které se vytváøejí instance pro program a agendy pro každé vlákno zvláš.
  WMain := TMonS3APIDataMain.Create;
  try
    //Hledání MonS3API.dll
    WDLLFile := WMain.FindDLL;

    //Hledání MonS3Reader.dll
    WDLLFile := WMain.FindDLL(MonS3Reader);

    //Popøípadì mùžeme použít vlastní cestu.
    WDLLFile := GetDLLFile;

    //Pøipojení se k DLL knihovnì.
    //Pøedáme jí název naší aplikace.
    WMain.LoadDLL(WDLLFile, CMonS3APITest_AppName);

    //Hledání dat MoneyS3.
    WDataFolder := WMain.FindData;

    //Popøípadì mùžeme použít vlastní cestu.
    WDataFolder := CMonS3APITest_DataFolder;

    //Nastavení cesty k datùm.
    WMain.SetDataPath(WDataFolder);

    //Instance pro program.
    //Jedna instance pro jedno vlákno.
    //V rámci jednoho vlákna je zbyteèné vytváøet více instancí.
    WProgram := WMain.GetProgramInstance;
    try
      //Pøed jakýkoliv dalším volání je potøeba se pøipojit k datùm.
      WProgram.ConnectData;

      //Blok pøihlašování:
      begin
        //Zjištìní informace o možnostech pøihlášení.
        WLogInfo := WProgram.GetLoginInformation;

        case WLogInfo of
          MonS3APILoginInformation_Password:
            begin
              //Potøebuji heslo!
            end;
          MonS3APILoginInformation_WindowsNO:
            begin
              //Není se možné pøihlásit.
            end;
          MonS3APILoginInformation_WindowsPassword:
            begin
              //Buïto vložím prázdné heslo a to pøihlásí pomocí uživatele a nebo zažádám o heslo - popøípadì dám uživateli na výbìr.
            end;
          MonS3APILoginInformation_WindowsYES:
            begin
              //Vložím prázdné heslo do funkce Login a nic neøeším.
            end;
          MonS3APILoginInformation_WithoutPassword:
            begin
              //Vložím prázdné heslo do funkce Login a nic neøeším.
            end;
        end;

        //Mùžu uživatele požádat o zadání hesla.
        //WPass := WProgram.LoginDialog(0);
        //Pokud jsem ve windows form aplikaci, tak doporuèuji vyplnit vstup Handlem okna.
        //Popøípadì:
        //WPass := WProgram.LoginDialog(Application.MainForm.Handle);

        //Pokud potøebuji heslo a znám ho z minula, tak jen pøeložím.
        WPass := CMonS3APITest_Password;
        WPass := WProgram.TranslatePassword(WPass);
        //NIKDY NEUKLÁDEJTE HESLA, ale vždy tento hash.
        //Takže pùvodní heslo je dobré smazat a nahradit tímto.

        //Samotné pøihlášení - buïto pomocí hesla a nebo pokud je to windows pøihlášení nebo bez hesla, tak prázdný string.
        //Funkce vrací, zda se to povedlo.
        WProgram.Login(WPass);

        //Nyní lze získat informace o uživateli.
        WUserInfo := WProgram.GetUserInfo;
        WUserInfo.Free;

        //A nebo seznam všech práv.
        WUserRights := TMonS3APIUserRightList.Create;
        try
          WProgram.GetUserRights(WUserRights);
        finally
          WUserRights.Free;
        end;
      end;
      //Konec bloku pøihlašování.

      //xxx
      //Odteï je možné volat další funkce.
      //xxx

      //Jakmile nepotøebujeme pøipojení, mùžem se odpojit.
      //Nebo staèí objekt uvolnit, k odpojení dojde automaticky.
      WProgram.DisconnectData;
    finally
      //Jen pro ukázku, objekty není tøeba uvolòovat hned.
      //Jenom je dobré je odpojovat od dat funkcí DisconnectData.
      WProgram.Free;
    end;
  finally
    //Ideálnì po ukonèení veškeré práce uvolnit knihovnu.
    //Pokud zavoláme tohle, tak nemusíme pøedtím volat DisconnectData.
    //WMain.UnLoadDLL;

    //V tomto pøípadì rovnou uvolníme celý objekt, což volá jak DisconnectData, tak UnLoadDLL.
    WMain.Free;
  end;
end;

procedure TMonS3APITest2.Run;
var
  WMain: TMonS3APIDataMain;
  WProgram: TMonS3APIDataProgram;
  WTableList: TMonS3APITableList;
  WTable: TMonS3APITable;
  WColumnList: TMonS3APITableColumnList;
  WWhat: TMonS3APIDataWhatList;
  WWhere: TMonS3APIDataWhereList;
  WSelectResult: TMonS3APIDataSelectResult;
  WPopis: string;
  WUzivatel: string;
  i: integer;
begin
  WMain := TMonS3APIDataMain.Create;
  try
    WMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
    WMain.SetDataPath(CMonS3APITest_DataFolder);

    WProgram := WMain.GetProgramInstance;
    try
      WProgram.ConnectData;
      WProgram.Login(WProgram.TranslatePassword(CMonS3APITest_Password));

      //Seznam tabulek.
      //Pokud potøebuji znát ten seznam a nebo si chci ovìøit na co mám právo.
      WTableList := TMonS3APITableList.Create;
      try
        WProgram.GetTableList(WTableList);

        //Informace o tabulce, pokud je potøeba.
        WTable := WTableList.GetByName('GlAkce');
      finally
        WTableList.Free;
      end;

      //Seznam sloupeèkù tabulky.
      //Jenom pokud to k nìèemu potøebuji.
      //Konkrétní sloupeèek lze najít pomocí GetByName popøípadì IDèko rovnou GetIDColumn.
      WColumnList := TMonS3APITableColumnList.Create;
      try
        WProgram.GetTableColumns('GlAkce', WColumnList);
      finally
        WColumnList.Free;
      end;

      WWhat := TMonS3APIDataWhatList.Create;
      WWhere := TMonS3APIDataWhereList.Create;
      WSelectResult := TMonS3APIDataSelectResult.Create;
      try
        //Pro funkci GetRows musí být vždy zavoláno AddAll.
        //V budoucnu bude možnost vybírat sloupeèky atd.
        WWhat.AddAll;

        //Podmínka na datum.
        WWhere.AddWhere('Datum', MonS3APIDataWhereOperator_Equals, TMonS3APIValue.CreateDate(Now));

        //Uložíme výstup.
        WProgram.GetRows('GlAkce', WWhat, WSelectResult, WWhere);

        //Dále nevolám žádnou funkci, takže je možné se od dat odpojit.
        //Ale není to nutnost.
        WProgram.DisconnectData;

        //Zpracovat data - možnost è 1.
        while WSelectResult.Next do
        begin
          //Pøeète hodnoty.
          WPopis := WSelectResult.GetColByName('Popis').AsString;
          WUzivatel := WSelectResult.GetColByName('Uzivatel').AsString;

          //nìco s daty udìlat
        end;

        //Zpracovat data - možnost è 2.
        //Zakomentováno, nemùže se volat po tom prvním prùchodu.
        (*
        for i := 0 to WSelectResult.Count-1 do
        begin
          WSelectResult.Next;

          //Pøeète hodnoty.
          WPopis := WSelectResult.GetColByName('Popis').AsString;
          WUzivatel := WSelectResult.GetColByName('Uzivatel').AsString;

          //nìco s daty udìlat
        end;
        *)
      finally
        WWhat.Free;
        WWhere.Free;
        WSelectResult.Free;
      end;
    finally
      WProgram.Free;
    end;
  finally
    WMain.Free;
  end;
end;

procedure TMonS3APITest3.Run;
var
  WMain: TMonS3APIDataMain;
  WProgram: TMonS3APIDataProgram;
  WWhat: TMonS3APIDataWhatList;
  WWhere: TMonS3APIDataWhereList;
  WSelectResult: TMonS3APIDataSelectResult;
  i: integer;
  WPopis: string;
  WUzivatel: string;
begin
  WMain := TMonS3APIDataMain.Create;
  try
    WMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
    WMain.SetDataPath(CMonS3APITest_DataFolder);

    WProgram := WMain.GetProgramInstance;
    try
      WProgram.ConnectData;
      WProgram.Login(WProgram.TranslatePassword(CMonS3APITest_Password));

      //Možné uzavøení, ale není to nutnost.
      WProgram.DisconnectData;

      WWhat := TMonS3APIDataWhatList.Create;
      WWhere := TMonS3APIDataWhereList.Create;
      WSelectResult := TMonS3APIDataSelectResult.Create;
      try
        WWhat.AddAll;
        WWhere.AddWhere('Datum', MonS3APIDataWhereOperator_Equals, TMonS3APIValue.CreateDate(Now));

        for i := 0 to 4 do
        begin
          WProgram.ConnectData;
          WProgram.GetRows('GlAkce', WWhat, WSelectResult, WWhere);
          WProgram.DisconnectData;

          while WSelectResult.Next do
          begin
            WPopis := WSelectResult.GetColByName('Popis').AsString;
            WUzivatel := WSelectResult.GetColByName('Uzivatel').AsString;

            //nìco s daty udìlat
          end;

          Sleep(2000);
        end;
      finally
        WWhat.Free;
        WWhere.Free;
        WSelectResult.Free;
      end;
    finally
      WProgram.Free;
    end;
  finally
    WMain.Free;
  end;
end;

procedure TMonS3APITest4.Run;
var
  WProgram: TMonS3APIDataProgram;
begin
  FMain := TMonS3APIDataMain.Create;
  FMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
  FMain.SetDataPath(CMonS3APITest_DataFolder);

  WProgram := FMain.GetProgramInstance;
  try
    WProgram.ConnectData;
    WProgram.Login(WProgram.TranslatePassword(CMonS3APITest_Password));

    //Možné uzavøení, ale není to nutnost.
    WProgram.DisconnectData;

    //Vytvoøím 2 vlákna a sám to taky takhle zpracuju.
    CreateThread;
    CreateThread;
    DoWork(WProgram);
  finally
    //WProgram mùžu ukonèit, ve vláknech mají svoji instanci na spoleèná data.
    WProgram.Free;
  end;

  //Správnì by se mìl FMain po ukonèení všech vláken uvolnit.
  //Tento kód však slouží jen pro úèely ukázky použití a tak to nebudu øešit.
  //FMain.Free;
end;

function MonS3APITest4ThreadWork(Parameter: Pointer): Integer;
begin
  TMonS3APITest4(Parameter).ThreadWork;
  Result := 0;
end;

procedure TMonS3APITest4.CreateThread;
var
  id: LongWord;
begin
  BeginThread(nil, 0, MonS3APITest4ThreadWork, Self, 0, id);
end;

procedure TMonS3APITest4.ThreadWork;
var
  WProgram: TMonS3APIDataProgram;
begin
  WProgram := FMain.GetProgramInstance;
  try
    DoWork(WProgram);
  finally
    WProgram.Free;
  end;
end;

procedure TMonS3APITest4.DoWork(AProgram: TMonS3APIDataProgram);
var
  WWhat: TMonS3APIDataWhatList;
  WWhere: TMonS3APIDataWhereList;
  WSelectResult: TMonS3APIDataSelectResult;
  i: integer;
  WPopis: string;
  WUzivatel: string;
begin
  WWhat := TMonS3APIDataWhatList.Create;
  WWhere := TMonS3APIDataWhereList.Create;
  WSelectResult := TMonS3APIDataSelectResult.Create;
  try
    WWhat.AddAll;
    WWhere.AddWhere('Datum', MonS3APIDataWhereOperator_Equals, TMonS3APIValue.CreateDate(Now));

    for i := 0 to 4 do
    begin
      AProgram.ConnectData;
      AProgram.GetRows('GlAkce', WWhat, WSelectResult, WWhere);
      AProgram.DisconnectData;

      while WSelectResult.Next do
      begin
        WPopis := WSelectResult.GetColByName('Popis').AsString;
        WUzivatel := WSelectResult.GetColByName('Uzivatel').AsString;

        //nìco s daty udìlat
      end;

      Sleep(2000);
    end;
  finally
    WWhat.Free;
    WWhere.Free;
    WSelectResult.Free;
  end;
end;

procedure TMonS3APITest5.Run;
var
  WMain: TMonS3APIDataMain;
  WProgram: TMonS3APIDataProgram;
  WWhat: TMonS3APIDataWhatList;
  WWhere: TMonS3APIDataWhereList;
  WSort: TMonS3APIDataSortList;
  WSelectResult: TMonS3APIDataSelectResult;
  WPopis: string;
  WUzivatel: string;
begin
  WMain := TMonS3APIDataMain.Create;
  try
    WMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
    WMain.SetDataPath(CMonS3APITest_DataFolder);

    WProgram := WMain.GetProgramInstance;
    try
      WProgram.ConnectData;
      WProgram.Login(WProgram.TranslatePassword(CMonS3APITest_Password));

      WWhat := TMonS3APIDataWhatList.Create;
      WWhere := TMonS3APIDataWhereList.Create;
      WSort := TMonS3APIDataSortList.Create;
      WSelectResult := TMonS3APIDataSelectResult.Create;
      try
        WWhat.AddAll;

        //Podmínka:
        //(Uzivatel = "a" || Uzivatel = "b") && Datum >= Now
        WWhere.AddBracketStart;
          WWhere.AddWhere('Uzivatel', MonS3APIDataWhereOperator_Equals, TMonS3APIValue.CreateString('a'));
          WWhere.AddOperatorOR;
          WWhere.AddWhere('Uzivatel', MonS3APIDataWhereOperator_Equals, TMonS3APIValue.CreateString('b'));
        WWhere.AddBracketEnd;
        WWhere.AddOperatorAND;
        WWhere.AddWhere('Datum', MonS3APIDataWhereOperator_GreaterEq, TMonS3APIValue.CreateDate(Now));

        //V tomto pøípadì lze ale i použít:
        //WWhere.Clear;
        //WWhere.AddWhere('Uzivatel', MonS3APIDataWhereOperator_InValues, [TMonS3APIValue.CreateString('a'), TMonS3APIValue.CreateString('b')]);
        //WWhere.AddOperatorAND;
        //WWhere.AddWhere('Datum', MonS3APIDataWhereOperator_GreaterEq, TMonS3APIValue.CreateDate(Now));

        //Øadit podle Datum a pak Popis
        WSort.AddSort('Datum', MonS3APIDataSortType_Ascending);
        WSort.AddSort('Popis', MonS3APIDataSortType_Ascending);

        WProgram.GetRows('GlAkce', WWhat, WSelectResult, WWhere, WSort);

        while WSelectResult.Next do
        begin
          WPopis := WSelectResult.GetColByName('Popis').AsString;
          WUzivatel := WSelectResult.GetColByName('Uzivatel').AsString;

          //nìco s daty udìlat
        end;
      finally
        WWhat.Free;
        WWhere.Free;
        WSort.Free;
        WSelectResult.Free;
      end;
    finally
      WProgram.Free;
    end;
  finally
    WMain.Free;
  end;
end;

procedure TMonS3APITest6.Run;
var
  WMain: TMonS3APIDataMain;
  WProgram: TMonS3APIDataProgram;
  WWhere: TMonS3APIDataWhereList;
  WExists: boolean;
begin
  WMain := TMonS3APIDataMain.Create;
  try
    WMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
    WMain.SetDataPath(CMonS3APITest_DataFolder);

    WProgram := WMain.GetProgramInstance;
    try
      WProgram.ConnectData;
      WProgram.Login(WProgram.TranslatePassword(CMonS3APITest_Password));

      WWhere := TMonS3APIDataWhereList.Create;
      try
        WWhere.AddWhere('Datum', MonS3APIDataWhereOperator_Equals, TMonS3APIValue.CreateDate(Now));
        WExists := WProgram.GetRowExists('GlAkce', WWhere);
      finally
        WWhere.Free;
      end;
    finally
      WProgram.Free;
    end;
  finally
    WMain.Free;
  end;
end;

procedure TMonS3APITest7.Run;
var
  WMain: TMonS3APIDataMain;
  WProgram: TMonS3APIDataProgram;
  WWhere: TMonS3APIDataWhereList;
  WCount: integer;
begin
  WMain := TMonS3APIDataMain.Create;
  try
    WMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
    WMain.SetDataPath(CMonS3APITest_DataFolder);

    WProgram := WMain.GetProgramInstance;
    try
      WProgram.ConnectData;
      WProgram.Login(WProgram.TranslatePassword(CMonS3APITest_Password));

      WWhere := TMonS3APIDataWhereList.Create;
      try
        WWhere.AddWhere('Datum', MonS3APIDataWhereOperator_Equals, TMonS3APIValue.CreateDate(Now));
        WCount := WProgram.GetRowCount('GlAkce', WWhere);
      finally
        WWhere.Free;
      end;
    finally
      WProgram.Free;
    end;
  finally
    WMain.Free;
  end;
end;

procedure TMonS3APITest8.Run;
var
  WMain: TMonS3APIDataMain;
  WProgram: TMonS3APIDataProgram;
  WValues: TMonS3APIDataValueList;
  WNewRowID: integer;
begin
  WMain := TMonS3APIDataMain.Create;
  try
    WMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
    WMain.SetDataPath(CMonS3APITest_DataFolder);

    WProgram := WMain.GetProgramInstance;
    try
      WProgram.ConnectData;
      WProgram.Login(WProgram.TranslatePassword(CMonS3APITest_Password));

      WValues := TMonS3APIDataValueList.Create;
      try
        WValues.AddValue('Uzivatel', TMonS3APIValue.CreateString('a'));
        WValues.AddValue('Popis', TMonS3APIValue.CreateString('vstup do agendy'));
        WValues.AddValue('Datum', TMonS3APIValue.CreateDate(Now));

        WNewRowID := WProgram.AddRow('GlAkce', WValues);
      finally
        WValues.Free;
      end;
    finally
      WProgram.Free;
    end;
  finally
    WMain.Free;
  end;
end;

procedure TMonS3APITest9.Run;
var
  WMain: TMonS3APIDataMain;
  WProgram: TMonS3APIDataProgram;
  WValues: TMonS3APIDataValueList;
  WWhere: TMonS3APIDataWhereList;
  WAfftectedRows: integer;
begin
  WMain := TMonS3APIDataMain.Create;
  try
    WMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
    WMain.SetDataPath(CMonS3APITest_DataFolder);

    WProgram := WMain.GetProgramInstance;
    try
      WProgram.ConnectData;
      WProgram.Login(WProgram.TranslatePassword(CMonS3APITest_Password));

      WValues := TMonS3APIDataValueList.Create;
      WWhere := TMonS3APIDataWhereList.Create;
      try
        WValues.AddValue('Popis', TMonS3APIValue.CreateString('upravený popis'));
        WWhere.AddWhere('Datum', MonS3APIDataWhereOperator_Equals, TMonS3APIValue.CreateDate(Now));

        WAfftectedRows := WProgram.UpdateRows('GlAkce', WValues, WWhere);
      finally
        WValues.Free;
        WWhere.Free;
      end;
    finally
      WProgram.Free;
    end;
  finally
    WMain.Free;
  end;
end;

procedure TMonS3APITest10.Run;
var
  WMain: TMonS3APIDataMain;
  WProgram: TMonS3APIDataProgram;
  WWhere: TMonS3APIDataWhereList;
  WAfftectedRows: integer;
begin
  WMain := TMonS3APIDataMain.Create;
  try
    WMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
    WMain.SetDataPath(CMonS3APITest_DataFolder);

    WProgram := WMain.GetProgramInstance;
    try
      WProgram.ConnectData;
      WProgram.Login(WProgram.TranslatePassword(CMonS3APITest_Password));

      WWhere := TMonS3APIDataWhereList.Create;
      try
        WWhere.AddWhere('Datum', MonS3APIDataWhereOperator_Equals, TMonS3APIValue.CreateDate(Now));

        WAfftectedRows := WProgram.DeleteRows('GlAkce', WWhere);

        //Podmínka musí být vyplnìna. Pokud chci smazat vše, tak:
        //WProgram.DeleteAll("GlAkce");

      finally
        WWhere.Free;
      end;
    finally
      WProgram.Free;
    end;
  finally
    WMain.Free;
  end;
end;

procedure TMonS3APITest11.Run;
var
  WMain: TMonS3APIDataMain;
  WProgram: TMonS3APIDataProgram;
begin
  WMain := TMonS3APIDataMain.Create;
  try
    WMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
    WMain.SetDataPath(CMonS3APITest_DataFolder);

    WProgram := WMain.GetProgramInstance;
    try
      WProgram.ConnectData;
      WProgram.Login(WProgram.TranslatePassword(CMonS3APITest_Password));

      WProgram.TransactionStart;
      try
        AddRow(WProgram, 'vstup do agendy');
        AddRow(WProgram, 'pøihlášení uživatele');
        AddRow(WProgram, 'ukonèení programu');

        WProgram.TransactionCommit;
      except
        WProgram.TransactionRollback;
        raise;
      end;
    finally
      WProgram.Free;
    end;
  finally
    WMain.Free;
  end;
end;

procedure TMonS3APITest11.AddRow(AProgram: TMonS3APIDataProgram; const APopis: string);
var
  WValues: TMonS3APIDataValueList;
  WNewRowID: integer;
begin
  WValues := TMonS3APIDataValueList.Create;
  try
    WValues.AddValue('Uzivatel', TMonS3APIValue.CreateString('a'));
    WValues.AddValue('Popis', TMonS3APIValue.CreateString(APopis));
    WValues.AddValue('Datum', TMonS3APIValue.CreateDate(Now));

    WNewRowID := AProgram.AddRow('GlAkce', WValues);
  finally
    WValues.Free;
  end;
end;

procedure TMonS3APITest12.Run;
var
  WMain: TMonS3APIDataMain;
  WProgram: TMonS3APIDataProgram;
  WAgendaList: TMonS3APIAgendaList;
  WAgendaItem: TMonS3APIAgenda;
  WAgenda: TMonS3APIDataAgenda;
begin
  WMain := TMonS3APIDataMain.Create;
  try
    WMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
    WMain.SetDataPath(CMonS3APITest_DataFolder);

    WProgram := WMain.GetProgramInstance;
    try
      WProgram.ConnectData;
      WProgram.Login(WProgram.TranslatePassword(CMonS3APITest_Password));

      WAgendaList := TMonS3APIAgendaList.Create;
      try
        //Získá seznam agend.
        //Jsou již naètená nìkterá základní data, takže není tøeba èíst tabulku Agenda.
        WProgram.GetListAgend(WAgendaList);

        //V tuto chvíli již nepotøebujeme data programu.
        //WProgram.DisconnectData;
        //Nebo rovnou mùžu program uvolnit.
        FreeAndNil(WProgram);

        //Vybrat nìjakou agendu.
        WAgendaItem := WAgendaList.GetByExt('001');

        //Popøípadì vzít první ne-demo agendu.
        WAgendaItem := WAgendaList.GetFirstNoDemo;

        //Mùžu také chtít info o konkrétní agendì, bez toho, aby se naèítal celý seznam agend.
        //WAgendaItem := WProgram.GetAgendaInfo('001');
        //WAgendaItem.Free;

        //Vytvoøit instanci pro agendu.
        WAgenda := WMain.GetAgendaInstance;
        try
          WAgenda.SetAgenda(WAgendaItem.Ext);
          WAgenda.ConnectData;

          //Nyní lze opìt získat info o agendì:
          //WAgendaItem := WAgenda.GetAgendaInfo;
          //WAgendaItem.Free;


          //***
          //Nyní je možné pracovat s agendou.
          //***

          //Opìt se lze od dat odpojit.
          WAgenda.DisconnectData;
        finally
          //Není tøeba volat DisconnectData pøed uvolnìním samotného objektu.
          WAgenda.Free;
        end;
      finally
        WAgendaList.Free;
      end;
    finally
      //Uvolnìním instance programu se neuvolní agenda. Ta má svoji nezávislou instanci.
      WProgram.Free;
    end;
  finally
    //Pøi uvolnìní hlavní instance se zruší (ale neuvolní) všechny instance programu a agend, takže to dìlat až úplnì na konec.
    WMain.Free;
  end;
end;

procedure TMonS3APITest13.Run;
var
  WMain: TMonS3APIDataMain;
  WProgram: TMonS3APIDataProgram;
  WAgenda: TMonS3APIDataAgenda;
  WYearList: TMonS3APIYearList;
begin
  WMain := TMonS3APIDataMain.Create;
  try
    WMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
    WMain.SetDataPath(CMonS3APITest_DataFolder);

    WProgram := WMain.GetProgramInstance;
    try
      WProgram.ConnectData;
      WProgram.Login(WProgram.TranslatePassword(CMonS3APITest_Password));
      WProgram.DisconnectData;

      WAgenda := WMain.GetAgendaInstance;
      try
        WAgenda.SetAgenda('001');
        WAgenda.ConnectData;

        WYearList := TMonS3APIYearList.Create;
        try
          //Naètení seznamu rokù.
          //Obsahuje práva a základní informace, takže není nutné èíst tabulku Rok.
          WAgenda.GetYearList(WYearList);

          //Možné volat:
          //WYearList.GetByDate(Now);
          //WYearList.GetActual;
        finally
          WYearList.Free;
        end;
      finally
        WAgenda.Free;
      end;
    finally
      WProgram.Free;
    end;
  finally
    WMain.Free;
  end;
end;

procedure TMonS3APITest14.Run;
var
  WMain: TMonS3APIDataMain;
  WProgram: TMonS3APIDataProgram;
  WAgenda: TMonS3APIDataAgenda;
  WWhat: TMonS3APIDataWhatList;
  WWhere: TMonS3APIDataWhereList;
  WSelectResult: TMonS3APIDataSelectResult;
  WPopis: string;
  WUzivatel: string;
begin
  WMain := TMonS3APIDataMain.Create;
  try
    WMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
    WMain.SetDataPath(CMonS3APITest_DataFolder);

    WProgram := WMain.GetProgramInstance;
    try
      WProgram.ConnectData;
      WProgram.Login(WProgram.TranslatePassword(CMonS3APITest_Password));
      WProgram.DisconnectData;

      WAgenda := WMain.GetAgendaInstance;
      try
        WAgenda.SetAgenda('001');
        WAgenda.ConnectData;

        //Stejnì jako pro program.

        WWhat := TMonS3APIDataWhatList.Create;
        WWhere := TMonS3APIDataWhereList.Create;
        WSelectResult := TMonS3APIDataSelectResult.Create;
        try
          WWhat.AddAll;
          WWhere.AddWhere('Datum', MonS3APIDataWhereOperator_Equals, TMonS3APIValue.CreateDate(Now));
          WAgenda.GetRows('AdAkce', WWhat, WSelectResult, WWhere);

          WAgenda.DisconnectData;

          while WSelectResult.Next do
          begin
            //Pøeète hodnoty.
            WPopis := WSelectResult.GetColByName('Popis').AsString;
            WUzivatel := WSelectResult.GetColByName('Uzivatel').AsString;

            //nìco s daty udìlat
          end;
        finally
          WWhat.Free;
          WWhere.Free;
          WSelectResult.Free;
        end;
      finally
        WAgenda.Free;
      end;
    finally
      WProgram.Free;
    end;
  finally
    WMain.Free;
  end;
end;

procedure TMonS3APITest15.Run;
var
  WMain: TMonS3APIDataMain;
  WProgram: TMonS3APIDataProgram;
  WAgenda: TMonS3APIDataAgenda;
begin
  WMain := TMonS3APIDataMain.Create;
  try
    WMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
    WMain.SetDataPath(CMonS3APITest_DataFolder);

    WProgram := WMain.GetProgramInstance;
    try
      WProgram.ConnectData;
      WProgram.Login(WProgram.TranslatePassword(CMonS3APITest_Password));
      WProgram.DisconnectData;

      WAgenda := WMain.GetAgendaInstance;
      try
        WAgenda.SetAgenda('001');
        WAgenda.ConnectData;

        //Stejnì jako pro program.

        WAgenda.TransactionStart;
        try
          AddRow(WAgenda, 'vstup do agendy');
          AddRow(WAgenda, 'pøihlášení uživatele');
          AddRow(WAgenda, 'ukonèení programu');

          WAgenda.TransactionCommit;
        except
          WAgenda.TransactionRollback;
          raise;
        end;
      finally
        WAgenda.Free;
      end;
    finally
      WProgram.Free;
    end;
  finally
    WMain.Free;
  end;
end;

procedure TMonS3APITest15.AddRow(AAgenda: TMonS3APIDataAgenda; const APopis: string);
var
  WValues: TMonS3APIDataValueList;
  WNewRowID: integer;
begin
  WValues := TMonS3APIDataValueList.Create;
  try
    WValues.AddValue('Uzivatel', TMonS3APIValue.CreateString('a'));
    WValues.AddValue('Popis', TMonS3APIValue.CreateString(APopis));
    WValues.AddValue('Datum', TMonS3APIValue.CreateDate(Now));

    WNewRowID := AAgenda.AddRow('AdAkce', WValues);
  finally
    WValues.Free;
  end;
end;

procedure TMonS3APITest16.Run;
var
  WProgram: TMonS3APIDataProgram;
  WAgenda: TMonS3APIDataAgenda;
begin
  FMain := TMonS3APIDataMain.Create;
  FMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
  FMain.SetDataPath(CMonS3APITest_DataFolder);

  WProgram := FMain.GetProgramInstance;
  try
    WProgram.ConnectData;
    WProgram.Login(WProgram.TranslatePassword(CMonS3APITest_Password));
    FreeAndNil(WProgram);

    WAgenda := FMain.GetAgendaInstance;
    try
      WAgenda.SetAgenda('001');
      WAgenda.ConnectData;

      //Vytvoøím 2 vlákna a sám to taky takhle zpracuju.
      CreateThread;
      //CreateThread;
      //DoWork(WAgenda);
    finally
      WAgenda.Free;
    end;
  finally
    WProgram.Free;
  end;

  //Správnì by se mìl WMain po ukonèení všech vláken uvolnit.
  //Tento kód však slouží jen pro úèely ukázky použití a tak to nebudu øešit.
  //FMain.Free;
end;

function MonS3APITest16ThreadWork(Parameter: Pointer): Integer;
begin
  TMonS3APITest16(Parameter).ThreadWork;
  Result := 0;
end;

procedure TMonS3APITest16.CreateThread;
var
  id: LongWord;
begin
  BeginThread(nil, 0, MonS3APITest4ThreadWork, Self, 0, id);
end;

procedure TMonS3APITest16.ThreadWork;
var
  WAgenda: TMonS3APIDataAgenda;
begin
  WAgenda := FMain.GetAgendaInstance;
  try
    WAgenda.SetAgenda('001');
    WAgenda.ConnectData;
    DoWork(WAgenda);
    WAgenda.DisconnectData;
  finally
    WAgenda.Free;
  end;
end;

procedure TMonS3APITest16.DoWork(AAgenda: TMonS3APIDataAgenda);
var
  WWhat: TMonS3APIDataWhatList;
  WWhere: TMonS3APIDataWhereList;
  WSelectResult: TMonS3APIDataSelectResult;
  WPopis: string;
  WUzivatel: string;
begin
  WWhat := TMonS3APIDataWhatList.Create;
  WWhere := TMonS3APIDataWhereList.Create;
  WSelectResult := TMonS3APIDataSelectResult.Create;
  try
    WWhat.AddAll;
    WWhere.AddWhere('Datum', MonS3APIDataWhereOperator_Equals, TMonS3APIValue.CreateDate(Now));
    AAgenda.GetRows('AdAkce', WWhat, WSelectResult, WWhere);

    while WSelectResult.Next do
    begin
      //Pøeète hodnoty.
      WPopis := WSelectResult.GetColByName('Popis').AsString;
      WUzivatel := WSelectResult.GetColByName('Uzivatel').AsString;

      //nìco s daty udìlat
    end;
  finally
    WWhat.Free;
    WWhere.Free;
    WSelectResult.Free;
  end;
end;

procedure TMonS3APITest17.Run;
var
  WMain: TMonS3APIDataMain;
  WProgram: TMonS3APIDataProgram;
  WAgenda1: TMonS3APIDataAgenda;
  WAgenda2: TMonS3APIDataAgenda;
  WWhat: TMonS3APIDataWhatList;
  WWhere: TMonS3APIDataWhereList;
  WSelectResult1: TMonS3APIDataSelectResult;
  WSelectResult2: TMonS3APIDataSelectResult;
begin
  WMain := TMonS3APIDataMain.Create;
  try
    WMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
    WMain.SetDataPath(CMonS3APITest_DataFolder);

    WProgram := WMain.GetProgramInstance;
    try
      WProgram.ConnectData;
      WProgram.Login(WProgram.TranslatePassword(CMonS3APITest_Password));
      FreeAndNil(WProgram);

      WAgenda1 := WMain.GetAgendaInstance;
      WAgenda2 := WMain.GetAgendaInstance;
      try
        WAgenda1.SetAgenda('spa');
        WAgenda2.SetAgenda('spb');

        WAgenda1.ConnectData;
        WAgenda2.ConnectData;

        WWhat := TMonS3APIDataWhatList.Create;
        WWhere := TMonS3APIDataWhereList.Create;
        WSelectResult1 := TMonS3APIDataSelectResult.Create;
        WSelectResult2 := TMonS3APIDataSelectResult.Create;
        try
          WWhat.AddAll;
          WWhere.AddWhere('Datum', MonS3APIDataWhereOperator_Equals, TMonS3APIValue.CreateDate(Now));

          WAgenda1.GetRows('AdAkce', WWhat, WSelectResult1, WWhere);
          WAgenda2.GetRows('AdAkce', WWhat, WSelectResult2, WWhere);

          WAgenda1.DisconnectData;
          WAgenda2.DisconnectData;

          //Zpracovat data...
        finally
          WWhat.Free;
          WWhere.Free;
          WSelectResult1.Free;
          WSelectResult2.Free;
        end;
      finally
        WAgenda1.Free;
        WAgenda2.Free;
      end;
    finally
      WProgram.Free;
    end;
  finally
    WMain.Free;
  end;
end;

procedure TMonS3APITest18.Run;
var
  wMain: TMonS3APIDataMain;
  wProgram: TMonS3APIDataProgram;
  wDokumenty: TMonS3APIDataDokumenty;
  wWhat: TMonS3APIDataWhatList;
  wWhere: TMonS3APIDataWhereList;
  wSelectResult: TMonS3APIDataSelectResult;
  wGuid: String;
  wNazev: String;
  wBuffer: TBytes;
  wFile: TFileStream;
begin
  wMain := TMonS3APIDataMain.Create;
  try
    wMain.LoadDLL(GetDLLFile, CMonS3APITest_AppName);
    wMain.SetDataPath(CMonS3APITest_DataFolder);

    wProgram := nil;
    wDokumenty := nil;
    try
      wProgram := wMain.GetProgramInstance;
      wProgram.ConnectData;
      wProgram.Login(wProgram.TranslatePassword(CMonS3APITest_Password));
      wProgram.DisconnectData;

      wDokumenty := wMain.GetDokumentyInstance;
      // Je potøeba nastavit, ze které agendy chceme s dokumenty.s3db pracovat
      wDokumenty.SetAgenda('spa');
      wDokumenty.ConnectData;

      wWhat := nil;
      wWhere := nil;
      wSelectResult := nil;
      try
        wWhat := TMonS3APIDataWhatList.Create;
        wWhere := TMonS3APIDataWhereList.Create;
        wSelectResult := TMonS3APIDataSelectResult.Create;
        wWhat.AddAll;
        wWhere.AddWhere('ID_Uloziste', MonS3APIDataWhereOperator_Equals, TMonS3APIValue.CreateInt64(1));
        wDokumenty.GetRows('Uloziste', wWhat, wSelectResult, wWhere);

        while wSelectResult.Next do
        begin
          wNazev := wSelectResult.GetColByName('Nazev').AsString;
          wGuid := wSelectResult.GetColByName('GUID').AsString;
          wBuffer := wSelectResult.GetColByName('Obsah').AsData;

          //wNazev := 'Path/' + wNazev  - pøidání cesty k názvu souboru
          wFile := TFileStream.Create(wNazev, fmCreate or fmShareDenyWrite);
          try
            wFile.WriteBuffer(wBuffer[0], Length(wBuffer));
          finally
            wFile.Free;
          end;
        end;
      finally
        wSelectResult.Free;
        wWhere.Free;
        wWhat.Free;
      end;

    finally
      wDokumenty.Free;
      wProgram.Free;
    end;
  finally
    wMain.Free
  end;
end;

end.

