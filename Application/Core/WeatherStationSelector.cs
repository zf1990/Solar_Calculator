using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using Application.Calculations;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Models;

namespace Application.Core
{

    public class WeatherStationSelector : BaseProcessor
    {
        private readonly double _Longitude;
        private readonly double _Latitude;

        private Dictionary<string, double> _Boundaries;

        public Dictionary<string, double> Boundaries
        {
            get
            {
                if (_Boundaries == null)
                {
                    _Boundaries = new Dictionary<string, double>
                    {
                        { "East", EastLongitudeBoundaryDeg },
                        { "West", WestLongitudeBoundaryDeg },
                        { "South", SouthLatitudeBoundaryDeg },
                        { "North", NorthLatitudeBoundaryDeg }
                    };
                }
                return _Boundaries;

            }
        }


        private double EastLongitudeBoundaryDeg;
        private double WestLongitudeBoundaryDeg;

        private double NorthLatitudeBoundaryDeg;
        private double SouthLatitudeBoundaryDeg;
        private readonly double _MaxDistanceInKm;

        private const double EarthRadiusKm = 6371.0;

        public WeatherStationSelector(DataContext Context, double Longitude, double Latitude, double MaxDistanceInKm = 100) : base(Context)
        {
            this._MaxDistanceInKm = MaxDistanceInKm;
            if (Longitude >= -180 && Longitude <= 180 && Latitude >= -89.5 && Latitude <= 89.5)
            {
                _Latitude = Latitude;
                _Longitude = Longitude;
            }
            else
            {
                throw new Exception("Latitude and Longitude must be valid values and cannot be located that close to the pole!");
            }

            CalculateBoundaries();
        }


        public void CalculateBoundaries()
        {

            //Calculating the upper and lower boundary for the latitude
            //For simplicity, assume 1 degree latitude will always be 111.133 km.
            NorthLatitudeBoundaryDeg = _Latitude + _MaxDistanceInKm / 111.133;
            SouthLatitudeBoundaryDeg = _Latitude - _MaxDistanceInKm / 111.133;

            double LongDif = GetLongitudeDegreeDifference();
            EastLongitudeBoundaryDeg = _Longitude + LongDif;
            WestLongitudeBoundaryDeg = _Longitude - LongDif;
        }

        public double CalculateHaversineFunction(double angle, bool IsDegree = true)
        {
            if (IsDegree)
            {
                return Math.Pow(MathTools.Sine(angle / 2), 2);
            }
            else
            {
                return Math.Pow(Math.Sin(angle / 2), 2);
            }

        }

        public double CalculateDistanceToPoint(double AnotherLongitudeDeg, double AnotherLatitudeDeg)
        {
            double d = 2 * EarthRadiusKm *
                    Math.Asin(
                        Math.Sqrt(
                            CalculateHaversineFunction(AnotherLatitudeDeg - _Latitude)
                            + MathTools.Cosine(AnotherLatitudeDeg) * MathTools.Cosine(_Latitude) * CalculateHaversineFunction(AnotherLongitudeDeg - _Longitude)
                        )
                    );
            return d;
        }

        public double GetLongitudeDegreeDifference()
        {
            double leftExpression = MathTools.ASine(
                Math.Sqrt(
                    Math.Pow(Math.Sin(_MaxDistanceInKm / EarthRadiusKm / 2), 2)
                    / Math.Pow(MathTools.Cosine(_Latitude), 2)
                    )
                );

            return leftExpression * 2;
        }

        public IList<WeatherStation> GetWeatherStations(int NumberToTake)
        {
            var results = _Context.WeatherStations.Where(
                x => x.Longitude > Boundaries["West"] &&
                x.Longitude < Boundaries["East"] &&
                x.Latitude < Boundaries["North"] &&
                x.Latitude > Boundaries["South"])
                .ToList();

            return results;
        }

    }
}