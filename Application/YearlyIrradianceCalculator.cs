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

        private double?[] TauB { get; set; }
        private double?[] TauD { get; set; }
        public YearlyIrradianceCalculator(double Latitude, double Longitude, IWeatherStationSelector Selector)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Selector = Selector;
            GetClosestWeatherStation();
            GetTauB();
            GetTauD();
        }

        public double[,,] Calculate()
        {
            double[,,] sums = new double[181, 91, 365]; // Each number represent yearly totals
            SolarAngleCalculator Calculator = new SolarAngleCalculator(Longitude: Longitude, Latitude: Latitude, TauBArr: TauB, TauDArr: TauD);
            DateTime day = new DateTime(2022, 1, 1);
            for(int i=0; i<365; i++)
            {
                double[][] values = Calculator.CalculateForDay(day);

                // Todo: Add unit tests.
                day = day.AddDays(1);
            }

            return sums;
        }

        private void GetClosestWeatherStation() =>        
            WeatherStaton = Selector.GetClosestWeatherStation();

        private void GetTauB()
        {
            TauB = this.WeatherStaton.WeatherData
                .OrderBy(w => w.Date)
                .Select(w => w.TauB)
                .ToArray();
        }

        private void GetTauD()
        {
            TauD = this.WeatherStaton.WeatherData
                .OrderBy(w => w.Date)
                .Select(w => w.TauD)
                .ToArray();
        }

    }
}
