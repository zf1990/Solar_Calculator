using System.Net.Security;
using System;
using System.Net.Mime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Reflection;
using Application.Core;
using System.Linq.Expressions;

namespace WeatherStationSelectorTest
{
    [TestClass]
    public class WeatherStationSelectorTest
    {
        // [TestMethod]
        // public void Test_Invalid_Longitude_And_Latitude()
        // {
        //     try
        //     {

        //     }
        //     catch
        //     {

        //     }
        // }

        [TestMethod]
        public void Test_Harversine_Formula_Distance_Calculation()
        {
            //Blaine, MN
            //45.19775N
            //-93.18108

            //Beijing
            //39.9042
            //116.4074

            WeatherStationSelector selector = new WeatherStationSelector(-93.18108, 45.19775);
            double distance = selector.CalculateDistanceToPoint(116.4074, 39.9042);
            Console.WriteLine(distance);
            Assert.IsTrue(distance >= 9995 && distance <= 10197);
        }
    }
}
