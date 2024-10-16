using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using The_Weather_Observatory.Models;

namespace The_Weather_Observatory.Services
{
    public class LocationService
    {
        private const string LocationsKey = "SearchedLocations";

        public async Task StoreLocationAsync(SaveLocation location)
        {
            var locations = await GetLocationsAsync();

            // Check if location already exists
            var existingLocation = locations.Find(l => l.Name == location.Name);
            if (existingLocation != null)
            {
                existingLocation.LastSearched = DateTime.Now;
            }
            else
            {
                locations.Add(location);
            }

            var json = JsonConvert.SerializeObject(locations);
            await SecureStorage.SetAsync(LocationsKey, json);
        }

        public async Task<List<SaveLocation>> GetLocationsAsync()
        {
            var json = await SecureStorage.GetAsync(LocationsKey);
            if (string.IsNullOrEmpty(json))
                return new List<SaveLocation>();

            return JsonConvert.DeserializeObject<List<SaveLocation>>(json);
        }
    }
}
