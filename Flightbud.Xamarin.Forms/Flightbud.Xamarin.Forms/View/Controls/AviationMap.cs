﻿using Flightbud.Xamarin.Forms.Data;
using Flightbud.Xamarin.Forms.Data.Models;
using Flightbud.Xamarin.Forms.View.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.View.Controls
{
    public class AviationMap : Map
    {
        public List<AirportPin> AirportPins
        {
            get
            {
                return ItemsSource?.OfType<Airport>()?.Select(a => a.MapPin).ToList();
            }
        }

        public List<VorPin> VorPins
        {
            get
            {
                return ItemsSource?.OfType<Navaid>()?.Where(p => p.MapPin is VorPin).Select(a => a.MapPin as VorPin).ToList();
            }
        }

        public List<NdbPin> NdbPins
        {
            get
            {
                return ItemsSource?.OfType<Navaid>()?.Where(p => p.MapPin is NdbPin).Select(a => a.MapPin as NdbPin).ToList();
            }
        }

        public AviationMap()
        {

        }

        public async Task OnVisibleRegionChanged(VisibleRegionChangedEventArgs e)
        {
            if (VisibleRegionChanged != null && VisibleRegion.Radius.Kilometers < Constants.LOCATION_ITEMS_REGION_SPAN_RADIUS_THRESHOLD)
            {
                await VisibleRegionChanged(this, e);
            }
        }

        public async Task OnMapItemDetailsRequested(MapItemDetailsRequestedEventArgs e)
        {
            if (MapItemDetailsRequested != null)
            {
                await MapItemDetailsRequested(this, e);
            }
        }

        public event VisibleRegionChangedEventHandler VisibleRegionChanged;
        public event MapItemDetailsRequestedEventHandler MapItemDetailsRequested;
    }


    public class VisibleRegionChangedEventArgs : EventArgs
    {
        // just in-case we need more parameters for the event args.. never know..
    }
    public delegate Task VisibleRegionChangedEventHandler(object sender, VisibleRegionChangedEventArgs e);

    public class MapItemDetailsRequestedEventArgs : EventArgs
    {
        public MapItemBase SelectedMapItem { get; set; }
    }
    public delegate Task MapItemDetailsRequestedEventHandler(object sender, MapItemDetailsRequestedEventArgs e);
}
