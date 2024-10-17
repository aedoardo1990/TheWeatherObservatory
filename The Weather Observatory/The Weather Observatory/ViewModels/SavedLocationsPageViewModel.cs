using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using The_Weather_Observatory.Services;
using The_Weather_Observatory.Models;
using Xamarin.Forms;

namespace The_Weather_Observatory.ViewModels
{
    public class SavedLocationsPageViewModel : BindableObject
    {
        private readonly LocationService _locationService;

        // Observable collection to hold saved locations
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

        // Command to refresh the locations
        public ICommand RefreshCommand { get; }

        public SavedLocationsPageViewModel(LocationService locationService)
        {
            _locationService = locationService;
            SavedLocations = new ObservableCollection<SaveLocation>();
            RefreshCommand = new Command(async () => await LoadLocationsAsync());
            LoadLocationsAsync().ConfigureAwait(false);
        }

        // Load saved locations asynchronously
        private async Task LoadLocationsAsync()
        {
            try
            {
                var locations = await _locationService.GetLocationsAsync();
                SavedLocations.Clear(); // Clear the existing locations before loading

                foreach (var location in locations)
                {
                    SavedLocations.Add(location); // Add the locations
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load saved locations: {ex.Message}", "OK");
            }
        }
    }
}
