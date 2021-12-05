using System;

namespace Application.Calculations
{
    public static class MathTools
    {
        public static double Sine(double degree)
        {
            var rad = DegreeToRadian(degree);
            return Math.Sin(rad);
        }

        public static double Cosine(double degree)
        {
            var rad = DegreeToRadian(degree);
            return Math.Cos(rad);
        }

        public static double ACosine(double ratio)
        {
            return RadianToDegree(Math.Acos(ratio));
        }

        public static double ASine(double ratio)
        {
            return RadianToDegree(Math.Asin(ratio));
        }

        public static double RadianToDegree(double rad)
        {
            return rad * 180 / Math.PI;
        }

        public static double DegreeToRadian(double degree)
        {
            return degree / 180.0 * Math.PI;
        }
    }
}