using Flightbud.Xamarin.Forms.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.View.Models
{
    public class AviationMap : Map
    {
        public List<AirportPin> AirportPins
        {
            get
            {
                return ItemsSource.OfType<Airport>()?.Select(a => a.MapPin).ToList();
            }
        }
        public AviationMap()
        {
            //AirportPins = new List<AirportPin>();
        }
    }
}
