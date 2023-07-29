using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Collections.Concurrent;

namespace Application.Calculations
{
    public class SolarAngleCalculator
    {
        private const double _Esc = 447.6;

        private readonly double _Latitude;
        private readonly double _Longitude;

        public double?[] TauBArr { get; set; }
        public double?[] TauDArr { get; set; }

        private double Declination;
        private double ET; //Equation of Time
        private double Eo; //Extraterrestrial raidant flux Eo.
        private double ab; //Air mass exponents
        private double ad; //Air mass exponents;
        private double TauB; //Used to calculuate relative air mass
        private double TauD; //Used to calculate relative air mass.
        public SolarAngleCalculator(double Longitude, double Latitude, double?[] TauBArr, double?[] TauDArr)
        {
            this._Longitude = Longitude;
            this._Latitude = Latitude;
            this.TauBArr = TauBArr;
            this.TauDArr = TauDArr;
        }

        /// <summary>
        /// Calculate the total solar energy a single spot receives per year.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double[,]> CalculateForYear()
        {
            var results = new ConcurrentBag<double[,]>();
            List<DateTime> dates = new List<DateTime>(365);
            DateTime time = new DateTime(DateTime.Now.Year, 1, 1, 12, 0, 0);
            for(int i=0; i<dates.Count; i++)
            {
                dates.Add(time);
                time.AddDays(1);
            }

            Parallel.ForEach(dates, date =>
            {
                results.Add(CalculateForDay(date));
            });

            return results;
        }

        /// <summary>
        /// Calcululate Solar the total solar energy on a clear sky day.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        internal double[,] CalculateForDay(DateTime time)
        {
            double[,] answer = new double[91, 181]; // 24 entries. Each sub array represent Eb, Ed, and Er.
            int dayOfTheYear = time.DayOfYear;
            CalculateRadiantFlux(dayOfTheYear);
            CalculateEquationOfTime(dayOfTheYear);
            CalculateDeclinationAngle(dayOfTheYear);
            TauB = InterpololateValues(TauBArr, time);
            TauD = InterpololateValues(TauDArr, time);
            SetAb();
            SetAd();
            // 0 is facing south. Therefore, this is going from east to west.
            for(int surfaceAzimuth = -90; surfaceAzimuth <= 90; surfaceAzimuth++)
            {
                for (int tilt = 0; tilt <= 90; tilt++)
                {
                    double sum = 0.0;
                    for (int hour = 0; hour < 24; hour++)
                    {
                        sum += CalculateSolarIrradiance(time, tilt, surfaceAzimuth);
                        time = time.AddHours(1);
                    }
                    answer[tilt,surfaceAzimuth + 90] = sum;
                }
            }
            
            return answer;
        }

        /// <summary>
        /// Calculate the solar irradiance at a given time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public double CalculateSolarIrradiance(DateTime time, double tilt, double surfaceAzimuth)
        {
            double AST = CalculateAST(time);
            double hourAngle = CalculateHourAngle(AST); // H
            double solarAltitudeAngle = CalculateSolarAltitudeAngle(hourAngle); // beta
            double solarAzimuthAngle = CalculateSolarAzimuth(hourAngle, solarAltitudeAngle); //Azimuth
            double surfaceSolarAzimuthAngle = solarAzimuthAngle - surfaceAzimuth; //Gamma
            double angleOfIncidence = CalculateAngleOfIncidence(solarAltitudeAngle, surfaceSolarAzimuthAngle, tilt); //theta
            double airmass = CalculateRelativeAirMass(solarAltitudeAngle); // m
            double Eb = CalculateClearSkyBeamRadiation(airmass);
            double Ed = CalculateClearSkyDiffuseRadiation(airmass);

            double Er = (Eb*MathTools.Sine(solarAltitudeAngle) + Ed) * 0.2 * (1 + MathTools.Cosine(solarAltitudeAngle)) / 2; // Ground-reflected irradiance. ρg is assumed to be 0.2 for a mixture of ground surfaces.
            double Etb = Eb * MathTools.Cosine(angleOfIncidence); // Modified Eb.
            double Y = Math.Max(0.45, 0.55 + 0.437 * MathTools.Cosine(angleOfIncidence) + 0.313 * Math.Pow(MathTools.Cosine(angleOfIncidence), 2)); // Y for diffuse radiation
            double Etd = Ed * (Y * MathTools.Sine(tilt) + MathTools.Cosine(tilt)); // Diffuse radiation represents the non-direct irradiance being reflected.,
            return Etb + Etd + Er;
        }

