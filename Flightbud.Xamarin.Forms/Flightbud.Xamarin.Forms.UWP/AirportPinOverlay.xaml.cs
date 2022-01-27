using Flightbud.Xamarin.Forms.Data.Models;
using Flightbud.Xamarin.Forms.View.Models;
using System;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Flightbud.Xamarin.Forms.UWP
{
    /// <summary>
    /// Overlay to display airport details on UWP platform.
    /// </summary>
    public sealed partial class AirportPinOverlay : UserControl
    {
        public AirportPinOverlayViewModel ViewModel { get; set; }
        MapPageViewModel _mapViewModel;

        public AirportPinOverlay(AirportPinOverlayViewModel overlayViewModel, MapPageViewModel mapViewModel)
        {
            this.InitializeComponent();
            ViewModel = overlayViewModel;
            _mapViewModel = mapViewModel;
            DataContext = ViewModel;
        }

        private async void OnMoreInfoClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await _mapViewModel.Map.OnMapItemDetailsRequested(new View.Controls.MapItemDetailsRequestedEventArgs { SelectedMapItem = ViewModel.SelectedAirport });
        }
    }
}
