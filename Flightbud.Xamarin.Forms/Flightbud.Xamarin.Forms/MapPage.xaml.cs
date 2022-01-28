using Flightbud.Xamarin.Forms.Data;
using Flightbud.Xamarin.Forms.Data.Facade;
using Flightbud.Xamarin.Forms.Data.Models;
using Flightbud.Xamarin.Forms.View.Controls;
using Flightbud.Xamarin.Forms.View.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Flightbud.Xamarin.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        List<IMapRegionData<MapItemBase>> dataSources;
        MapPageViewModel viewModel;
        AirportDetailsPage airportDetailsPage;
        public MapPage()
        {
            InitializeComponent();
            viewModel = new MapPageViewModel(
            this.AviationMap,
            null,
            Constants.LOCATION_INITIAL_SPAN_RADIUS);
            BindingContext = viewModel;

            dataSources = new List<IMapRegionData<MapItemBase>>
            {
                new AirportData(),
                new NavaidData()
            };

            BeginCurrentLocationUpdate();
        }

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Handles the event where Visible Region changes in the AviationMap.
        /// 
        /// Implements cancellation logic, wherein if more Visible Region changes happen, 
        /// the previous attempt is cancelled as the new position takes new priority.
        /// 
        /// This is to optimize and prevent multiple instances of the app trying to
        /// pull data from the data sources for each minute change in the map visible region.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task VisibleRegionChangedEventHandler(Object sender, VisibleRegionChangedEventArgs e)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken ct = cancellationTokenSource.Token;

            try
            {
                await Task.Delay(Constants.LOCATION_UPDATE_DELAY_MILLISECONDS, ct);
                viewModel.IsLoading = true;
                List<MapItemBase> mapItemsInRange = new List<MapItemBase>();
                foreach (var dataSource in dataSources)
                {
                    mapItemsInRange.AddRange(await Task.Run(() => dataSource.Get(viewModel.Map.VisibleRegion.Center, viewModel.Map.VisibleRegion.Radius.Kilometers), ct));
                }
                
                foreach (var mapItem in mapItemsInRange)
                {
                    if (!viewModel.MapItems.Exists(a => a.Name == mapItem.Name))
                    {
                        viewModel.MapItems.Add(mapItem);
                    }
                }
            }
            catch (OperationCanceledException oce)
            {
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                viewModel.IsLoading = false;
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

        private async Task MapItemDetailsRequestedEventHandler(object sender, MapItemDetailsRequestedEventArgs e)
        {
            if (e.SelectedMapItem is Airport)
            {
                if (airportDetailsPage == null)
                {
                    airportDetailsPage = new AirportDetailsPage();
                }
                airportDetailsPage.ViewModel.SelectedAirport = e.SelectedMapItem as Airport;
                await Navigation.PushModalAsync(airportDetailsPage);
            }
        }
    }
}