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

                // Update the SavedLocations collection to match the locations from the service
                SavedLocations.Clear();
                foreach (var location in locations)
                {
                    if (!SavedLocations.Contains(location))
                    {
                        SavedLocations.Add(location);
                    }
                }

                // Remove any locations from SavedLocations that are not in the service's list
                var locationNames = locations.Select(l => l.Name);
                var locationsToRemove = SavedLocations.Where(l => !locationNames.Contains(l.Name)).ToList();
                foreach (var locationToRemove in locationsToRemove)
                {
                    SavedLocations.Remove(locationToRemove);
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
                    Name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(locationName.ToLower()),
                    LastSearched = DateTime.Now
                    // You might want to add logic to get current weather icon and temperature here
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
                // Get the MainPage instance
                var mainPage = Application.Current.MainPage;
                if (mainPage != null)
                {
                    // Get the MainPageViewModel instance from the BindingContext of the MainPage
                    var mainPageViewModel = mainPage.BindingContext as MainPageViewModel;
                    if (mainPageViewModel != null)
                    {
                        // Call the GetWeatherData method in MainPageViewModel
                        await mainPageViewModel.GetWeatherData(selectedLocation.Latitude, selectedLocation.Longitude);

                        // Navigate back to the MainPage
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }
                }
            }
        }

        private async void DeleteLocationCommandHandler(string locationName)
        {
            try
            {
                var locationToDelete = SavedLocations.FirstOrDefault(l => l.Name == locationName);
                if (locationToDelete != null)
                {
                    bool answer = await Application.Current.MainPage.DisplayAlert("Confirm Deletion", $"Are you sure you want to delete {locationToDelete.Name}?", "Yes", "No");
                    if (answer)
                    {
                        SavedLocations.Remove(locationToDelete);
                        // Update the stored locations
                        await _locationService.DeleteLocationAsync(locationToDelete);
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to delete location: {ex.Message}", "OK");
            }
        }

    }
}
