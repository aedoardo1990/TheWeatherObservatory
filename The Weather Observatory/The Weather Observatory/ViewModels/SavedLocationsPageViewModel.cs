using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using The_Weather_Observatory.Services;
using Xamarin.Forms;
using The_Weather_Observatory.Models;

namespace The_Weather_Observatory.ViewModels
{
    public class SavedLocationsPageViewModel : BindableObject
    {
        private readonly LocationService _locationService;
        private ObservableCollection<SaveLocation> _savedLocations;

        public ObservableCollection<SaveLocation> SavedLocations
        {
            get => _savedLocations;
            set
            {
                _savedLocations = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }

        public SavedLocationsPageViewModel(LocationService locationService)
        {
            _locationService = locationService;
            SavedLocations = new ObservableCollection<SaveLocation>();
            RefreshCommand = new Command(async () => await LoadLocationsAsync());
        }

        public async Task LoadLocationsAsync()
        {
            var savedLocations = await _locationService.GetLocationsAsync();
            SavedLocations.Clear();
            foreach (var savedLocation in savedLocations)
            {
                SavedLocations.Add(savedLocation);
            }
        }
    }
}
