using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirMonitor.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace AirMonitor.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();
            BindingContext = new HomePageViewModel(Navigation);
        }

        private void Pin_OnInfoWindowClicked(object sender, PinClickedEventArgs e)
        {
            (BindingContext as HomePageViewModel)?.InfoWindowClickedCommand.Execute((sender as Xamarin.Forms.Maps.Pin).Address);
        }
    }
}