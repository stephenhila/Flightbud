using System;
using System.Collections.Generic;
using System.Text;

namespace Flightbud.Xamarin.Forms.Data
{
    public static class Constants
    {
        //DATA SOURCES
        public const string AIRPORT_DATA_RESOURCE = "Flightbud.Xamarin.Forms.Assets.airports.csv";

        //LIMITS
        public const double LOCATION_ITEMS_REGION_SPAN_RADIUS_THRESHOLD = 500;

        //TIMEOUTS
        public const int LOCATION_UPDATE_DELAY_MILLISECONDS = 750;
    }
}
