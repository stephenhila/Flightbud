using Flightbud.Xamarin.Forms.Data;
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

        public event MapPanningEventHandler MapPanning;
        public event VisibleRegionChangedEventHandler VisibleRegionChanged;
        public event MapItemDetailsRequestedEventHandler MapItemDetailsRequested;
    }

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
