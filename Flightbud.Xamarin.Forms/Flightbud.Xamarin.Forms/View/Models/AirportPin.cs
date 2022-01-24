using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.View.Models
{
    public class AirportPin : BaseAviationPin
    {
        public string Name { get; set; }

        public override string Image => "icon_pin_solidblue.png";
    }
}
