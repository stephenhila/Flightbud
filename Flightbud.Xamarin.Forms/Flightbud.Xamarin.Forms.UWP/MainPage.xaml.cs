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
            global::Xamarin.FormsMaps.Init("rDFzuoHYGIOMksDDmgAF~g0AFGzuC_TlPuU8bVsTFew~AqLP1dKdQ3FsVPqLflh84t7qX9R_oKU--qM_FlEHaQHo_QqM-TbiaWJknQ1N3QQG");
            Windows.Services.Maps.MapService.ServiceToken = "rDFzuoHYGIOMksDDmgAF~g0AFGzuC_TlPuU8bVsTFew~AqLP1dKdQ3FsVPqLflh84t7qX9R_oKU--qM_FlEHaQHo_QqM-TbiaWJknQ1N3QQG";
            LoadApplication(new Flightbud.Xamarin.Forms.App());
        }
    }
}
