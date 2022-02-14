using Flightbud.Xamarin.Forms.Data.Models;
using Flightbud.Xamarin.Forms.View.Controls;
using System;
using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace Flightbud.Xamarin.Forms.View.Models
{
    /// <summary>
    /// View Model for MapPage.xaml DataContext
    /// </summary>
    public class MapPageViewModel : ViewModelBase
    {
        public AviationMap Map { get; set; }

        public double MapItemsSearchFrequency { get; set; }

        public MapSpan CurrentGeolocation { get; set; }

        public List<MapItemBase> MapItems { get; set; }

        bool _isLoading;
        public bool IsLoading 
        { 
            get { return _isLoading; }
            set { _isLoading = value; OnPropertyChanged(); }
        }

        bool _isAutoFollow = false;
        public bool IsAutoFollow
        {
            get { return _isAutoFollow; }
            set { _isAutoFollow = value; OnPropertyChanged(); }
        }

        bool _isMapPanning = false;
        public bool IsMapPanning
        {
            get { return _isMapPanning; }
            set { _isMapPanning = value; OnPropertyChanged(); }
        }

        public MapPageViewModel(AviationMap map)
        {
            Map = map;

            MapItems = new List<MapItemBase>();
            _isMapPanning = false;
            _isLoading = false;
        }
    }
}
