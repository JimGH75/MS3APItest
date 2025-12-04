using System;
using System.Threading;
using MonS3API;
using System.IO;

namespace MonS3APITest
{
    public static class MonS3APITestConst
    {
        public const string DLLFile32 = @"D:\MonS3API\API\MonS3API_32.dll";
        public const string DLLFile64 = @"D:\MonS3API\API\MonS3API_64.dll";
        public const string DataFolder = @"D:\MonS3API\Data\";
        public const string Password = "a"; //NIKDY NEUKLÁDEJTE takto HESLA, viz dokumentace
        public const string AppName = "Testovaci Aplikace";

        public static string GetDLLFile()
        {
            if (Environment.Is64BitProcess)
            {
                return DLLFile64;
            }
            else
            {
                return DLLFile32;
            }
        }
    }

    class MonS3APITest
    {
        public void Run()
        {
            new MonS3APITest1().Run();
            new MonS3APITest2().Run();
            new MonS3APITest3().Run();
            new MonS3APITest4().Run();
            new MonS3APITest5().Run();
            new MonS3APITest6().Run();
            new MonS3APITest7().Run();
            new MonS3APITest8().Run();
            new MonS3APITest9().Run();
            new MonS3APITest10().Run();
            new MonS3APITest11().Run();
            new MonS3APITest12().Run();
            new MonS3APITest13().Run();
            new MonS3APITest14().Run();
            new MonS3APITest15().Run();
            new MonS3APITest16().Run();
            new MonS3APITest17().Run();
            new MonS3APITest18().Run();
        }
    }

    /// <summary>
    /// Základní připojení k DLL knihovně, datům, přihlášení se.
    /// </summary>
    class MonS3APITest1
    {
        public void Run()
        {
            //Instance pro práci s daty.
            //Ideálně pro jedny data používat jednu instanci.
            //Pro více vláken se využije tato jedna instance, ze které se vytvářejí instance pro program a agendy pro každé vlákno zvlášť.
            MonS3APIDataMain main = new MonS3APIDataMain();

            //Hledání MonS3API.dll
            string dllFile = main.FindDLL();

            //Hledání MonS3Reader.dll
            string dllFile = main.FindDLL(MonS3APIDataMain.MonS3APIDLLType.MonS3Reader);

            //Popřípadě můžeme použít vlastní cestu.
            dllFile = MonS3APITestConst.GetDLLFile();

            //Připojení se k DLL knihovně.
            //Předáme jí název naší aplikace.
            main.LoadDLL(dllFile, MonS3APITestConst.AppName);

            //Hledání dat MoneyS3.
            string dataFolder = main.FindData();

            //Popřípadě můžeme použít vlastní cestu.
            dataFolder = MonS3APITestConst.DataFolder;

            //Nastavení cesty k datům.
            main.SetDataPath(dataFolder);


            //Instance pro program.
            //Jedna instance pro jedno vlákno.
            //V rámci jednoho vlákna je zbytečné vytvářet více instancí.
            MonS3APIDataProgram program = main.GetProgramInstance();

            //Před jakýkoliv dalším volání je potřeba se připojit k datům.
            program.ConnectData();


            //Blok přihlašování:
            {
                //Zjištění informace o možnostech přihlášení.
                MonS3APILoginInformation logInfo = program.GetLoginInformation();

                switch (logInfo)
                {
                    case MonS3APILoginInformation.Password:
                        //Potřebuji heslo!
                        break;
                    case MonS3APILoginInformation.WindowsNO:
                        //Není se možné přihlásit.
                        break;
                    case MonS3APILoginInformation.WindowsPassword:
                        //Buďto vložím prázdné heslo a to přihlásí pomocí uživatele a nebo zažádám o heslo - popřípadě dám uživateli na výběr.
                        break;
                    case MonS3APILoginInformation.WindowsYES:
                        //Vložím prázdné heslo do funkce Login a nic neřeším.
                        break;
                    case MonS3APILoginInformation.WithoutPassword:
                        //Vložím prázdné heslo do funkce Login a nic neřeším.
                        break;
                }

                //Můžu uživatele požádat o zadání hesla.
                //pass = program.LoginDialog(IntPtr.Zero);
                //Pokud jsem ve windows form aplikaci, tak doporučuji vyplnit vstup Handlem okna.
                //Popřípadě:
                //pass = program.LoginDialog(Application.OpenForms[0].Handle)

                //Pokud potřebuji heslo a znám ho z minula, tak jen přeložím.
                string pass = MonS3APITestConst.Password;
                pass = program.TranslatePassword(pass);
                //NIKDY NEUKLÁDEJTE HESLA, ale vždy tento hash.
                //Takže původní heslo je dobré smazat a nahradit tímto.

                //Samotné přihlášení - buďto pomocí hesla a nebo pokud je to windows přihlášení nebo bez hesla, tak prázdný string.
                //Funkce vrací, zda se to povedlo.
                program.Login(pass);

                //Nyní lze získat informace o uživateli.
                MonS3APIUserInfo userInfo = program.GetUserInfo();

                //A nebo seznam všech práv.
                MonS3APIUserRightList userRights = program.GetUserRights();
            }
            //Konec bloku přihlašování.

            //xxx
            //Odteď je možné volat další funkce.
            //xxx

            //Jakmile nepotřebujeme připojení, můžem se odpojit.
            program.DisconnectData();

            //Ideálně po ukončení veškeré práce uvolnit knihovnu.
            //Pokud zavoláme tohle, tak nemusíme předtím volat DisconnectData.
            main.UnLoadDLL();
        }
    }

