using Flightbud.Xamarin.Forms.Data;
using Flightbud.Xamarin.Forms.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.View.Models
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
        public AviationMap()
        {

        }

        public void OnVisibleRegionChanged(VisibleRegionChangedEventArgs e)
        {
            if (VisibleRegionChanged != null && VisibleRegion.Radius.Kilometers < Constants.LOCATION_ITEMS_REGION_SPAN_RADIUS_THRESHOLD)
            {
                VisibleRegionChanged(this, e);
            }
        }

        public event VisibleRegionChangedEventHandler VisibleRegionChanged;
    }


    public class VisibleRegionChangedEventArgs : EventArgs
    {
        // just in-case we need more parameters for the event args.. never know..
    }
    public delegate void VisibleRegionChangedEventHandler(Object sender, VisibleRegionChangedEventArgs e);
}
