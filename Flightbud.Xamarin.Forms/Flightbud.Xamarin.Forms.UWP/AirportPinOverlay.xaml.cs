using Flightbud.Xamarin.Forms.View.Models;
using System;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Flightbud.Xamarin.Forms.UWP
{
    public sealed partial class AirportPinOverlay : UserControl
    {
        AirportPin _pin;
        public AirportPinOverlay(AirportPin pin)
        {
            this.InitializeComponent();
            _pin = pin;
            SetupData();
        }

        void SetupData()
        {
            Label.Text = _pin.Label;
            Address.Text = _pin.Address;
        }

        private async void OnInfoButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            //await Launcher.LaunchUriAsync(new Uri(customPin.Url));
        }
    }
}
