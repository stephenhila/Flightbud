using Xamarin.Essentials;

namespace Flightbud.Xamarin.Forms.View.Models
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public bool EnableSimConnect
        { 
            get
            {
                return Preferences.Get(nameof(EnableSimConnect), false);
            }
            set
            {
                Preferences.Set(nameof(EnableSimConnect), value);
                OnPropertyChanged();
            }
        }
    }
}
