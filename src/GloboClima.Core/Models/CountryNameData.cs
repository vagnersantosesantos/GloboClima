namespace GloboClima.Core.Models
{
    public class CountryNameData
    {
        public string Name { get; set; } = string.Empty;
        public string OfficialName { get; set; } = string.Empty;
        public string NativeName { get; set; } = string.Empty;

        public List<string> Tld { get; set; } = new();
        public string Cca2 { get; set; } = string.Empty;
        public string Cca3 { get; set; } = string.Empty;
        public string Cioc { get; set; } = string.Empty;

        public bool Independent { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool UnMember { get; set; }

        public Dictionary<string, CurrencyData> Currencies { get; set; } = new();
        public List<string> Capital { get; set; } = new();

        public string Region { get; set; } = string.Empty;
        public string Subregion { get; set; } = string.Empty;

        public Dictionary<string, string> Languages { get; set; } = new();
        public List<double> Latlng { get; set; } = new();

        public bool Landlocked { get; set; }
        public List<string> Borders { get; set; } = new();

        public double Area { get; set; }
        public long Population { get; set; }

        public Dictionary<string, TranslationData> Translations { get; set; } = new();

        public string Flag { get; set; } = string.Empty;
        public string FlagPng { get; set; } = string.Empty;
        public string FlagSvg { get; set; } = string.Empty;

        // Nested reuso
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
    }

}
