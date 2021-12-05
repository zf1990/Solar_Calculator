using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Application.Calculations
{
    public class SolarAngleCalculator
    {
        private readonly double _Esc;
        private readonly double _Latitude;
        private readonly double _Longitude;

        public Dictionary<DateTime, double> TauB { get; set; }

        public Dictionary<DateTime, double> TauD { get; set; }

        private double declination;
        private double ET; //Equation of Time
        private double Eo; //Extraterrestrial raidant flux Eo.
        public SolarAngleCalculator(double Longitude, double Latitude, double Esc = 447.6)
        {
            this._Longitude = Longitude;
            this._Latitude = Latitude;
            this._Esc = Esc;
        }

        private int GetDayOfTheYear(DateTime date)
        {
            return date.DayOfYear;
        }

        //n is the day of the year;
        private void CalculateRadiantFlux(int n)
        {
            if (n > 366 || n < 1)
                throw new Exception("The day is invalid");

            Eo = _Esc * (1 + 0.033 * MathTools.Cosine(360 * (n - 3) / 365));
        }

        //n is the day of the year
        private void CalculateEquationOfTime(int n)
        {
            double angle = 2 * Math.PI * (n - 1) / 365;
            ET = 2.2918 * (0.0075 + 0.1868 * Math.Cos(angle) - 3.2077 * Math.Sin(angle) - 1.4615 * Math.Cos(2 * angle) - 4.089 * Math.Sin(2 * angle));
        }

        //LST means Local Standard Time
        private double CalculateAST(DateTime LST)
        {
            double LST_hours = LST.Hour + LST.Minute / 60.0;
            double AST = LST_hours + ET / 60.0 + (_Longitude % 15) / 15.0;
            return AST;
        }

        private void CalculateDeclinationAngle(int n)
        {
            declination = 23.45 * Math.Sin(2 * Math.PI * (n + 284) / 365) * Math.PI / 180;
        }

        private double CalculateHourAngle(double AST)
        {
            return 15 * (AST - 12);
        }

        //H is the hour angle
        private double CalculateSolarAltitudeAngle(double H)
        {
            double sinBeta = Math.Cos(_Latitude) * Math.Cos(declination) * Math.Cos(H) + Math.Sin(_Latitude) * Math.Sin(declination);
            return Math.Asin(sinBeta);
        }

        private double CalculateSolarAzimuth(double H, double beta)
        { //H is the hour angle, beta is the solar altitude angle
            double sinOmega = Math.Sin(H) * Math.Cos(declination) / Math.Cos(beta);
            return Math.Asin(sinOmega);
        }

        private double CalculateRelativeAirMass(double beta)
        {
            double betaDegrees = beta / Math.PI * 180;
            return 1 / (Math.Sin(beta) + 0.50572 * Math.Pow((6.07995 + betaDegrees), -1.6364));
        }


    }
}