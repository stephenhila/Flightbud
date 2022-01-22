using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.View.Models
{
    public class AviationMap : Map
    {
        public List<AirportPin> AirportPins { get; set; }

        public AviationMap()
        {
            AirportPins = new List<AirportPin>();
        }
    }
}
