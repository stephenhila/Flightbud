using System;
using System.Collections.Generic;
using System.Text;

namespace Flightbud.Xamarin.Forms.Data
{
    public static class Constants
    {
        //DATA SOURCES
        public const string AIRPORT_DATA_RESOURCE = "Flightbud.Xamarin.Forms.Assets.airports.csv";
        public const string AIRPORT_FREQUENCY_DATA_RESOURCE = "Flightbud.Xamarin.Forms.Assets.airport-frequencies.csv";
        public const string RUNWAY_DATA_RESOURCE = "Flightbud.Xamarin.Forms.Assets.runways.csv";

        //DEFAULT VALUES
        public const string NO_DATA_AVAILABLE = "no data";

        //LIMITS
        public const double LOCATION_INITIAL_SPAN_RADIUS = 10;
        public const double LOCATION_ITEMS_REGION_SPAN_RADIUS_THRESHOLD = 100;

        //TIMEOUTS
        public const int LOCATION_UPDATE_DELAY_MILLISECONDS = 750;
    }
}
