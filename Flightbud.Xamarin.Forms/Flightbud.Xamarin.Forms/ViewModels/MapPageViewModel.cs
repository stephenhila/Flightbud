using System;
using System.Collections.Generic;
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

        public MapPageViewModel(Map map, Location location)
        {
            Map = map;
            CurrentLocation = location;
        }

        public void Update()
        {
            MapCenter = new Position(_currentLocation.Latitude, _currentLocation.Longitude);
            MapSpan = new MapSpan(MapCenter, 0.01, 0.01);
            Map.MoveToRegion(MapSpan);
        }
    }
}
