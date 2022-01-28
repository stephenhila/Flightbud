using Flightbud.Xamarin.Forms.View.Models;
using Windows.UI.Xaml.Controls;

namespace Flightbud.Xamarin.Forms.UWP
{
    public sealed partial class NavaidPinOverlay : UserControl
    {
        public MapPinOverlayViewModel ViewModel { get; set; }
        MapPageViewModel _mapViewModel;

        public NavaidPinOverlay(MapPinOverlayViewModel overlayViewModel, MapPageViewModel mapViewModel)
        {
            this.InitializeComponent();
            ViewModel = overlayViewModel;
            _mapViewModel = mapViewModel;
            DataContext = ViewModel;
        }

        private async void OnMoreInfoClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await _mapViewModel.Map.OnMapItemDetailsRequested(new View.Controls.MapItemDetailsRequestedEventArgs { SelectedMapItem = ViewModel.SelectedMapItem });
        }
    }
}
