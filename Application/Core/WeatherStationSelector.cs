using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Core
{

    public class WeatherStationSelector
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

        public WeatherStationSelector(double Longitude, double Latitude, double MaxDistanceInKm = 100)
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

        //Limit the 
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
                double radian = DegToRad(angle);
                return Math.Pow(Math.Sin(radian / 2), 2);
            }
            else
            {
                return Math.Pow(Math.Sin(angle / 2), 2);
            }

        }

        public double CalculateDistanceToPoint(double AnotherLongitudeDeg, double AnotherLatitudeDeg)
        {
            double AnotherLatitudeRad = DegToRad(AnotherLatitudeDeg);
            double LatitudeRad = DegToRad(_Latitude);
            double d = 2 * EarthRadiusKm *
                    Math.Asin(
                        Math.Sqrt(
                            CalculateHaversineFunction(AnotherLatitudeDeg - _Latitude)
                            + Math.Cos(LatitudeRad) * Math.Cos(AnotherLatitudeRad) * CalculateHaversineFunction(AnotherLongitudeDeg - _Longitude)
                        )
                    );
            return d;
        }

        public double GetLongitudeDegreeDifference()
        {
            double leftExpression = Math.Asin(
                Math.Sqrt(
                    Math.Pow(Math.Sin(_MaxDistanceInKm / EarthRadiusKm / 2), 2)
                    / Math.Pow(Math.Cos(DegToRad(_Latitude)), 2)
                    )
                );
            double leftExpressionDeg = RadToDeg(leftExpression);
            return leftExpressionDeg * 2;
        }

        public double DegToRad(double degree)
        {
            return degree / 180.0 * Math.PI;
        }

        public double RadToDeg(double radian)
        {
            return radian / Math.PI * 180.0;
        }


    }
}