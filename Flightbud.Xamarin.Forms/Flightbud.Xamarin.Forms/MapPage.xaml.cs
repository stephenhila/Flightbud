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
            new Map { IsShowingUser = true, MapType = MapType.Satellite}, 
            null,
            10/*KMs*/);
        public MapPage()
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            viewModel.CurrentLocation = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(60)));
            viewModel.MapCenter = new Position(viewModel.CurrentLocation.Latitude, viewModel.CurrentLocation.Longitude);
            viewModel.MapSpan = MapSpan.FromCenterAndRadius(viewModel.MapCenter, Distance.FromKilometers(viewModel.MapSpanRadius));

            var airportsCsvResourceId = "Flightbud.Xamarin.Forms.Assets.world-airports.csv";
            var assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(airportsCsvResourceId);

            using (var reader = new System.IO.StreamReader(stream))
            {
                if (reader != null)
                {
                    using (var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ","}))
                    {
                        csvReader.Read();
                        csvReader.ReadHeader();
                        while (csvReader.Read())
                        {
                            var latitudeDegreeField = csvReader.GetField<double>(csvReader.GetFieldIndex("latitude_deg"));
                            var longitudeDegreeField = csvReader.GetField<double>(csvReader.GetFieldIndex("longitude_deg"));
                            if (latitudeDegreeField < viewModel.MapCenter.Latitude + viewModel.MapSpan.LatitudeDegrees
                                 && latitudeDegreeField > viewModel.MapCenter.Latitude - viewModel.MapSpan.LatitudeDegrees
                                 && longitudeDegreeField < viewModel.MapCenter.Longitude + viewModel.MapSpan.LongitudeDegrees
                                 && longitudeDegreeField > viewModel.MapCenter.Longitude - viewModel.MapSpan.LongitudeDegrees)
                            {
                                Airport airport = csvReader.GetRecord<Airport>();
                                viewModel.Airports.Add(airport);
                            }
                        }
                    }
                }
            }
            viewModel.Update();
        }
    }
}