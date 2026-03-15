using Mistruna.Core.Enums;
using Mistruna.Core.Models;

namespace Mistruna.Core.Extensions;

public static class CurrencyExtensions
{
    extension(Currency currency)
    {
        public string GetCode() => currency.ToString().ToUpperInvariant();
        public short GetNumeric() => (short)currency;

        public string GetFullName()
            => currency switch
            {
                Currency.Aed => "UAE Dirham",
                Currency.Afn => "Afghani",
                Currency.All => "Lek",
                Currency.Amd => "Armenian Dram",
                Currency.Aoa => "Kwanza",
                Currency.Ars => "Argentine Peso",
                Currency.Aud => "Australian Dollar",
                Currency.Awg => "Aruban Florin",
                Currency.Azn => "Azerbaijan Manat",

                Currency.Bam => "Convertible Mark",
                Currency.Bbd => "Barbados Dollar",
                Currency.Bdt => "Taka",
                Currency.Bgn => "Bulgarian Lev",
                Currency.Bhd => "Bahraini Dinar",
                Currency.Bif => "Burundi Franc",
                Currency.Bmd => "Bermudian Dollar",
                Currency.Bnd => "Brunei Dollar",
                Currency.Bob => "Boliviano",
                Currency.Bov => "Mvdol",
                Currency.Brl => "Brazilian Real",
                Currency.Bsd => "Bahamian Dollar",
                Currency.Btn => "Ngultrum",
                Currency.Bwp => "Pula",
                Currency.Byn => "Belarusian Ruble",
                Currency.Bzd => "Belize Dollar",

                Currency.Cad => "Canadian Dollar",
                Currency.Cdf => "Congolese Franc",
                Currency.Che => "WIR Euro",
                Currency.Chf => "Swiss Franc",
                Currency.Chw => "WIR Franc",
                Currency.Clf => "Unidad de Fomento",
                Currency.Clp => "Chilean Peso",
                Currency.Cny => "Yuan Renminbi",
                Currency.Cop => "Colombian Peso",
                Currency.Cou => "Unidad de Valor Real",
                Currency.Crc => "Costa Rican Colon",
                Currency.Cup => "Cuban Peso",
                Currency.Cve => "Cape Verde Escudo",
                Currency.Czk => "Czech Koruna",

                Currency.Djf => "Djibouti Franc",
                Currency.Dkk => "Danish Krone",
                Currency.Dop => "Dominican Peso",
                Currency.Dzd => "Algerian Dinar",

                Currency.Egp => "Egyptian Pound",
                Currency.Ern => "Nakfa",
                Currency.Etb => "Ethiopian Birr",
                Currency.Eur => "Euro",

                Currency.Fjd => "Fiji Dollar",
                Currency.Fkp => "Falkland Islands Pound",

                Currency.Gbp => "Pound Sterling",
                Currency.Gel => "Lari",
                Currency.Ghs => "Ghana Cedi",
                Currency.Gip => "Gibraltar Pound",
                Currency.Gmd => "Dalasi",
                Currency.Gnf => "Guinea Franc",
                Currency.Gtq => "Quetzal",
                Currency.Gyd => "Guyana Dollar",

                Currency.Hkd => "Hong Kong Dollar",
                Currency.Hnl => "Lempira",
                Currency.Htg => "Gourde",
                Currency.Huf => "Forint",

                Currency.Idr => "Rupiah",
                Currency.Ils => "New Israeli Sheqel",
                Currency.Inr => "Indian Rupee",
                Currency.Iqd => "Iraqi Dinar",
                Currency.Irr => "Iranian Rial",
                Currency.Isk => "Iceland Krona",

                Currency.Jmd => "Jamaican Dollar",
                Currency.Jod => "Jordanian Dinar",
                Currency.Jpy => "Yen",

                Currency.Kes => "Kenyan Shilling",
                Currency.Kgs => "Som",
                Currency.Khr => "Riel",
                Currency.Kmf => "Comoro Franc",
                Currency.Kpw => "North Korean Won",
                Currency.Krw => "Won",
                Currency.Kwd => "Kuwaiti Dinar",
                Currency.Kyd => "Cayman Islands Dollar",
                Currency.Kzt => "Tenge",

                Currency.Lak => "Kip",
                Currency.Lbp => "Lebanese Pound",
                Currency.Lkr => "Sri Lanka Rupee",
                Currency.Lrd => "Liberian Dollar",
                Currency.Lsl => "Loti",
                Currency.Lyd => "Libyan Dinar",

                Currency.Mad => "Moroccan Dirham",
                Currency.Mdl => "Moldovan Leu",
                Currency.Mga => "Malagasy Ariary",
                Currency.Mkd => "Denar",
                Currency.Mmk => "Kyat",
                Currency.Mnt => "Tugrik",
                Currency.Mop => "Pataca",
                Currency.Mru => "Ouguiya",
                Currency.Mur => "Mauritius Rupee",
                Currency.Mvr => "Rufiyaa",
                Currency.Mwk => "Malawi Kwacha",
                Currency.Mxn => "Mexican Peso",
                Currency.Mxv => "Mexican Unidad de Inversion (UDI)",
                Currency.Myr => "Malaysian Ringgit",
                Currency.Mzn => "Mozambique Metical",

                Currency.Nad => "Namibia Dollar",
                Currency.Ngn => "Naira",
                Currency.Nio => "Cordoba Oro",
                Currency.Nok => "Norwegian Krone",
                Currency.Npr => "Nepalese Rupee",
                Currency.Nzd => "New Zealand Dollar",

                Currency.Omr => "Rial Omani",

                Currency.Pab => "Balboa",
                Currency.Pen => "Sol",
                Currency.Pgk => "Kina",
                Currency.Php => "Philippine Peso",
                Currency.Pkr => "Pakistan Rupee",
                Currency.Pln => "Zloty",
                Currency.Pyg => "Guarani",

                Currency.Qar => "Qatari Rial",

                Currency.Ron => "Romanian Leu",
                Currency.Rsd => "Serbian Dinar",
                Currency.Rub => "Russian Ruble",
                Currency.Rwf => "Rwanda Franc",

                Currency.Sar => "Saudi Riyal",
                Currency.Sbd => "Solomon Islands Dollar",
                Currency.Scr => "Seychelles Rupee",
                Currency.Sdg => "Sudanese Pound",
                Currency.Sek => "Swedish Krona",
                Currency.Sgd => "Singapore Dollar",
                Currency.Shp => "Saint Helena Pound",
                Currency.Sle => "Leone",
                Currency.Sos => "Somali Shilling",
                Currency.Srd => "Surinam Dollar",
                Currency.Ssp => "South Sudanese Pound",
                Currency.Stn => "Dobra",
                Currency.Svc => "El Salvador Colon",
                Currency.Syp => "Syrian Pound",
                Currency.Szl => "Lilangeni",

                Currency.Thb => "Baht",
                Currency.Tjs => "Somoni",
                Currency.Tmt => "Turkmenistan New Manat",
                Currency.Tnd => "Tunisian Dinar",
                Currency.Top => "Pa’anga",
                Currency.Try => "Turkish Lira",
                Currency.Ttd => "Trinidad and Tobago Dollar",
                Currency.Twd => "New Taiwan Dollar",
                Currency.Tzs => "Tanzanian Shilling",

                Currency.Uah => "Hryvnia",
                Currency.Ugx => "Uganda Shilling",
                Currency.Usd => "US Dollar",
                Currency.Usn => "US Dollar (Next day)",
                Currency.Uyi => "Uruguay Peso en Unidades Indexadas",
                Currency.Uyu => "Peso Uruguayo",
                Currency.Uyw => "Unidad Previsional",
                Currency.Uzs => "Uzbekistan Sum",

                Currency.Ved => "Venezuelan Digital Bolívar",
                Currency.Ves => "Venezuelan Sovereign Bolívar",
                Currency.Vnd => "Dong",
                Currency.Vuv => "Vatu",

                Currency.Wst => "Tala",

                Currency.Xad => "Arab Accounting Dinar",
                Currency.Xaf => "CFA Franc BEAC",
                Currency.Xag => "Silver",
                Currency.Xau => "Gold",
                Currency.Xba => "Bond Markets Unit European Composite Unit (EURCO)",
                Currency.Xbb => "Bond Markets Unit European Monetary Unit (E.M.U.-6)",
                Currency.Xbc => "Bond Markets Unit European Unit of Account 9 (E.U.A.-9)",
                Currency.Xbd => "Bond Markets Unit European Unit of Account 17 (E.U.A.-17)",
                Currency.Xcd => "East Caribbean Dollar",
                Currency.Xcg => "Caribbean Guilder",
                Currency.Xdr => "SDR (Special Drawing Right)",
                Currency.Xof => "CFA Franc BCEAO",
                Currency.Xpd => "Palladium",
                Currency.Xpf => "CFP Franc",
                Currency.Xpt => "Platinum",
                Currency.Xsu => "Sucre",
                Currency.Xts => "Codes Reserved for Testing Purposes",
                Currency.Xua => "ADB Unit of Account",
                Currency.Xxx => "No Currency",

                Currency.Yer => "Yemeni Rial",

                Currency.Zar => "Rand",
                Currency.Zmw => "Zambian Kwacha",
                Currency.Zwg => "Zimbabwe Gold",

                _ => throw new ArgumentOutOfRangeException(nameof(currency), currency, "Unsupported currency.")
            };

