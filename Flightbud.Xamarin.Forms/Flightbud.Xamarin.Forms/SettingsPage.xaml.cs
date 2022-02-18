using Flightbud.Xamarin.Forms.View.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Flightbud.Xamarin.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        SettingsPageViewModel viewModel;

        public SettingsPage()
        {
            InitializeComponent();

            viewModel = new SettingsPageViewModel();
            BindingContext = viewModel;
        }
    }
}