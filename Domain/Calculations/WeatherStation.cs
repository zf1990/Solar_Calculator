namespace Domain
{
    public class WeatherStation
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public ClimateZoneEnum ClimateZone { get; set; }
    }
}