using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Flightbud.Xamarin.Forms.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            //you may create a bing maps API Key and paste into the Init() argument, and ServiceToken property below:
            global::Xamarin.FormsMaps.Init("[insert_your_api_key]");
            Windows.Services.Maps.MapService.ServiceToken = "[insert_your_api_key]";
            LoadApplication(new Flightbud.Xamarin.Forms.App());
        }
    }
}
