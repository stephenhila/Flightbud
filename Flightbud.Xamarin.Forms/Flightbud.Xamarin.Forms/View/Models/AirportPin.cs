﻿namespace Flightbud.Xamarin.Forms.View.Models
{
    public class AirportPin : BaseAviationPin
    {
        public string Name { get; set; }

        public override string Image => "icon_map_airport.png";
    }
}