    /// <summary>
    /// Získání struktury tabulek a přečtení dat.
    /// </summary>
    class MonS3APITest2
    {
        public void Run()
        {
            MonS3APIDataMain main = new MonS3APIDataMain();
            main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));

            //Seznam tabulek.
            //Pokud potřebuji znát ten seznam a nebo si chci ověřit na co mám právo.
            MonS3APITableList tableList = program.GetTableList();

            //Informace o tabulce, pokud je potřeba.
            MonS3APITable table = tableList.GetByName("GlAkce");

            //Seznam sloupečků tabulky.
            //Jenom pokud to k něčemu potřebuji.
            //Konkrétní sloupeček lze najít pomocí GetByName popřípadě IDčko rovnou GetIDColumn.
            MonS3APITableColumnList columnList = program.GetTableColumns("GlAkce");

            //Pro funkci GetRows musí být vždy zavoláno AddAll.
            //V budoucnu bude možnost vybírat sloupečky atd.
            MonS3APIDataWhatList what = new MonS3APIDataWhatList();
            what.AddAll();

            //Podmínka na datum.
            MonS3APIDataWhereList where = new MonS3APIDataWhereList();
            where.AddWhere("Datum", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateDate(DateTime.Now));

            //Uložíme výstup.
            MonS3APIDataSelectResult selectResult = program.GetRows("GlAkce", what, where);

            //Dále nevolám žádnou funkci, takže je možné se od dat odpojit.
            //Ale není to nutnost.
            program.DisconnectData();

            //Zpracovat data - možnost č 1.
            while (selectResult.Next())
            {
                //Přečte hodnoty.
                string popis = selectResult.GetColByName("Popis").AsString;
                string uzivatel = selectResult.GetColByName("Uzivatel").AsString;

                //něco s daty udělat
            }

            //Zpracovat data - možnost č 2.
            //Zakomentováno, nemůže se volat po tom prvním průchodu.
            /*
            for (int i = 0; i < selectResult.Count; i++)
            {
                selectResult.Next();

                //Přečte hodnoty.
                string popis = selectResult.GetColByName("Popis").AsString;
                string uzivatel = selectResult.GetColByName("Uzivatel").AsString;

                //něco s daty udělat
            }
            */

