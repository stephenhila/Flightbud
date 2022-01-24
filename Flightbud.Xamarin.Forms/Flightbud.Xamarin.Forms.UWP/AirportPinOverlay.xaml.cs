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
        public MapItemOverlayViewModel ViewModel { get; set; }
        public AirportPinOverlay(MapItemOverlayViewModel viewModel)
        {
            this.InitializeComponent();
            ViewModel = viewModel;
            DataContext = ViewModel;
        }

        private async void OnInfoButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            //await Launcher.LaunchUriAsync(new Uri(customPin.Url));
        }
    }
}
