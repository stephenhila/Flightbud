using Flightbud.Xamarin.Forms.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flightbud.Xamarin.Forms.View.Models
{
    public class AirportPinOverlayViewModel : ViewModelBase
    {
        Airport selectedAirport;
        public Airport SelectedAirport
        {
            get { return selectedAirport; }
            set
            {
                selectedAirport = value;
                OnPropertyChanged();
            }
        }
    }
}