        private double CalculateAngleOfIncidence(double solarAltitudeAngle, double surfaceAzimuthAngle, double tilt)
        {
            // theta = angle of incidence
            // beta = solar altitude angle
            // gamma = surface azimuth angle
            // sigma tilt = angle between the horizontal surface and the surface of the panel.
            double cos_theta = MathTools.Cosine(solarAltitudeAngle) * MathTools.Cosine(surfaceAzimuthAngle) * MathTools.Cosine(tilt) + MathTools.Sine(solarAltitudeAngle) * MathTools.Cosine(tilt);
            return MathTools.ACosine(cos_theta);
        }

        //n is the day of the year;
        internal void CalculateRadiantFlux(int n)
        {
            Eo = _Esc * (1 + 0.033 * MathTools.Cosine(360 * (n - 3) / 365));
        }

        //n is the day of the year
        internal void CalculateEquationOfTime(int n)
        {
            double angle = 360 * (n - 1) / 365;
            ET = 2.2918 * (0.0075 + 0.1868 * MathTools.Cosine(angle) - 3.2077 * MathTools.Sine(angle) - 1.4615 * MathTools.Cosine(2 * angle) - 4.089 * MathTools.Sine(2 * angle));
        }

        //LST means Local Standard Time
        internal double CalculateAST(DateTime LST)
        {
            double LST_hours = LST.Hour + LST.Minute / 60.0;
            double AST = LST_hours + ET / 60.0 + (_Longitude % 15) / 15.0;
            return AST;
        }

        internal void CalculateDeclinationAngle(int n) =>
            Declination = 23.45 * MathTools.Sine((n + 284) / 365) * Math.PI / 180;

        internal double CalculateHourAngle(double AST) =>
            15.0 * (AST - 12);

        //H is the hour angle
        internal double CalculateSolarAltitudeAngle(double H)
        {
            double sinBeta = MathTools.Cosine(_Latitude) * MathTools.Cosine(Declination) * MathTools.Cosine(H) + MathTools.Sine(_Latitude) * MathTools.Sine(Declination);
            return MathTools.ASine(sinBeta);
        }

        internal double CalculateSolarAzimuth(double H, double beta)
        { //H is the hour angle, beta is the solar altitude angle
            double sinOmega = MathTools.Sine(H) * MathTools.Cosine(Declination) / MathTools.Cosine(beta);
            return MathTools.ASine(sinOmega);
        }

        internal double CalculateRelativeAirMass(double beta) =>
            1 / (MathTools.Sine(beta) + 0.50572 * Math.Pow((6.07995 + beta), -1.6364));

        internal double CalculateClearSkyBeamRadiation(double m)
        {
            double x = -1 * TauB * Math.Pow(m, ab);
            return Eo*Math.Pow(Math.E, x);
        }

        internal double CalculateClearSkyDiffuseRadiation(double m)
        {
            double x = -1 * TauD * Math.Pow(m, ad);
            return Eo * Math.Pow(Math.E, x);
        }

        internal void SetAb() =>
            ab = 1.454 - 0.406 * TauB - 0.268 * TauD + 0.021 * TauB * TauD; // Checked

        internal void SetAd() =>
            ad = 0.507 + 0.205 * TauB - 0.080 * TauD - 0.190 * TauB * TauD; // Checked

        internal double InterpololateValues(double?[] arr, DateTime date)
        {
            if (date.Day == 21)
                return arr[date.Month - 1].Value;

            int[] indices = GetInterpoloationIndices(date);
            double previous = arr[indices[0]].Value;
            double next = arr[indices[1]].Value;

            DateTime previousDate = new DateTime(DateTime.Now.Year, indices[0], 21);
            int nextDateYear = previousDate.Month == 12 ? previousDate.Year + 1 : previousDate.Year;
            DateTime nextDate = new DateTime(nextDateYear, indices[1], 21);
            int totalDays = (nextDate - previousDate).Days;
            int daysPassed = (date - previousDate).Days;

            return previous + (daysPassed * 1.0 / totalDays) * (next - previous);
        }

        // TODO: Write Unit Test for this.
        internal int[] GetInterpoloationIndices(DateTime date)
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