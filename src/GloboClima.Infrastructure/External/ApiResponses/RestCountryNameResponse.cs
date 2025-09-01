namespace GloboClima.Infrastructure.External.ApiResponses
{
    public class RestCountryNameResponse
    {
        public NameData Name { get; set; } = new();
        public List<string> Tld { get; set; } = new();
        public string Cca2 { get; set; } = string.Empty;
        public string Cca3 { get; set; } = string.Empty;
        public string Cioc { get; set; } = string.Empty;
        public bool Independent { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool UnMember { get; set; }

        public Dictionary<string, CurrencyData>? Currencies { get; set; }

        public List<string> Capital { get; set; } = new();
        public string Region { get; set; } = string.Empty;
        public string Subregion { get; set; } = string.Empty;
        public Dictionary<string, string>? Languages { get; set; }
        public List<double> Latlng { get; set; } = new();
        public bool Landlocked { get; set; }
        public List<string> Borders { get; set; } = new();
        public double Area { get; set; }
        public long Population { get; set; }
        public Dictionary<string, TranslationData>? Translations { get; set; }
        public string Flag { get; set; } = string.Empty;
        public FlagData Flags { get; set; } = new();

        public class NameData
        {
            public string Common { get; set; } = string.Empty;
            public string Official { get; set; } = string.Empty;
            public Dictionary<string, NativeNameData>? NativeName { get; set; }

            public class NativeNameData
            {
                public string Common { get; set; } = string.Empty;
                public string Official { get; set; } = string.Empty;
            }
        }

        public class CurrencyData
        {
            public string Symbol { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }

        public class TranslationData
        {
            public string Official { get; set; } = string.Empty;
            public string Common { get; set; } = string.Empty;
        }

        public class FlagData
        {
            public string Png { get; set; } = string.Empty;
            public string Svg { get; set; } = string.Empty;
            public string Alt { get; set; } = string.Empty;
        }
    }

}
