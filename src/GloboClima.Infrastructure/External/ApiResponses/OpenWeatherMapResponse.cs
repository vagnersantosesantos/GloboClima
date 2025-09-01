namespace GloboClima.Infrastructure.External.ApiResponses
{
    public class OpenWeatherMapResponse
    {
        public CoordData Coord { get; set; } = new();
        public List<WeatherInfo> Weather { get; set; } = new();
        public string Base { get; set; } = string.Empty;
        public MainData Main { get; set; } = new();
        public int Visibility { get; set; }
        public WindData Wind { get; set; } = new();
        public RainData Rain { get; set; } = new();
        public CloudsData Clouds { get; set; } = new();
        public long Dt { get; set; }
        public SysData Sys { get; set; } = new();
        public int Timezone { get; set; }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Cod { get; set; }

        public class CoordData
        {
            public double Lon { get; set; }
            public double Lat { get; set; }
        }

        public class WeatherInfo
        {
            public int Id { get; set; }
            public string Main { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
        }

        public class MainData
        {
            public double Temp { get; set; }
            public double FeelsLike { get; set; }
            public double TempMin { get; set; }
            public double TempMax { get; set; }
            public int Pressure { get; set; }
            public int Humidity { get; set; }
            public int SeaLevel { get; set; }
            public int GrndLevel { get; set; }
        }

        public class WindData
        {
            public double Speed { get; set; }
            public int Deg { get; set; }
            public double Gust { get; set; }
        }

        public class RainData
        {
            public double OneH { get; set; }
        }

        public class CloudsData
        {
            public int All { get; set; }
        }

        public class SysData
        {
            public int Type { get; set; }
            public int Id { get; set; }
            public string Country { get; set; } = string.Empty;
            public long Sunrise { get; set; }
            public long Sunset { get; set; }
        }
    }
}
