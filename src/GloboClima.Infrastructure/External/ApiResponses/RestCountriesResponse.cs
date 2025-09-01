using System.Text.Json.Serialization;

namespace GloboClima.Infrastructure.External.ApiResponses
{
    public class RestCountriesResponse
    {
        public NameData Name { get; set; } = new();

        [JsonPropertyName("capital")]
        public List<string>? Capital { get; set; }

        [JsonPropertyName("population")]
        public long Population { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; } = string.Empty;

        [JsonPropertyName("languages")]
        public Dictionary<string, string>? Languages { get; set; }

        [JsonPropertyName("currencies")]
        public Dictionary<string, CurrencyData>? Currencies { get; set; }

        [JsonPropertyName("flags")]
        public FlagsData Flags { get; set; } = new();

        [JsonPropertyName("area")]
        public double Area { get; set; }

        public class NameData
        {
            [JsonPropertyName("common")]
            public string Common { get; set; } = string.Empty;

            [JsonPropertyName("official")]
            public string Official { get; set; } = string.Empty;
        }

        public class CurrencyData
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("symbol")]
            public string Symbol { get; set; } = string.Empty;
        }

        public class FlagsData
        {
            [JsonPropertyName("png")]
            public string Png { get; set; } = string.Empty;

            [JsonPropertyName("svg")]
            public string Svg { get; set; } = string.Empty;
        }
    }
}
