using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace Flightbud.Xamarin.Forms.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public string Title { get; set; } = "Flightbud";
        public string IconApp { get; set; } = "icon_app.png";
        public ObservableCollection<MenuItem> MenuItems { get; set; }

        public MainPageViewModel(List<MenuItem> menuItems)
        {
            MenuItems = new ObservableCollection<MenuItem>(menuItems);
        }
    }
}
