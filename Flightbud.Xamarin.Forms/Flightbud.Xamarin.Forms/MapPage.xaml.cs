using CsvHelper;
using CsvHelper.Configuration;
using Flightbud.Xamarin.Forms.Data;
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
using System.Threading;
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
        IMapRegionData<MapItemBase> airportData;

        MapPageViewModel viewModel;
        public MapPage()
        {
            InitializeComponent();
            viewModel = new MapPageViewModel(
            this.AviationMap,
            null,
            10/*KMs*/);
            BindingContext = viewModel;

            airportData = new AirportData();

            //Device.StartTimer(TimeSpan.FromSeconds(15), CurrentLocationUpdate_Tick);
            CurrentLocationUpdate_Tick();
        }

        private bool CurrentLocationUpdate_Tick()
        {
            BeginCurrentLocationUpdate();
            return true;
        }

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private async void VisibleRegionChangedEventHandler(Object sender, VisibleRegionChangedEventArgs e)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken ct = cancellationTokenSource.Token;

            try
            {
                await Task.Delay(Constants.LOCATION_UPDATE_DELAY_MILLISECONDS, ct);
                List<MapItemBase> airportsInRange = await Task.Run(() => airportData.Get(viewModel.Map.VisibleRegion.Center, viewModel.Map.VisibleRegion.Radius.Kilometers), ct);

                foreach (var airport in airportsInRange)
                {
                    if (!viewModel.MapItems.Exists(a => a.Name == airport.Name))
                    {
                        viewModel.MapItems.Add(airport);
                    }
                }
            }
            catch (OperationCanceledException oce)
            {
                // if you use your two eyes you would notice that nothing is being done in this catch block..
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }

        private async void BeginCurrentLocationUpdate()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                viewModel.CurrentLocation = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(60)));
                viewModel.MapCenter = new Position(viewModel.CurrentLocation.Latitude, viewModel.CurrentLocation.Longitude);
                viewModel.MapSpan = MapSpan.FromCenterAndRadius(viewModel.MapCenter, Distance.FromKilometers(viewModel.MapSpanRadius));

                viewModel.Map.MoveToRegion(viewModel.MapSpan);
            });
        }
    }
}