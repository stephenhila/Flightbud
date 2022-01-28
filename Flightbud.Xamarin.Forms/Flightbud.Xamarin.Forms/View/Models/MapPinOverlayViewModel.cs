using Flightbud.Xamarin.Forms.Data.Models;

namespace Flightbud.Xamarin.Forms.View.Models
{
    public class MapPinOverlayViewModel : ViewModelBase
    {
        MapItemBase _selectedMapItem;
        public MapItemBase SelectedMapItem
        {
            get { return _selectedMapItem; }
            set
            {
                _selectedMapItem = value;
                OnPropertyChanged();
            }
        }
    }
}
