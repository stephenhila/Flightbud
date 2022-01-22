using CsvHelper;
using CsvHelper.Configuration;
using Flightbud.Xamarin.Forms.Data.Facade;
using Flightbud.Xamarin.Forms.Data.Models;
using Flightbud.Xamarin.Forms.View.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using Map = Xamarin.Forms.Maps.Map;

namespace Flightbud.Xamarin.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        IMapRegionData<Airport> airportData;

        MapPageViewModel viewModel = new MapPageViewModel(
            new AviationMap { IsShowingUser = true, MapType = MapType.Satellite}, 
            null,
            10/*KMs*/);
        public MapPage()
        {
            InitializeComponent();
            BindingContext = viewModel;

            airportData = new AirportData();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            viewModel.CurrentLocation = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(60)));
            viewModel.MapCenter = new Position(viewModel.CurrentLocation.Latitude, viewModel.CurrentLocation.Longitude);
            viewModel.MapSpan = MapSpan.FromCenterAndRadius(viewModel.MapCenter, Distance.FromKilometers(viewModel.MapSpanRadius));

            viewModel.Airports = airportData.Get(viewModel.MapCenter, viewModel.MapSpanRadius);

            viewModel.Update();
        }
    }
}