        public string GetSymbol()
            => currency switch
            {
                Currency.Usd or Currency.Cad or Currency.Aud or Currency.Nzd or Currency.Sgd or Currency.Hkd
                    or Currency.Bzd or Currency.Fjd or Currency.Gyd or Currency.Lrd or Currency.Nad
                    or Currency.Bbd or Currency.Bmd or Currency.Bnd or Currency.Bsd or Currency.Kyd
                    or Currency.Xcd or Currency.Srd or Currency.Ttd
                    => "$",

                Currency.Eur or Currency.Che => "€",
                Currency.Gbp or Currency.Fkp or Currency.Gip or Currency.Sdg or Currency.Ssp or Currency.Shp
                    => "£",
                Currency.Jpy or Currency.Cny => "¥",
                Currency.Rub => "₽",
                Currency.Uah => "₴",
                Currency.Pln => "zł",
                Currency.Try => "₺",
                Currency.Inr => "₹",
                Currency.Krw or Currency.Kpw => "₩",
                Currency.Ils => "₪",
                Currency.Php => "₱",
                Currency.Vnd => "₫",
                Currency.Lak => "₭",
                Currency.Khr => "៛",
                Currency.Mnt => "₮",
                Currency.Kzt => "₸",
                Currency.Twd => "NT$",
                Currency.Thb => "฿",
                Currency.Myr => "RM",
                Currency.Idr => "Rp",
                Currency.Aed => "د.إ",
                Currency.Afn => "؋",
                Currency.All => "L",
                Currency.Amd => "֏",
                Currency.Aoa => "Kz",
                Currency.Azn => "₼",
                Currency.Bam => "KM",
                Currency.Bdt => "৳",
                Currency.Bgn => "лв",
                Currency.Bhd => "ب.د",
                Currency.Bif => "FBu",
                Currency.Bob => "Bs",
                Currency.Brl => "R$",
                Currency.Btn => "Nu.",
                Currency.Bwp => "P",
                Currency.Byn => "Br",
                Currency.Chf or Currency.Chw => "CHF",
                Currency.Clp => "$",
                Currency.Cop => "$",
                Currency.Crc => "₡",
                Currency.Cup => "$",
                Currency.Cve => "$",
                Currency.Czk => "Kč",
                Currency.Dkk or Currency.Nok or Currency.Sek or Currency.Isk => "kr",
                Currency.Dop => "RD$",
                Currency.Dzd => "د.ج",
                Currency.Egp => "£",
                Currency.Ern => "Nfk",
                Currency.Etb => "Br",
                Currency.Gel => "₾",
                Currency.Ghs => "₵",
                Currency.Gmd => "D",
                Currency.Gnf => "FG",
                Currency.Gtq => "Q",
                Currency.Hnl => "L",
                Currency.Htg => "G",
                Currency.Huf => "Ft",
                Currency.Iqd => "ع.د",
                Currency.Irr => "﷼",
                Currency.Jmd => "J$",
                Currency.Jod => "JD",
                Currency.Kes => "KSh",
                Currency.Kgs => "сом",
                Currency.Kmf => "CF",
                Currency.Kwd => "KD",
                Currency.Lbp => "ل.ل",
                Currency.Lkr => "Rs",
                Currency.Lsl => "L",
                Currency.Lyd => "ل.د",
                Currency.Mad => "د.م.",
                Currency.Mdl => "L",
                Currency.Mga => "Ar",
                Currency.Mkd => "ден",
                Currency.Mmk => "K",
                Currency.Mop => "MOP$",
                Currency.Mru => "UM",
                Currency.Mur => "₨",
                Currency.Mvr => "Rf",
                Currency.Mwk => "MK",
                Currency.Mxn => "$",
                Currency.Mzn => "MT",
                Currency.Ngn => "₦",
                Currency.Nio => "C$",
                Currency.Npr => "₨",
                Currency.Omr => "ر.ع.",
                Currency.Pab => "B/.",
                Currency.Pen => "S/",
                Currency.Pgk => "K",
                Currency.Pkr => "₨",
                Currency.Pyg => "₲",
                Currency.Qar => "ر.ق",
                Currency.Ron => "lei",
                Currency.Rsd => "дин.",
                Currency.Rwf => "FRw",
                Currency.Sar => "ر.س",
                Currency.Scr => "₨",
                Currency.Sle => "Le",
                Currency.Sos => "Sh",
                Currency.Stn => "Db",
                Currency.Svc => "₡",
                Currency.Syp => "£",
                Currency.Szl => "E",
                Currency.Tjs => "ЅМ",
                Currency.Tmt => "m",
                Currency.Tnd => "د.ت",
                Currency.Top => "T$",
                Currency.Tzs => "Sh",
                Currency.Ugx => "Sh",
                Currency.Uyu => "$U",
                Currency.Uzs => "so'm",
                Currency.Ved or Currency.Ves => "Bs.",
                Currency.Vuv => "VT",
                Currency.Wst => "WS$",
                Currency.Xaf or Currency.Xof or Currency.Xpf => "F",
                Currency.Xag => "XAG",
                Currency.Xau => "XAU",
                Currency.Xpd => "XPD",
                Currency.Xpt => "XPT",
                Currency.Yer => "﷼",
                Currency.Zar => "R",
                Currency.Zmw => "ZK",
                Currency.Zwg => "ZiG",

                _ => currency.GetCode()
            };

        public CurrencyInfo GetInfo()
            => new(
                Code: currency.GetCode(),
                Numeric: currency.GetNumeric(),
                FullName: currency.GetFullName(),
                Symbol: currency.GetSymbol());
    }

    public static bool TryParse(string code, out Currency currency)
    {
        if (!string.IsNullOrWhiteSpace(code))
        {
            return Enum.TryParse(code, ignoreCase: true, out currency);
        }

        currency = default;
        return false;
    }

    public static Currency Parse(string code)
        => TryParse(code, out var currency)
            ? currency
            : throw new ArgumentException($"Invalid currency code: '{code}'.", nameof(code));
}
