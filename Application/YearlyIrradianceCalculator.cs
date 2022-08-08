using Application.Calculations;
using Application.Core;
using Persistence.Models;
using System;

namespace Application
{
    public class YearlyIrradianceCalculator : IYearlyIrradianceCalculator
    {
        public double Latitude { get; }
        public double Longitude { get; }
        public IWeatherStationSelector Selector { get; }
        private WeatherStation WeatherStaton { get; set; }
        public YearlyIrradianceCalculator(double Latitude, double Longitude, IWeatherStationSelector Selector)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Selector = Selector;
            GetClosestWeatherStation();
        }

        public double Calculate()
        {
            SolarAngleCalculator Calculator = new SolarAngleCalculator(Longtitude: Longtitude, Latitude: Latitude, TauBArr: this.WeatherStaton.WeatherData.)
        }

        private void GetClosestWeatherStation() =>        
            WeatherStaton = Selector.GetClosestWeatherStation();
        
    }
}
