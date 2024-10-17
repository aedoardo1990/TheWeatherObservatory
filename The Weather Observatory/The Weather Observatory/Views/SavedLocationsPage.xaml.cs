using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using The_Weather_Observatory.Services;
using The_Weather_Observatory.ViewModels;
using The_Weather_Observatory.Models;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace The_Weather_Observatory
{
    public partial class SavedLocationsPage : ContentPage
    {

        public SavedLocationsPage()
        {
            InitializeComponent();
            BindingContext = new SavedLocationsPageViewModel(new LocationService());
        }

    }
}
