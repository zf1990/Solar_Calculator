using System;
using System.Collections.Generic;

#nullable disable

namespace Persistence.Models
{
    public partial class WeatherStation
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? TimeZone { get; set; }
    }
}
