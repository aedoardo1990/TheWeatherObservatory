using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                string apiKey = await SecureStorage.GetAsync("ApiKey");
                if (string.IsNullOrEmpty(apiKey))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "API key not found.", "OK");
                    return;
                }

                var input = searchTerm as string;
                if (input != null)
                {
                    var coords = input.Split(',');
                    if (coords.Length == 2 &&
                        double.TryParse(coords[0], out double lat) &&
                        double.TryParse(coords[1], out double lon))
                    {
                        await GetData($"https://api.openweathermap.org/data/3.0/onecall?lat={lat}&lon={lon}&appid={apiKey}&units=metric&exclude=minutely");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Please enter valid coordinates in the format 'lat,lon'.", "OK");
                    }
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
