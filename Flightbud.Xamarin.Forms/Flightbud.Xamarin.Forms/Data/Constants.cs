namespace Flightbud.Xamarin.Forms.Data
{
    public static class Constants
    {
        //DATA SOURCES
        public const string AIRPORT_DATA_RESOURCE = "Flightbud.Xamarin.Forms.Assets.airports.csv";
        public const string AIRPORT_FREQUENCY_DATA_RESOURCE = "Flightbud.Xamarin.Forms.Assets.airport-frequencies.csv";
        public const string NAVAID_DATA_RESOURCE = "Flightbud.Xamarin.Forms.Assets.navaids.csv";
        public const string RUNWAY_DATA_RESOURCE = "Flightbud.Xamarin.Forms.Assets.runways.csv";

        //DEFAULT VALUES
        public const string NO_DATA_AVAILABLE = "no data";
        public const string NO_DATA_SUPPORT = "data not supported";

        //LIMITS
        public const int LOCATION_TIMEOUT = 10000;
        public const double LOCATION_INITIAL_SPAN_RADIUS = 11.1;
        public const double LOCATION_ITEMS_REGION_SPAN_RADIUS_THRESHOLD = 100;

        //TIMEOUTS
        public const int AUTO_FOLLOW_RESUME_MILLISECONDS = 10000;
        public const int AUTO_FOLLOW_FREQUENCY_MILLISECONDS = 3333;
        public const int ELAPSED_TIME_LOAD_LOCATIONS_MILISECONDS = 1000;
        public const int ELAPSED_TIME_SCHEDULER_FREQUENCY_MILISECONDS = 500;
    }
}
