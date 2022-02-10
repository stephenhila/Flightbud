using Flightbud.Xamarin.Forms.Data;
using Flightbud.Xamarin.Forms.Data.Facade;
using Flightbud.Xamarin.Forms.Data.Models;
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
        List<IMapRegionData<MapItemBase>> dataSources;
        MapPageViewModel viewModel;
        AirportDetailsPage airportDetailsPage;

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        Stopwatch _stopwatch;

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

            LoadLocation();

            Device.StartTimer(TimeSpan.FromMilliseconds(Constants.AUTO_FOLLOW_FREQUENCY_MILLISECONDS), 
                () =>
                {
                    Task.Run(async () =>
                    {
                        if (viewModel.IsAutoFollow)
                        {
                            Device.BeginInvokeOnMainThread(async () => await UpdateLocation());
                            // do something with time...
                        }
                    });

                    return true;
                });

            _stopwatch = new Stopwatch();
        }

        private void LoadLocation()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await UpdateLocation();
                await VisibleRegionChangedEventHandler(this, new VisibleRegionChangedEventArgs { VisibleRegion = viewModel.CurrentGeolocation });

                _stopwatch.Start();
            });
        }

        private async Task<bool> UpdateLocation()
        {
            var currentLocation = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(60)));
            viewModel.CurrentGeolocation = MapSpan.FromCenterAndRadius(new Position(currentLocation.Latitude, currentLocation.Longitude), Distance.FromKilometers(Constants.LOCATION_INITIAL_SPAN_RADIUS));
            viewModel.Map.MoveToRegion(viewModel.CurrentGeolocation);

            // TODO: implement error-handling later
            return true;
        }

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
            CancellationToken ct = cancellationTokenSource.Token;

            try
            {
                viewModel.IsMapPanning = true;
                viewModel.IsLoading = true;

                if (Constants.LOCATION_ITEMS_REGION_SPAN_RADIUS_THRESHOLD < e.VisibleRegion.Radius.Kilometers)
                    return;

                viewModel.IsLoading = true;

                IEnumerable<Task<List<MapItemBase>>> mapDataTasksQuery =
                from dataSource in dataSources
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