            //Úplně na konec.
            main.UnLoadDLL();
        }
    }

    /// <summary>
    /// Opakovaná práce s daty.
    /// </summary>
    class MonS3APITest3
    {
        public void Run()
        {
            MonS3APIDataMain main = new MonS3APIDataMain();
            main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));

            //Možné uzavření, ale není to nutnost.
            program.DisconnectData();

            MonS3APIDataWhatList what = new MonS3APIDataWhatList();
            what.AddAll();

            MonS3APIDataWhereList where = new MonS3APIDataWhereList();
            where.AddWhere("Datum", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateDate(DateTime.Now));

            for (int i = 0; i < 5; i++)
            {
                program.ConnectData();
                MonS3APIDataSelectResult selectResult = program.GetRows("GlAkce", what, where);
                program.DisconnectData();

                while (selectResult.Next())
                {
                    string popis = selectResult.GetColByName("Popis").AsString;
                    string uzivatel = selectResult.GetColByName("Uzivatel").AsString;

                    //něco s daty udělat
                }

                Thread.Sleep(2000);
            }

            //Úplně na konec.
            main.UnLoadDLL();
        }
    }

    /// <summary>
    /// Více vláken.
    /// </summary>
    class MonS3APITest4
    {
        private MonS3APIDataMain Main;

        public void Run()
        {
            Main = new MonS3APIDataMain();
            Main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            Main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = Main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));

            //Možné uzavření, ale není to nutnost.
            program.DisconnectData();

            //Vytvořím 2 vlákna a sám to taky takhle zpracuju.
            CreateThread();
            CreateThread();
            DoWork(program);

            //Úplně na konec. Pokud se bojím zavolat kvůli vláknům, tak to radši nezavolám.
            //Až na to GC příjde, tak to uvolní.
            //Main.UnLoadDLL();
        }

        private void CreateThread()
        {
            Thread thr = new Thread(ThreadWork);
            thr.Start();
        }

        private void ThreadWork()
        {
            MonS3APIDataProgram program = Main.GetProgramInstance();
            DoWork(program);
        }

        private void DoWork(MonS3APIDataProgram program)
        {
            MonS3APIDataWhatList what = new MonS3APIDataWhatList();
            what.AddAll();

            MonS3APIDataWhereList where = new MonS3APIDataWhereList();
            where.AddWhere("Datum", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateDate(DateTime.Now));

            for (int i = 0; i < 5; i++)
            {
                program.ConnectData();
                MonS3APIDataSelectResult selectResult = program.GetRows("GlAkce", what, where);
                program.DisconnectData();

                while (selectResult.Next())
                {
                    string popis = selectResult.GetColByName("Popis").AsString;
                    string uzivatel = selectResult.GetColByName("Uzivatel").AsString;

                    //něco s daty udělat
                }

                Thread.Sleep(2000);
            }
        }
    }

    /// <summary>
    /// Složité podmínky a řazení.
    /// </summary>
    class MonS3APITest5
    {
        public void Run()
        {
            MonS3APIDataMain main = new MonS3APIDataMain();
            main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));

            MonS3APIDataWhatList what = new MonS3APIDataWhatList();
            what.AddAll();

            //Podmínka:
            //(Uzivatel = "a" || Uzivatel = "b") && Datum >= Now
            MonS3APIDataWhereList where = new MonS3APIDataWhereList();
            where.AddBracketStart();
                where.AddWhere("Uzivatel", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateString("a"));
                where.AddOperatorOR();
                where.AddWhere("Uzivatel", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateString("b"));
            where.AddBracketEnd();
            where.AddOperatorAND();
            where.AddWhere("Datum", MonS3APIDataWhereOperator.GreaterEq, MonS3APIValue.CreateDate(DateTime.Now));

            //V tomto případě lze ale i použít:
            //where.Clear();
            //where.AddWhere("Uzivatel", MonS3APIDataWhereOperator.InValues, new MonS3APIValueList() { MonS3APIValue.CreateString("a"), MonS3APIValue.CreateString("b") } );
            //where.AddOperatorAND();
            //where.AddWhere("Datum", MonS3APIDataWhereOperator.GreaterEq, MonS3APIValue.CreateDate(DateTime.Now));


            //Řadit podle Datum a pak Popis
            MonS3APIDataSortList sort = new MonS3APIDataSortList();
            sort.AddSort("Datum", MonS3APIDataSortType.Ascending);
            sort.AddSort("Popis", MonS3APIDataSortType.Ascending);

            MonS3APIDataSelectResult selectResult = program.GetRows("GlAkce", what, where, sort);

            while (selectResult.Next())
            {
                string popis = selectResult.GetColByName("Popis").AsString;
                string uzivatel = selectResult.GetColByName("Uzivatel").AsString;

                //něco s daty udělat
            }

            main.UnLoadDLL();
        }
    }

    /// <summary>
    /// Zda řádek existuje.
    /// </summary>
    class MonS3APITest6
    {
        public void Run()
        {
            MonS3APIDataMain main = new MonS3APIDataMain();
            main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));

            MonS3APIDataWhereList where = new MonS3APIDataWhereList();
            where.AddWhere("Datum", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateDate(DateTime.Now));

            bool exists = program.GetRowExists("GlAkce", where);

            main.UnLoadDLL();
        }
    }

    /// <summary>
    /// Počet řádků.
    /// </summary>
    class MonS3APITest7
    {
        public void Run()
        {
            MonS3APIDataMain main = new MonS3APIDataMain();
            main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));

            MonS3APIDataWhereList where = new MonS3APIDataWhereList();
            where.AddWhere("Datum", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateDate(DateTime.Now));

            int count = program.GetRowCount("GlAkce", where);

            main.UnLoadDLL();
        }
    }

    /// <summary>
    /// Přidání řádku.
    /// Nelze použít s knihovnou MonS3Reader.
    /// </summary>
    class MonS3APITest8
    {
        public void Run()
        {
            MonS3APIDataMain main = new MonS3APIDataMain();
            main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));

            MonS3APIDataValueList values = new MonS3APIDataValueList();
            values.AddValue("Uzivatel", MonS3APIValue.CreateString("a"));
            values.AddValue("Popis", MonS3APIValue.CreateString("vstup do agendy"));
            values.AddValue("Datum", MonS3APIValue.CreateDate(DateTime.Now));

            int NewRowID = program.AddRow("GlAkce", values);

            main.UnLoadDLL();
        }
    }

    /// <summary>
    /// Aktualizace řádku.
    /// Nelze použít s knihovnou MonS3Reader.
    /// </summary>
    class MonS3APITest9
    {
        public void Run()
        {
            MonS3APIDataMain main = new MonS3APIDataMain();
            main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));

            MonS3APIDataValueList values = new MonS3APIDataValueList();
            values.AddValue("Popis", MonS3APIValue.CreateString("upravený popis"));

            MonS3APIDataWhereList where = new MonS3APIDataWhereList();
            where.AddWhere("Datum", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateDate(DateTime.Now));

            int AfftectedRows = program.UpdateRows("GlAkce", values, where);

            main.UnLoadDLL();
        }
    }

    /// <summary>
    /// Smazání řádku.
    /// Nelze použít s knihovnou MonS3Reader.
    /// </summary>
    class MonS3APITest10
    {
        public void Run()
        {
            MonS3APIDataMain main = new MonS3APIDataMain();
            main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));

            MonS3APIDataWhereList where = new MonS3APIDataWhereList();
            where.AddWhere("Datum", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateDate(DateTime.Now));

            int AfftectedRows = program.DeleteRows("GlAkce", where);

            //Podmínka musí být vyplněna. Pokud chci smazat vše, tak:
            //program.DeleteAll("GlAkce");

            main.UnLoadDLL();
        }
    }

    /// <summary>
    /// Transakce.
    /// Nelze použít s knihovnou MonS3Reader.
    /// </summary>
    class MonS3APITest11
    {
        public void Run()
        {
            MonS3APIDataMain main = new MonS3APIDataMain();
            main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));

            program.TransactionStart();
            try
            {
                AddRow(program, "vstup do agendy");
                AddRow(program, "přihlášení uživatele");
                AddRow(program, "ukončení programu");

                program.TransactionCommit();
            }
            catch
            {
                program.TransactionRollback();
                throw;
            }

            main.UnLoadDLL();
        }

        private void AddRow(MonS3APIDataProgram program, string popis)
        {
            MonS3APIDataValueList values = new MonS3APIDataValueList();
            values.AddValue("Uzivatel", MonS3APIValue.CreateString("a"));
            values.AddValue("Popis", MonS3APIValue.CreateString(popis));
            values.AddValue("Datum", MonS3APIValue.CreateDate(DateTime.Now));

            int NewRowID = program.AddRow("GlAkce", values);
        }
    }

    /// <summary>
    /// Připojení se k agendě.
    /// </summary>
    class MonS3APITest12
    {
        public void Run()
        {
            MonS3APIDataMain main = new MonS3APIDataMain();
            main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));

            //Získá seznam agend.
            //Jsou již načtená některá základní data, takže není třeba číst tabulku Agenda.
            MonS3APIAgendaList agendaList = program.GetListAgend();

            //V tuto chvíli již nepotřebujeme data programu.
            program.DisconnectData();

            //Vybrat nějakou agendu.
            MonS3APIAgenda agendaItem = agendaList.GetByExt("001");

            //Popřípadě vzít první ne-demo agendu.
            agendaItem = agendaList.GetFirstNoDemo();

            //Můžu také chtít info o konkrétní agendě, bez toho, aby se načítal celý seznam agend.
            //program.GetAgendaInfo("001");

            //Vytvořit instanci pro agendu.
            MonS3APIDataAgenda agenda = main.GetAgendaInstance();
            agenda.SetAgenda(agendaItem.Ext);
            agenda.ConnectData();

            //Nyní lze opět získat info o agendě:
            //agenda.GetAgendaInfo();

            //***
            //Nyní je možné pracovat s agendou.
            //***

            //Opět se lze od dat odpojit.
            agenda.DisconnectData();

            main.UnLoadDLL();
        }
    }

    /// <summary>
    /// Získat seznam roků agendy.
    /// </summary>
    class MonS3APITest13
    {
        public void Run()
        {
            MonS3APIDataMain main = new MonS3APIDataMain();
            main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));
            program.DisconnectData();

            MonS3APIDataAgenda agenda = main.GetAgendaInstance();
            agenda.SetAgenda("001");
            agenda.ConnectData();

            //Načtení seznamu roků.
            //Obsahuje práva a základní informace, takže není nutné číst tabulku Rok.
            MonS3APIYearList yearList = agenda.GetYearList();

            //Možné volat:
            //yearList.GetByDate(DateTime.Now);
            //yearList.GetActual();

            main.UnLoadDLL();
        }
    }

    /// <summary>
    /// Práce s daty agendy.
    /// </summary>
    class MonS3APITest14
    {
        public void Run()
        {
            MonS3APIDataMain main = new MonS3APIDataMain();
            main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));
            program.DisconnectData();

            MonS3APIDataAgenda agenda = main.GetAgendaInstance();
            agenda.SetAgenda("001");
            agenda.ConnectData();

            //Stejně jako pro program.

            MonS3APIDataWhatList what = new MonS3APIDataWhatList();
            what.AddAll();

            MonS3APIDataWhereList where = new MonS3APIDataWhereList();
            where.AddWhere("Datum", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateDate(DateTime.Now));

            MonS3APIDataSelectResult selectResult = agenda.GetRows("AdAkce", what, where);

            agenda.DisconnectData();

            while (selectResult.Next())
            {
                //Přečte hodnoty.
                string popis = selectResult.GetColByName("Popis").AsString;
                string uzivatel = selectResult.GetColByName("Uzivatel").AsString;

                //něco s daty udělat
            }

            main.UnLoadDLL();
        }
    }

    /// <summary>
    /// Transakce v agendě.
    /// Nelze použít s knihovnou MonS3Reader.
    /// </summary>
    class MonS3APITest15
    {
        public void Run()
        {
            MonS3APIDataMain main = new MonS3APIDataMain();
            main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));
            program.DisconnectData();

            MonS3APIDataAgenda agenda = main.GetAgendaInstance();
            agenda.SetAgenda("001");
            agenda.ConnectData();

            //Stejně jako pro program.

            agenda.TransactionStart();
            try
            {
                AddRow(agenda, "vstup do agendy");
                AddRow(agenda, "přihlášení uživatele");
                AddRow(agenda, "ukončení programu");

                agenda.TransactionCommit();
            }
            catch
            {
                agenda.TransactionRollback();
                throw;
            }

            main.UnLoadDLL();
        }

        private void AddRow(MonS3APIDataAgenda agenda, string popis)
        {
            MonS3APIDataValueList values = new MonS3APIDataValueList();
            values.AddValue("Uzivatel", MonS3APIValue.CreateString("a"));
            values.AddValue("Popis", MonS3APIValue.CreateString(popis));
            values.AddValue("Datum", MonS3APIValue.CreateDate(DateTime.Now));

            int NewRowID = agenda.AddRow("AdAkce", values);
        }
    }

    /// <summary>
    /// Práce s jednou agendou ve vláknech.
    /// </summary>
    class MonS3APITest16
    {
        private MonS3APIDataMain Main;

        public void Run()
        {
            Main = new MonS3APIDataMain();
            Main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            Main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = Main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));
            program.DisconnectData();

            MonS3APIDataAgenda agenda = Main.GetAgendaInstance();
            agenda.SetAgenda("001");           
            agenda.ConnectData();

            //Vytvořím 2 vlákna a sám to taky takhle zpracuju.
            CreateThread();
            CreateThread();
            DoWork(agenda);

            //Úplně na konec. Pokud se bojím zavolat kvůli vláknům, tak to radši nezavolám.
            //Až na to GC příjde, tak to uvolní.
            //main.UnLoadDLL();
        }

        private void CreateThread()
        {
            Thread thr = new Thread(ThreadWork);
            thr.Start();
        }

        private void ThreadWork()
        {
            MonS3APIDataAgenda agenda = Main.GetAgendaInstance();
            agenda.SetAgenda("001");
            agenda.ConnectData();
            DoWork(agenda);
            agenda.DisconnectData();
        }

        private void DoWork(MonS3APIDataAgenda agenda)
        {
            MonS3APIDataWhatList what = new MonS3APIDataWhatList();
            what.AddAll();

            MonS3APIDataWhereList where = new MonS3APIDataWhereList();
            where.AddWhere("Datum", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateDate(DateTime.Now));

            MonS3APIDataSelectResult selectResult = agenda.GetRows("AdAkce", what, where);

            while (selectResult.Next())
            {
                string popis = selectResult.GetColByName("Popis").AsString;
                string uzivatel = selectResult.GetColByName("Uzivatel").AsString;

                //něco s daty udělat
            }
        }
    }

    /// <summary>
    /// Práce s více agendami najednou.
    /// </summary>
    class MonS3APITest17
    {
        public void Run()
        {
            MonS3APIDataMain main = new MonS3APIDataMain();
            main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));
            program.DisconnectData();

            MonS3APIDataAgenda agenda1 = main.GetAgendaInstance();
            agenda1.SetAgenda("spa");
            agenda1.ConnectData();

            MonS3APIDataAgenda agenda2 = main.GetAgendaInstance();
            agenda2.SetAgenda("spb");
            agenda2.ConnectData();



            MonS3APIDataWhatList what = new MonS3APIDataWhatList();
            what.AddAll();

            MonS3APIDataWhereList where = new MonS3APIDataWhereList();
            where.AddWhere("Datum", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateDate(DateTime.Now));

            MonS3APIDataSelectResult selectResult1 = agenda1.GetRows("AdAkce", what, where);
            MonS3APIDataSelectResult selectResult2 = agenda2.GetRows("AdAkce", what, where);

            main.UnLoadDLL();
        }
    }

    /// <summary>
    /// Select z Dokumenty.s3db.
    /// </summary>
    class MonS3APITest18
    {
        public void Run()
        {
            MonS3APIDataMain main = new MonS3APIDataMain();
            main.LoadDLL(MonS3APITestConst.GetDLLFile(), MonS3APITestConst.AppName);
            main.SetDataPath(MonS3APITestConst.DataFolder);

            MonS3APIDataProgram program = main.GetProgramInstance();
            program.ConnectData();
            program.Login(program.TranslatePassword(MonS3APITestConst.Password));
            program.DisconnectData();

            MonS3APIDataDokumenty dokumenty = main.GetDokumentyInstance();
            // Je potřeba nastavit, ze které agendy chceme s dokumenty.s3db pracovat
            dokumenty.SetAgenda("spb");
            dokumenty.ConnectData();

            MonS3APIDataWhatList what = new MonS3APIDataWhatList();
            what.AddAll();

            MonS3APIDataWhereList where = new MonS3APIDataWhereList();
            where.AddWhere("ID_Uloziste", MonS3APIDataWhereOperator.Equals, MonS3APIValue.CreateInt64(1));

            MonS3APIDataSelectResult selectResult = dokumenty.GetRows("Uloziste", what, where); 

            while (selectResult.Next())
            {
                //Přečte hodnoty.
                string nazev = selectResult.GetColByName("Nazev").AsString;
                string guid = selectResult.GetColByName("GUID").AsString;
                byte[] obsah = selectResult.GetColByName("Obsah").AsData;
                // Test vytvoření souboru, Nazev souboru + obsah
                File.WriteAllBytes(nazev, obsah);
                //něco s daty udělat
            }

            main.UnLoadDLL();
        }
    }


}
