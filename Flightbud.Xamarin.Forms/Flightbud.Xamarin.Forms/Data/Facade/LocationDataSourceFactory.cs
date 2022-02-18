using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace Flightbud.Xamarin.Forms.Data.Facade
{
    public class LocationDataSourceFactory
    {
        public static ILocationData Create()
        {
            if (Preferences.Get("EnableSimConnect", false))
            {
                return new SimConnectGpsLocationData();
            }
            else
            {
                return new DeviceGpsLocationData();
            }
        }
    }
}
