using Application.Calculations;
using Application.Core;
using Persistence.Models;
using System;
using System.Linq;

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
            SolarAngleCalculator Calculator = new SolarAngleCalculator(Longitude: Longitude, Latitude: Latitude, TauBArr: GetTauB(), TauDArr: GetTauD());
        }

        private void GetClosestWeatherStation() =>        
            WeatherStaton = Selector.GetClosestWeatherStation();

        private double?[] GetTauB()
        {
            return this.WeatherStaton.WeatherData
                .OrderBy(w => w.Date)
                .Select(w => w.TauB)
                .ToArray();
        }

        private double?[] GetTauD()
        {
            return this.WeatherStaton.WeatherData
                .OrderBy(w => w.Date)
                .Select(w => w.TauD)
                .ToArray();
        }

    }
}
