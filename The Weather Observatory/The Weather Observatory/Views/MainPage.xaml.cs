using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Weather_Observatory.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace The_Weather_Observatory
{
    public partial class MainPage : ContentPage
    {
        // private MainPageViewModel _viewModel;
        public MainPage()
        {
            InitializeComponent();
            //_viewModel = new MainPageViewModel();
            this.BindingContext = new MainPageViewModel();
        }

        //async void Button_Clicked(System.Object sender, System.EventArgs e)
        //{
        //    await _viewModel.GetCurrentLocationWeather();
        //}
    }
}
