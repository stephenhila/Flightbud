using Flightbud.Xamarin.Forms.Data;
using Flightbud.Xamarin.Forms.Data.Facade;
using Flightbud.Xamarin.Forms.Data.Models;
using Flightbud.Xamarin.Forms.Utils;
using Flightbud.Xamarin.Forms.View.Controls;
using Flightbud.Xamarin.Forms.View.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        List<IMapRegionData<MapItemBase>> regionDataSources;
        ILocationData locationDataSource;
        MapPageViewModel viewModel;
        AirportDetailsPage airportDetailsPage;

        CancellationTokenSource regionDataCancellationTokenSource = new CancellationTokenSource();
        CancellationTokenSource locationDataCancellationTokenSource = new CancellationTokenSource();

        TaskElapsedTimeScheduler locationUpdatesScheduler;

        public MapPage()
        {
            InitializeComponent();
            viewModel = new MapPageViewModel(this.AviationMap);
            BindingContext = viewModel;

            regionDataSources = new List<IMapRegionData<MapItemBase>>
            {
                new AirportDataSylvanDataSource(),
                new NavaidDataSylvanDataSource()
            };
            locationDataSource = LocationDataSourceFactory.Create();

            LoadLocation();

            locationUpdatesScheduler = new TaskElapsedTimeScheduler(
                async () =>
                {
                    if (viewModel.IsAutoFollow)
                    {
                        await UpdateLocation();
                    }
                }
                , Constants.AUTO_FOLLOW_FREQUENCY_MILLISECONDS
                , TaskElapsedTimeSchedulerBehavior.Recurring);
            locationUpdatesScheduler.Start();
        }

        private void LoadLocation()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await UpdateLocation();
            });
        }

        private async Task<bool> UpdateLocation()
        {
            try
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var currentLocation = await locationDataSource.Get(GeolocationAccuracy.High, Constants.LOCATION_TIMEOUT, locationDataCancellationTokenSource.Token);
                    if (currentLocation != null)
                    {
                        lock (viewModel.Map.VisibleRegion)
                        {
                            viewModel.CurrentGeolocation = MapSpan.FromCenterAndRadius(new Position(currentLocation.Latitude, currentLocation.Longitude), viewModel.Map.VisibleRegion.Radius);
                        }
                        viewModel.Map.MoveToRegion(viewModel.CurrentGeolocation);
                    }
                });
            }
            catch (OperationCanceledException oce)
            {
                viewModel.IsAutoFollow = false;
                locationDataCancellationTokenSource = new CancellationTokenSource();
            }

            return true;
        }

        /// <summary>
        /// Handles the Map Panning event, wherein the map starts to be panned by the user.
        /// 
        /// When the panning starts, we need to be cancelling retrieval of region data since 
        /// there would be a new region shown after the panning.
        /// 
        /// We also need location update to be cancelled to avoid going back to your location
        /// to avoid interrupting the panning process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task MapPanningEventHandler(Object sender, MapPanningEventArgs e)
        {
            regionDataCancellationTokenSource.Cancel();
            regionDataCancellationTokenSource = new CancellationTokenSource();
            if (viewModel.IsAutoFollow)
            {
                locationDataCancellationTokenSource.Cancel();
                locationDataCancellationTokenSource = new CancellationTokenSource();
            }
        }

        /// <summary>
        /// Handles the event where Visible Region changes in the AviationMap, after panning
        /// is halted.
        /// 
        /// When region changes, map region data gets loaded into the new visible region.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task VisibleRegionChangedEventHandler(Object sender, VisibleRegionChangedEventArgs e)
        {
            CancellationToken ct = regionDataCancellationTokenSource.Token;

            try
            {
                viewModel.IsMapPanning = true;
                viewModel.IsLoading = true;

                if (Constants.LOCATION_ITEMS_REGION_SPAN_RADIUS_THRESHOLD < e.VisibleRegion.Radius.Kilometers)
                    return;

                viewModel.IsLoading = true;

                IEnumerable<Task<List<MapItemBase>>> mapDataTasksQuery =
                from dataSource in regionDataSources
                select GetMapData(e.VisibleRegion, dataSource, ct);

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

        private async Task<List<MapItemBase>> GetMapData(MapSpan mapSpan, IMapRegionData<MapItemBase> dataSource, CancellationToken ct)
        {
            return await dataSource.Get(mapSpan, ct);
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