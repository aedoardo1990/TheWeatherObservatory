using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace The_Weather_Observatory
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Call the async method to store the API key
            // _ = StoreApiKeyAsync();
        }

        private async Task StoreApiKeyAsync()
        {
            try
            {
                await SecureStorage.SetAsync("ApiKey", "your-api-key");  // Replace "your-api-key" with your actual API key
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur when storing the key
                Console.WriteLine($"Error storing API key");
            }
        }
        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}