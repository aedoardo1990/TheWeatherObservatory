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

        public ICommand SearchCommand { get; set; }
        public MainPageViewModel()
        {
            SearchCommand = new Command(async (searchTerm) =>
            {
                // to get ApiKey
                string apiKey = await SecureStorage.GetAsync("ApiKey");
                if (string.IsNullOrEmpty(apiKey))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "API key not found.", "OK");
                    return;
                }

                try
                {
                    var input = searchTerm as string;
                    var locations = await Geocoding.GetLocationsAsync(input);
                    var location = locations?.FirstOrDefault();
                    var lat = location.Latitude;
                    var lon = location.Longitude;
                    if (input != null)
                    {
                        await GetData($"https://api.openweathermap.org/data/3.0/onecall?lat={lat}&lon={lon}&appid={apiKey}&units=metric&exclude=minutely");
                    }
                }
                catch (FeatureNotSupportedException fnsEx)
                {
                    // Feature not supported on device
                }
                catch (Exception ex)
                {
                    // Handle exception that may have occurred in geocoding
                }
            });
        }
        public async Task GetData(string url)
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
