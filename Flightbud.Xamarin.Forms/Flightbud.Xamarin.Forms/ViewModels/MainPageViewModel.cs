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

        public MainPageViewModel()
        {
            IconApp = "icon_app.png";
            MenuItems = new ObservableCollection<MenuItem>(new[]
                {
                    new MenuItem { Text = "Home", IconImageSource=@"icon_home.png" },
                    new MenuItem { Text = "Map", IconImageSource=@"icon_map.png" },
                    new MenuItem { Text = "About", IconImageSource=@"icon_home.png" },
                });

            OnPropertyChanged("IconApp");
            OnPropertyChanged("MenuItems");
        }
    }
}
