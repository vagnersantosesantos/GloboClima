namespace GloboClima.Core.Models
{
    public class CountryData
    {
        public string Name { get; set; } = string.Empty;
        public string Capital { get; set; } = string.Empty;
        public long Population { get; set; }
        public string Region { get; set; } = string.Empty;
        public List<string> Languages { get; set; } = new();
        public List<string> Currencies { get; set; } = new();
        public string Flag { get; set; } = string.Empty;
        public double Area { get; set; }

        public List<string> AllCapitals { get; set; } = new();
        public string FormattedPopulation => Population.ToString("N0");
        public string FormattedArea => Area.ToString("N0") + " km²";
    }

}
