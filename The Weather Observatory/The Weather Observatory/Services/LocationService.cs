using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using The_Weather_Observatory.Models;

namespace The_Weather_Observatory.Services
{
    public class LocationService
    {
        private const string LocationsKey = "SearchedLocations";

        // Store a location asynchronously
        public async Task StoreLocationAsync(SaveLocation savedLocation)
        {
            var savedLocations = await GetLocationsAsync();

            // Check if the location already exists
            var existingLocation = savedLocations.FirstOrDefault(l => l.Name == savedLocation.Name);
            if (existingLocation != null)
            {
                existingLocation.LastSearched = DateTime.Now; // Update the last searched time
            }
            else
            {
                savedLocation.LastSearched = DateTime.Now; // Set the current time if it's a new location
                savedLocations.Add(savedLocation);
            }

            // Sort by LastSearched descending to keep the most recent on top
            savedLocations = savedLocations.OrderByDescending(l => l.LastSearched).ToList();

            // Save the list back to secure storage
            var json = JsonConvert.SerializeObject(savedLocations);
            await SecureStorage.SetAsync(LocationsKey, json);
        }

        // Retrieve the saved locations asynchronously
        public async Task<List<SaveLocation>> GetLocationsAsync()
        {
            var json = await SecureStorage.GetAsync(LocationsKey);
            if (string.IsNullOrEmpty(json))
                return new List<SaveLocation>();

            return JsonConvert.DeserializeObject<List<SaveLocation>>(json);
        }

        // Delete a saved location
        public async Task DeleteLocationAsync(string locationName)
        {
            var savedLocations = await GetLocationsAsync();
            savedLocations.RemoveAll(l => l.Name == locationName);
            var json = JsonConvert.SerializeObject(savedLocations);
            await SecureStorage.SetAsync(LocationsKey, json);
        }
    }
}
