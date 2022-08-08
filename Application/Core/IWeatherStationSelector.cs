using Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Core
{
    public interface IWeatherStationSelector
    {
        IList<WeatherStation> LoadWeatherStations();
        WeatherStation GetClosestWeatherStation();
        IList<WeatherStation> GetClosestWeatherStations(int NumberToTake);

    }
}
