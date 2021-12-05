using System;
using System.Collections.Generic;

#nullable disable

namespace Persistence.Models
{
    public partial class WeatherDatum
    {
        public int WeatherStationId { get; set; }
        public DateTime Date { get; set; }
        public double? TauB { get; set; }
        public double? TauD { get; set; }
        public double? RadAvg { get; set; }
        public double? RadStd { get; set; }

        public virtual WeatherStation WeatherStation { get; set; }
    }
}
