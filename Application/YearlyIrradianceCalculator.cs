using Application.Calculations;
using Application.Core;
using Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application
{
    public class YearlyIrradianceCalculator 
    {
        internal double Latitude { get; }
        internal double Longitude { get; }
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

        public double[,] Calculate(out double max)
        {
            SolarAngleCalculator Calculator = new SolarAngleCalculator(Longitude: Longitude, Latitude: Latitude, TauBArr: TauB, TauDArr: TauD);
            //Day of the year, tilt, surfaceAzimuth + 90.  REMEMBER to subtract 90 to get the actual azimuth
            IEnumerable<double[,]> results = Calculator.CalculateForYear();
            double[,] resultSums = new double[results.First().GetLength(0), results.First().GetLength(1)];
            max = 0;
            foreach(var dailyResult in results)
            {
                for(int i=0; i<dailyResult.GetLength(0); i++)
                {
                    for(int j=0; j<dailyResult.GetLength(1); j++)
                    {
                        resultSums[i,j] += dailyResult[i,j];
                        max = Math.Max(max, resultSums[i, j]);
                    }
                }
            }
            return resultSums;
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
