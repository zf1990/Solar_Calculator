using System.Net.Security;
using System;
using System.Net.Mime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Reflection;
using Application.Core;
using System.Linq.Expressions;
using System.ComponentModel;
using Persistence.Models;
using Persistence;

namespace WeatherStationSelectorTest
{
    [TestClass]
    public class WeatherStationSelectorTest
    {
        DataContext context = new DataContext();

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Test_Invalid_Inputs()
        {
            WeatherStationSelector selector = new WeatherStationSelector(context, -93.18108, 190);
        }

        [TestMethod]
        public void Test_Harversine_Formula_Distance_Calculation()
        {
            //Blaine, MN
            //45.19775N
            //-93.18108

            //Beijing
            //39.9042
            //116.4074

            WeatherStationSelector selector = new WeatherStationSelector(context, -93.18108, 45.19775);
            double distance = selector.CalculateDistanceToPoint(116.4074, 39.9042);
            Console.WriteLine(distance);
            Assert.IsTrue(distance >= 9995 && distance <= 10197);
        }

        [TestMethod]
        public void Test_For_Appropriate_Latitude_Boundaries()
        {
            //Blaine, MN
            //45.19775N
            //-93.18108

            WeatherStationSelector selector = new WeatherStationSelector(context, -93.18108, 45.19775, 50);
            selector.CalculateBoundaries();



            double NorthBoundary = selector.Boundaries["North"];
            double SouthBoundary = selector.Boundaries["South"];

            //North boundary should be the values listed.
            Assert.IsTrue(Math.Abs(45.6476613674061 - NorthBoundary) < 0.01 && Math.Abs(44.74783863253939 - SouthBoundary) < 0.01);
        }

        [TestMethod]
        public void Test_For_Appropriate_Longitude_Boundaries()
        {
            //Blaine, MN
            //45.19775N
            //-93.18108

            WeatherStationSelector selector = new WeatherStationSelector(context, -93.18108, 45.19775, 160);
            selector.CalculateBoundaries();

            double EastBoundary = selector.Boundaries["East"];
            double WestBoundary = selector.Boundaries["West"];
            Console.WriteLine("East Boundary is {0}", EastBoundary);

            Console.WriteLine("West Boundary is {0}", WestBoundary);

            //East and west boundary should be the values listed.
            Assert.IsTrue(Math.Abs(-91.13869956 - EastBoundary) < 0.01 && Math.Abs(-95.22346044 - WestBoundary) < 0.01);
        }

        [TestMethod]
        public void Test_For_WeatherStations()
        {
            WeatherStationSelector selector = new WeatherStationSelector(context, -93.18108, 45.19775);
            selector.CalculateBoundaries();

            WeatherStation station = selector.GetClosestWeatherStation();
            Assert.IsTrue(station.Name.ToUpper().Contains("ANOKA COUNTY"));
        }

        [TestMethod]
        public void Test_For_MultipleWeatherStation()
        {
            WeatherStationSelector selector = new WeatherStationSelector(context, 118.8, 31.74, 50);
            var stations = selector.GetClosestWeatherStations(20);
            foreach (var station in stations)
            {
                Console.WriteLine($"Station name is: {station.Name}");
            }
            Assert.IsTrue(stations.Count == 1);
        }
    }


}
