using System;
using System.Collections.Generic;

#nullable disable

namespace Persistence.Models
{
    public partial class WeatherStation
    {
        public WeatherStation()
        {
            WeatherData = new HashSet<WeatherDatum>();
        }

        public int WeatherStationId { get; set; }
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double TimeZone { get; set; }

        public virtual ICollection<WeatherDatum> WeatherData { get; set; }
    }
}
