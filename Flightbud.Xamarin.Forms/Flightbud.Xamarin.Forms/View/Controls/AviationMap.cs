﻿using Flightbud.Xamarin.Forms.Data;
using Flightbud.Xamarin.Forms.Data.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.View.Controls
{
    public class AviationMap : Map
    {
        public static readonly BindableProperty VisibleRegionChangedFrequencyProperty = 
            BindableProperty.Create(
                propertyName: nameof(VisibleRegionChangedFrequency),
                returnType: typeof(double),
                declaringType: typeof(AviationMap),
                defaultValue: default,
                defaultBindingMode: BindingMode.OneWay);
        public double VisibleRegionChangedFrequency
        {
            get { return Convert.ToDouble(base.GetValue(VisibleRegionChangedFrequencyProperty)); }
        }

        Stopwatch _stopwatch;

        public AviationMap()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public async Task OnVisibleRegionChanged(VisibleRegionChangedEventArgs e)
        {
            if (VisibleRegionChanged != null)
            {
                if (_stopwatch.ElapsedMilliseconds > VisibleRegionChangedFrequency)
                {
                    if (VisibleRegion.Radius.Kilometers < Constants.LOCATION_ITEMS_REGION_SPAN_RADIUS_THRESHOLD)
                    {
                        await Task.Run(() => VisibleRegionChanged(this, e));
                    }
                    _stopwatch.Restart();
                }
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
        public MapSpan VisibleRegion { get; set; }
    }
    public delegate Task VisibleRegionChangedEventHandler(object sender, VisibleRegionChangedEventArgs e);

    public class MapItemDetailsRequestedEventArgs : EventArgs
    {
        public MapItemBase SelectedMapItem { get; set; }
    }
    public delegate Task MapItemDetailsRequestedEventHandler(object sender, MapItemDetailsRequestedEventArgs e);
}
