using Flightbud.Xamarin.Forms.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Flightbud.Xamarin.Forms
{
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();

            Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(HomePage)));
        }
    }
}
