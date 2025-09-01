namespace GloboClima.Core.Models
{
    public class WeatherData
    {
        public string CityName { get; set; }
        public string Country { get; set; }
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public double TempMin { get; set; }
        public double TempMax { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public int SeaLevel { get; set; }
        public int GrndLevel { get; set; }

        public string WeatherMain { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        public double WindSpeed { get; set; }
        public int WindDeg { get; set; }
        public double WindGust { get; set; }

        public double? Rain1h { get; set; }
        public int CloudsAll { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int Visibility { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
        public int Timezone { get; set; }
    }
}
