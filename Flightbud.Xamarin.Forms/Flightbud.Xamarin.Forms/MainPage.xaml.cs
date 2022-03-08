using Flightbud.Xamarin.Forms.View.Controls;
using Flightbud.Xamarin.Forms.View.Models;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Flightbud.Xamarin.Forms
{
    public partial class MainPage : FlyoutPage
    {
        List<PausableContentPage> _loadedPages = new List<PausableContentPage>();
        public MainPage()
        {
            InitializeComponent();
            List<MenuItem> menuItems = new List<MenuItem>
                {
                    new FlyoutMenuItem { Text = "Home", IconImageSource=@"icon_home.png", TargetType = typeof(HomePage) },
                    new FlyoutMenuItem { Text = "Map", IconImageSource=@"icon_map.png", TargetType = typeof(MapPage) },
                    new FlyoutMenuItem { Text = "Settings", IconImageSource=@"icon_settings.png", TargetType = typeof(SettingsPage) },
                    new FlyoutMenuItem { Text = "About", IconImageSource=@"icon_about.png", TargetType = typeof(AboutPage) },
                };
            BindingContext = new MainPageViewModel(menuItems);

            Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(HomePage)));
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as FlyoutMenuItem;
            if (item != null)
            {
                ((Detail as NavigationPage)?.CurrentPage as PausableContentPage)?.Pause();

                Page requestedPage = _loadedPages.Find(x => x.GetType() == item.TargetType);

                if (requestedPage == null)
                {
                    requestedPage = Activator.CreateInstance(item.TargetType) as Page;
                    if (requestedPage is PausableContentPage)
                    {
                        _loadedPages.Add(requestedPage as PausableContentPage);
                    }
                }
                else
                {
                    (requestedPage as PausableContentPage)?.Resume();
                }
                Detail = new NavigationPage(requestedPage);
                IsPresented = false;
            }
        }
    }
}
