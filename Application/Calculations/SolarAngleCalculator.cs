using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Application.Calculations
{
    public class SolarAngleCalculator
    {
        private const double _Esc = 447.6;

        private readonly double _Latitude;
        private readonly double _Longitude;

        public double[] TauBArr { get; set; }
        public double[] TauDArr { get; set; }

        private double Declination;
        private double ET; //Equation of Time
        private double Eo; //Extraterrestrial raidant flux Eo.
        private double ab; //Air mass exponents
        private double ad; //Air mass exponents;
        private double TauB; //Used to calculuate relative air mass
        private double TauD; //Used to calculate relative air mass.
        public SolarAngleCalculator(double Longitude, double Latitude, double[] TauBArr, double[] TauDArr)
        {
            this._Longitude = Longitude;
            this._Latitude = Latitude;
            this.TauBArr = TauBArr;
            this.TauDArr = TauDArr;
        }

        /// <summary>
        /// Calcululate Solar the total solar energy on a clear sky day.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public double[] CalculateForDay(DateTime time)
        {
            double[] answer = new double[24];
            int dayOfTheYear = time.DayOfYear;
            CalculateRadiantFlux(dayOfTheYear);
            CalculateEquationOfTime(dayOfTheYear);
            CalculateDeclinationAngle(dayOfTheYear);
            TauB = InterpololateValues(TauBArr, time);
            TauD = InterpololateValues(TauDArr, time);
            SetAb();
            SetAd();

            for (int i = 0; i < 24; i++)
            {
                DateTime hour = time.AddHours(i);
                answer[i] = CalculateSolarIrradiance(hour);
            }
            return answer;
        }

        /// <summary>
        /// Calculate the solar irradiance at a given time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public double CalculateSolarIrradiance(DateTime time)
        {
            double AST = CalculateAST(time);
            double hourAngle = CalculateHourAngle(AST); // H
            double solarAltitudeAngle = CalculateSolarAltitudeAngle(hourAngle); // beta
            double azimuthAngle = CalculateSolarAzimuth(hourAngle, solarAltitudeAngle); //Azimuth
            double airmass = CalculateRelativeAirMass(solarAltitudeAngle); // m
            double Eb = CalculateClearSkyBeamRadiation(airmass);
            double Ed = CalculateClearSkyDiffuseRadiation(airmass);
            return Eb + Ed;
        }


        //n is the day of the year;
        private void CalculateRadiantFlux(int n)
        {
            Eo = _Esc * (1 + 0.033 * MathTools.Cosine(360 * (n - 3) / 365));
        }

        //n is the day of the year
        private void CalculateEquationOfTime(int n)
        {
            double angle = 360 * (n - 1) / 365;
            ET = 2.2918 * (0.0075 + 0.1868 * MathTools.Cosine(angle) - 3.2077 * MathTools.Sine(angle) - 1.4615 * MathTools.Cosine(2 * angle) - 4.089 * MathTools.Sine(2 * angle));
        }

        //LST means Local Standard Time
        private double CalculateAST(DateTime LST)
        {
            double LST_hours = LST.Hour + LST.Minute / 60.0;
            double AST = LST_hours + ET / 60.0 + (_Longitude % 15) / 15.0;
            return AST;
        }

        private void CalculateDeclinationAngle(int n) =>
            Declination = 23.45 * MathTools.Sine((n + 284) / 365) * Math.PI / 180;

        private double CalculateHourAngle(double AST) =>
            15.0 * (AST - 12);

        //H is the hour angle
        private double CalculateSolarAltitudeAngle(double H)
        {
            double sinBeta = MathTools.Cosine(_Latitude) * MathTools.Cosine(Declination) * MathTools.Cosine(H) + MathTools.Sine(_Latitude) * MathTools.Sine(Declination);
            return MathTools.ASine(sinBeta);
        }

        private double CalculateSolarAzimuth(double H, double beta)
        { //H is the hour angle, beta is the solar altitude angle
            double sinOmega = MathTools.Sine(H) * MathTools.Cosine(Declination) / MathTools.Cosine(beta);
            return MathTools.ASine(sinOmega);
        }

        private double CalculateRelativeAirMass(double beta) =>
            1 / (MathTools.Sine(beta) + 0.50572 * Math.Pow((6.07995 + beta), -1.6364));

        private double CalculateClearSkyBeamRadiation(double m)
        {
            double x = -1 * TauB * Math.Pow(m, ab);
            return Eo*Math.Pow(Math.E, x);
        }

        private double CalculateClearSkyDiffuseRadiation(double m)
        {
            double x = -1 * TauD * Math.Pow(m, ad);
            return Eo * Math.Pow(Math.E, x);
        }

        private void SetAb() =>
            ab = 1.454 - 0.406 * TauB - 0.268 * TauD + 0.021 * TauB * TauD; // Checked

        private void SetAd() =>
            ad = 0.507 + 0.205 * TauB - 0.080 * TauD - 0.190 * TauB * TauD; // Checked

        public double InterpololateValues(double[] arr, DateTime date)
        {
            if (date.Day == 21)
                return arr[date.Month - 1];

            int[] indices = GetInterpoloationIndices(date);
            double previous = arr[indices[0]];
            double next = arr[indices[1]];

            DateTime previousDate = new DateTime(DateTime.Now.Year, indices[0], 21);
            int nextDateYear = previousDate.Month == 12 ? previousDate.Year + 1 : previousDate.Year;
            DateTime nextDate = new DateTime(nextDateYear, indices[1], 21);
            int totalDays = (nextDate - previousDate).Days;
            int daysPassed = (date - previousDate).Days;

            return previous + (daysPassed * 1.0 / totalDays) * (next - previous);
        }

        // TODO: Write Unit Test for this.
        public int[] GetInterpoloationIndices(DateTime date)
        {
            // Since the values are 21st of each month
            int month = date.Month;

            if (date.Day > 21)
                return new int[] { (month - 1), month % 12 };
            else
                return new int[] { (month - 2 < 0 ? 11 : month - 2), month - 1 };
            
        }



    }
}