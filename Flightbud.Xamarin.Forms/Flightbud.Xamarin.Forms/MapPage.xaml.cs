using Flightbud.Xamarin.Forms.Data;
using Flightbud.Xamarin.Forms.Data.Facade;
using Flightbud.Xamarin.Forms.Data.Models;
using Flightbud.Xamarin.Forms.View.Controls;
using Flightbud.Xamarin.Forms.View.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            viewModel = new MapPageViewModel(this.AviationMap);
            viewModel.MapItemsSearchFrequency = Constants.MAP_ITEMS_SEARCH_FREQUENCY;
            BindingContext = viewModel;

            dataSources = new List<IMapRegionData<MapItemBase>>
            {
                new AirportDataSylvanDataSource(),
                new NavaidDataSylvanDataSource()
            };

            Device.StartTimer(TimeSpan.FromMilliseconds(Constants.LOCATION_UPDATE_FREQUENCY_MILLISECONDS), () => BeginCurrentLocationUpdate());
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
        private async void VisibleRegionChangedEventHandler(Object sender, VisibleRegionChangedEventArgs e)
        {
            CancellationToken ct = cancellationTokenSource.Token;

            try
            {
                viewModel.IsMapPanning = true;
                viewModel.IsLoading = true;

                double currentRadius = viewModel.Map.VisibleRegion.Radius.Kilometers;
                if (Constants.LOCATION_ITEMS_REGION_SPAN_RADIUS_THRESHOLD < currentRadius)
                    return;

                Position currentPosition = viewModel.Map.VisibleRegion.Center;
                viewModel.IsLoading = true;

                viewModel.LastMapPanned = DateTime.UtcNow;

                IEnumerable<Task<List<MapItemBase>>> mapDataTasksQuery =
                from dataSource in dataSources
                select GetMapData(dataSource, ct);

                List<Task<List<MapItemBase>>> listOfTasks = mapDataTasksQuery.ToList();

                while (listOfTasks.Any())
                {
                    Task<List<MapItemBase>> finishedTask = await Task.WhenAny(listOfTasks);
                    listOfTasks.Remove(finishedTask);
                    var mapItemsInRange = await finishedTask;
                    foreach (var mapItem in mapItemsInRange)
                    {
                        lock (viewModel.MapItems)
                        {
                            if (!viewModel.MapItems.Exists(a => a.Name == mapItem.Name))
                            {
                                viewModel.MapItems.Add(mapItem);
                            }
                        }
                    }
                    await Task.Delay(Constants.LOCATION_UPDATE_DELAY_MILLISECONDS);
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
                viewModel.IsMapPanning = false;
                viewModel.IsLoading = false;
            }
        }

        private async Task<List<MapItemBase>> GetMapData(IMapRegionData<MapItemBase> dataSource, CancellationToken ct)
        {
            return await dataSource.Get(viewModel.Map.VisibleRegion, ct);
        }

        private bool BeginCurrentLocationUpdate()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var currentLocation = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(60)));
                viewModel.CurrentGeolocation = MapSpan.FromCenterAndRadius(new Position(currentLocation.Latitude, currentLocation.Longitude), Distance.FromKilometers(Constants.LOCATION_INITIAL_SPAN_RADIUS));

                if (viewModel.LastMapPanned.AddMilliseconds(Constants.LOCATION_UPDATE_RESUME_MILLISECONDS) > DateTime.UtcNow)
                {
                    viewModel.Map.MoveToRegion(viewModel.CurrentGeolocation);
                }
            });

            return false;
        }

        private async Task MapItemDetailsRequestedEventHandler(object sender, MapItemDetailsRequestedEventArgs e)
        {
            if (e.SelectedMapItem is Airport)
            {
                if (airportDetailsPage == null)
                {
                    airportDetailsPage = new AirportDetailsPage();
                }
                Airport selectedAirport = e.SelectedMapItem as Airport;
                await selectedAirport.LoadDetails();
                airportDetailsPage.ViewModel.SelectedAirport = e.SelectedMapItem as Airport;
                await Navigation.PushModalAsync(airportDetailsPage);
            }
        }
    }
}