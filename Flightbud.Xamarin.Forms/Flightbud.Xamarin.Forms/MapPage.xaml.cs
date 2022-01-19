using CsvHelper;
using CsvHelper.Configuration;
using Flightbud.Xamarin.Forms.DataModels;
using Flightbud.Xamarin.Forms.ViewModels;
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
        MapPageViewModel viewModel = new MapPageViewModel(
            new Map 
                { 
                    IsShowingUser = true, 
                    MapType = MapType.Satellite
                }, null);
        public MapPage()
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            viewModel.CurrentLocation = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(60)));

            var assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("Flightbud.Xamarin.Forms.Assets.world-airports.csv");

            using (var reader = new System.IO.StreamReader(stream))
            {
                if (reader != null)
                {
                    using (var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ","}))
                    {
                        while (csvReader.Read())
                        {
                            Airport airport = csvReader.GetRecord<Airport>();
                            viewModel.Airports.Add(airport);
                        }
                    }
                }
            }
            viewModel.Update();
        }
    }
}