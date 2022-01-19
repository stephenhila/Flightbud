using Flightbud.Xamarin.Forms.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;
using Map = Xamarin.Forms.Maps.Map;

namespace Flightbud.Xamarin.Forms.ViewModels
{
    public class MapPageViewModel : ViewModelBase
    {
        public Map Map { get; set; }
        public MapSpan MapSpan { get; set; }
        public Position MapCenter { get; set; }
        public double LatitudeSpanDegrees { get; set; } = 0.15;
        public double LongitudeSpanDegrees { get; set; } = 0.15;

        Location _currentLocation;
        public Location CurrentLocation
        { 
            get
            {
                return _currentLocation;
            }
            set
            {
                _currentLocation = value;
                OnPropertyChanged();
            }
        }

        public List<Airport> Airports { get; set; } = new List<Airport>();

        public MapPageViewModel(Map map, Location location)
        {
            Map = map;
            CurrentLocation = location;
        }

        public void Update()
        {
            MapCenter = new Position(_currentLocation.Latitude, _currentLocation.Longitude);
            MapSpan = new MapSpan(MapCenter, LatitudeSpanDegrees, LongitudeSpanDegrees);
            Map.MoveToRegion(MapSpan);

            // add POI's
            var pinsWithinMapSpan = 
                Airports.Where(a => a.Position.Latitude < MapCenter.Latitude + MapSpan.LatitudeDegrees
                                 && a.Position.Latitude > MapCenter.Latitude - MapSpan.LatitudeDegrees
                                 && a.Position.Longitude < MapCenter.Longitude + MapSpan.LongitudeDegrees
                                 && a.Position.Longitude > MapCenter.Longitude - MapSpan.LongitudeDegrees
                              ).Select(a => a.MapPin);

            foreach (var pin in pinsWithinMapSpan)
            {
                Map.Pins.Add(pin);
            }
        }
    }
}
