using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirMonitor.Models;
using AirMonitor.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AirMonitor.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            BindingContext = new HomePageViewModel(Navigation);
        }

        void ListView_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            (BindingContext as HomePageViewModel)?.GoToDetailsCommand.Execute(e.Item as Measurement);
        }
    }
}