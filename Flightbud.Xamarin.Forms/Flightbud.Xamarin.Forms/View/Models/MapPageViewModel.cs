﻿using Flightbud.Xamarin.Forms.Data.Models;
using Flightbud.Xamarin.Forms.View.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;
using Map = Xamarin.Forms.Maps.Map;

namespace Flightbud.Xamarin.Forms.View.Models
{
    /// <summary>
    /// View Model for MapPage.xaml DataContext
    /// </summary>
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

        public List<MapItemBase> MapItems { get; set; }

        public MapPageViewModel(AviationMap map, Location location, double mapSpanRadius)
        {
            Map = map;
            CurrentLocation = location;
            MapSpanRadius = mapSpanRadius;

            MapItems = new List<MapItemBase>();
        }

        public void Update()
        {
            Map.MoveToRegion(MapSpan);
        }
    }
}
