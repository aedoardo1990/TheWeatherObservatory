using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using The_Weather_Observatory.Services;
using The_Weather_Observatory.Models;
using Xamarin.Forms;
using System.Linq;

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
        // Other commands
        public Command BackCommand { get; set; }
        public Command AddLocationCommand { get; set; }
        public Command SelectLocationCommand { get; set; }
        public Command DeleteLocationCommand { get; set; }

        public SavedLocationsPageViewModel(LocationService locationService)
        {
            _locationService = locationService;
            SavedLocations = new ObservableCollection<SaveLocation>();
            RefreshCommand = new Command(async () => await LoadLocationsAsync());
            LoadLocationsAsync().ConfigureAwait(false);

            BackCommand = new Command(BackCommandHandler);
            AddLocationCommand = new Command(AddLocationCommandHandler);
            SelectLocationCommand = new Command<string>(SelectLocationCommandHandler);
            DeleteLocationCommand = new Command<string>(DeleteLocationCommandHandler);
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

        // Command handlers
        private async void BackCommandHandler()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async void AddLocationCommandHandler()
        {
            string locationName = await Application.Current.MainPage.DisplayPromptAsync("Add Location", "Enter location name:");
            if (!string.IsNullOrWhiteSpace(locationName))
            {
                var newLocation = new SaveLocation
                {
                    Name = locationName,
                    LastSearched = DateTime.Now
                    // You might want to add logic to get latitude and longitude here
                };
                await _locationService.StoreLocationAsync(newLocation);
                await LoadLocationsAsync(); // Refresh the list
            }
        }

        private async void SelectLocationCommandHandler(string locationName)
        {
            var selectedLocation = SavedLocations.FirstOrDefault(l => l.Name == locationName);
            if (selectedLocation != null)
            {
                // Here you would typically navigate back to the main page and display weather for this location
                // For now, let's just show an alert
                await Application.Current.MainPage.DisplayAlert("Location Selected", $"You selected {selectedLocation.Name}", "OK");
            }
        }

        private async void DeleteLocationCommandHandler(string locationName)
        {
            var locationToDelete = SavedLocations.FirstOrDefault(l => l.Name == locationName);
            if (locationToDelete != null)
            {
                bool answer = await Application.Current.MainPage.DisplayAlert("Confirm Deletion", $"Are you sure you want to delete {locationToDelete.Name}?", "Yes", "No");
                if (answer)
                {
                    SavedLocations.Remove(locationToDelete);
                    // Update the stored locations
                    await _locationService.StoreLocationAsync(new SaveLocation()); // This will trigger saving the current list
                    await LoadLocationsAsync(); // Refresh the list
                }
            }
        }

    }
}
