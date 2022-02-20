using System;
using System.Collections.Generic;
using System.Text;

namespace Flightbud.Xamarin.Forms.View.Models
{
    public class LocationPin : BaseAviationPin
    {
        public override string Image => "icon_location_pointer.png";

        public string Model3DPath = "icon_location_pointer.3mf";
        public double Model3DRotationOffset = 270;
    }
}
