using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using The_Weather_Observatory.Models;
using The_Weather_Observatory.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace The_Weather_Observatory.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private WeatherData _data;

        public WeatherData Data
        {
            get => _data;
            set
            {
                _data = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private bool _isNotLoading;
        public bool IsNotLoading 
        {
            get => _isNotLoading;
            set
            {
                _isNotLoading = value;
                OnPropertyChanged();
            }
        }

        // command to get weather by location
        public ICommand SearchCommand { get; set; }

        // command to get weather by GPS location
        public ICommand GetCurrentLocationWeatherCommand { get; set; }

        // to invoke permission request on main thread
        private Task<Location> GetLastKnownLocation()
        {
            var locationTaskCompletionSource = new TaskCompletionSource<Location>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                locationTaskCompletionSource.SetResult(await Geolocation.GetLastKnownLocationAsync());
            });

            return locationTaskCompletionSource.Task;
        }

        public MainPageViewModel()
        {
            SearchCommand = new Command<string>(async (searchTerm) => await SearchLocation(searchTerm));

            GetCurrentLocationWeatherCommand = new Command(async () => await GetCurrentLocationWeather());

            // this allows the app to retrive weather data immediately for current location
            Task.Run(async () => await GetCurrentLocationWeather());
        }

        private readonly LocationService _locationService = new LocationService();

        private async Task SearchLocation(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return;

            IsLoading = true;
            IsNotLoading = false;

            try
            {
                var input = searchTerm as string;
                var locations = await Geocoding.GetLocationsAsync(input);
                var location = locations?.FirstOrDefault();
                double lat = location.Latitude;
                double lon = location.Longitude;
                if (input != null)
                {
                    await GetWeatherData(lat, lon);
                }

                // After successful weather search, save the location
                var savedLocation = new SaveLocation
                {
                    Name = searchTerm,
                    Latitude = lat,  // Get these values from your weather API
                    Longitude = lon,
                    LastSearched = DateTime.Now
                };
                await _locationService.StoreLocationAsync(savedLocation);

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
                IsNotLoading = true;
            }
        }

        public async Task GetCurrentLocationWeather()
        {
            IsLoading = true;
            IsNotLoading = false;

            try
            {
                var location = await GetLastKnownLocation().ConfigureAwait(false);
                if (location == null)
                {
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)));
                }

                if (location != null)
                {
                    await GetWeatherData(location.Latitude, location.Longitude);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Unable to get current location.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
                IsNotLoading = true;
            }
        }

        private async Task GetWeatherData(double lat, double lon)
        {
            // to get ApiKey
            string apiKey = await SecureStorage.GetAsync("ApiKey");
            if (string.IsNullOrEmpty(apiKey))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "API key not found.", "OK");
                return;
            }
            string url = $"https://api.openweathermap.org/data/3.0/onecall?lat={lat}&lon={lon}&appid={apiKey}&units=metric&exclude=minutely";
            await GetData(url);
        }

        private async Task GetData(string url)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            var response =
                await client.GetAsync(client.BaseAddress);
            response.EnsureSuccessStatusCode();
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<WeatherData>(jsonResult);
            Data = result;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
