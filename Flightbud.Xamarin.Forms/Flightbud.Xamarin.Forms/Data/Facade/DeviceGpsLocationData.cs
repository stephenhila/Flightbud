﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Flightbud.Xamarin.Forms.Data.Facade
{
    public class DeviceGpsLocationData : ILocationData
    {
        public async Task<Location> Get(GeolocationAccuracy accuracy, int timeout, CancellationToken ct)
        {
            return await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromMilliseconds(timeout)), ct);
        }
    }
}
