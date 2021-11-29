using System;
using System.Collections.Generic;

#nullable disable

namespace Persistence.Models
{
    public partial class WeatherStationInformation
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public double? TauB { get; set; }
        public double? TauD { get; set; }
        public double? RadAvg { get; set; }
        public double? RadStd { get; set; }
        public double TimeZone { get; set; }
    }
}
