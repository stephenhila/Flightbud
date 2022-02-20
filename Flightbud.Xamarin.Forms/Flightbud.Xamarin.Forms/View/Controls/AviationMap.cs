using Flightbud.Xamarin.Forms.Data;
using Flightbud.Xamarin.Forms.Data.Models;
using Flightbud.Xamarin.Forms.View.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Map = Xamarin.Forms.Maps.Map;

namespace Flightbud.Xamarin.Forms.View.Controls
{
    public class AviationMap : Map
    {
        public static readonly BindableProperty CurrentLocationPinProperty =
        BindableProperty.Create(
        propertyName: nameof(CurrentLocationPin),
        returnType: typeof(LocationPin),
        declaringType: typeof(AviationMap),
        defaultValue: default,
        defaultBindingMode: BindingMode.OneWayToSource);
        public LocationPin CurrentLocationPin
        {
            get { return base.GetValue(CurrentLocationPinProperty) as LocationPin; }
            private set { SetValue(CurrentLocationPinProperty, value); }
        }

        public AviationMap()
        {
            CurrentLocationPin = new LocationPin { Label = "current position" };
            Pins.Add(CurrentLocationPin);
        }

        public async Task OnMapPanning(MapPanningEventArgs e)
        {
            if (MapPanning != null)
            {
                await MapPanning(this, e);
            }
        }

        public async Task OnVisibleRegionChanged(VisibleRegionChangedEventArgs e)
        {
            if (VisibleRegionChanged != null)
            {
                await Task.Run(() => VisibleRegionChanged(this, e));
            }
        }

        public async Task OnMapItemDetailsRequested(MapItemDetailsRequestedEventArgs e)
        {
            if (MapItemDetailsRequested != null)
            {
                await MapItemDetailsRequested(this, e);
            }
        }

        public async Task OnCurrentLocationChanged(CurrentLocationChangedEventArgs e)
        {
            if (CurrentLocationChanged != null)
            {
                await CurrentLocationChanged(this, e);
            }
        }

        public event CurrentLocationChangedEventHandler CurrentLocationChanged;
        public event MapPanningEventHandler MapPanning;
        public event VisibleRegionChangedEventHandler VisibleRegionChanged;
        public event MapItemDetailsRequestedEventHandler MapItemDetailsRequested;
    }

    public class CurrentLocationChangedEventArgs : EventArgs
    {
        public Location NewLocation;
    }
    public delegate Task CurrentLocationChangedEventHandler(object sender, CurrentLocationChangedEventArgs e);

    public class MapPanningEventArgs : EventArgs
    {

    }
    public delegate Task MapPanningEventHandler(object sender, MapPanningEventArgs e);

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
