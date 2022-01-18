using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace Flightbud.Xamarin.Forms.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public string IconApp { get; set; }
        public ObservableCollection<MenuItem> MenuItems { get; set; }

        public MainPageViewModel(List<MenuItem> menuItems)
        {
            IconApp = "icon_app.png";
            MenuItems = new ObservableCollection<MenuItem>(menuItems);

            OnPropertyChanged("IconApp");
            OnPropertyChanged("MenuItems");
        }
    }
}
