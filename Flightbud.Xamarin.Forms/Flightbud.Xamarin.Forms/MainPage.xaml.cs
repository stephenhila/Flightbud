﻿using Flightbud.Xamarin.Forms.View.Models;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Flightbud.Xamarin.Forms
{
    public partial class MainPage : FlyoutPage
    {
        public MainPage()
        {
            InitializeComponent();
            List<MenuItem> menuItems = new List<MenuItem>
                {
                    new FlyoutMenuItem { Text = "Home", IconImageSource=@"icon_home.png", TargetType = typeof(HomePage) },
                    new FlyoutMenuItem { Text = "Map", IconImageSource=@"icon_map.png", TargetType = typeof(MapPage) },
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
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
                //(sender as ListView).SelectedItem = null;
                IsPresented = false;
            }
        }
    }
}
