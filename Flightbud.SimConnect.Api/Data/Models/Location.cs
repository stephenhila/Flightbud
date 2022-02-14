namespace Flightbud.SimConnect.Api.Data.Models
{
    public struct Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
        public double? Course { get; set; }
        public double? Speed { get; set; }
    }
}
