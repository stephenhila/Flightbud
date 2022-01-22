using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.View.Models
{
    public abstract class BaseAviationPin : Pin
    {
        public abstract string Image { get; }
    }
}
