using MonS3API;

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
    where.AddWhere("Druh", MonS3APIDataWhereOperator.Equals,
                   MonS3APIValue.CreateString("R"));

    var rows = _program.GetRows("ObjPrijHl", what, where);

    while (rows.Next())
    {
        var vyrizeno = rows.GetColByName("Vyrizeno").AsDate;

        if (vyrizeno != DateTime.MinValue && vyrizeno != default(DateTime))
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
