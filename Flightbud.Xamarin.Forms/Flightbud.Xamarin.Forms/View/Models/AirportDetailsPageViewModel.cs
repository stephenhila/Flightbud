using Flightbud.Xamarin.Forms.Data.Models;

namespace Flightbud.Xamarin.Forms.View.Models
{
    public class AirportDetailsPageViewModel : ViewModelBase
    {
        Airport _selectedAirport;
        public Airport SelectedAirport 
        {
            get
            {
                return _selectedAirport;
            }
            set
            {
                _selectedAirport = value;
                OnPropertyChanged();
            }
        }
    }
}
