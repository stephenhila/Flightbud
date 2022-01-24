using Flightbud.Xamarin.Forms.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flightbud.Xamarin.Forms.View.Models
{
    public class MapItemOverlayViewModel : ViewModelBase
    {
        MapItemBase selectedMapItem;
        public MapItemBase SelectedMapItem
        {
            get { return selectedMapItem; }
            set
            {
                selectedMapItem = value;
                OnPropertyChanged();
            }
        }
    }
}
