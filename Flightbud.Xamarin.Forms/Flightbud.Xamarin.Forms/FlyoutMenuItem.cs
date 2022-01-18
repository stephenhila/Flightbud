using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Flightbud.Xamarin.Forms
{
    public class FlyoutMenuItem : MenuItem
    {
        public Type TargetType { get; set; }
    }
}
