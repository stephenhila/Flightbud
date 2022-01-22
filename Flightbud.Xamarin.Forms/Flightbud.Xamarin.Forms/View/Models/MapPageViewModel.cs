using Flightbud.Xamarin.Forms.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;
using Map = Xamarin.Forms.Maps.Map;

namespace Flightbud.Xamarin.Forms.View.Models
{
    public class MapPageViewModel : ViewModelBase
    {
        public AviationMap Map { get; set; }
        public MapSpan MapSpan { get; set; }
        public Position MapCenter { get; set; }
        public double MapSpanRadius { get; set; }

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

        public MapPageViewModel(AviationMap map, Location location, double mapSpanRadius)
        {
            Map = map;
            CurrentLocation = location;
            MapSpanRadius = mapSpanRadius;
        }

        public void Update()
        {
            Map.MoveToRegion(MapSpan);
            Map.AirportPins.Clear();
            Map.Pins.Clear();

            // add POI's
            Airports.ForEach(a => Map.AirportPins.Add(a.MapPin));
            //Airports.ForEach(a => Map.Pins.Add(a.MapPin));
        }
    }
}
