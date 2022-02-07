using Flightbud.Xamarin.Forms.View.Models;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Flightbud.Xamarin.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AirportDetailsPage : ContentPage
    {
        public AirportDetailsPageViewModel ViewModel { get; set; }
        public AirportDetailsPage()
        {
            InitializeComponent();
            ViewModel = new AirportDetailsPageViewModel();
            BindingContext = ViewModel;
        }

        private void BackButtonClicked(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }
    }
}