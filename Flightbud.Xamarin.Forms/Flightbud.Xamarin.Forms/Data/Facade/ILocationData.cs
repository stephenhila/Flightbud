using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Flightbud.Xamarin.Forms.Data.Facade
{
    public interface ILocationData
    {
        Task<Location> Get(GeolocationAccuracy accuracy, TimeSpan timeout);
    }
}
