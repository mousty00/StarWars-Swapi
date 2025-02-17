using StarWars.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace StarWars.Services
{
    public class CharacterService
    {
        private readonly HttpClient _client;

        public CharacterService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<Character>> GetCharacters()
        {
            try
            {
                var data = await _client.GetFromJsonAsync<ApiResponse<Character>>("https://swapi.dev/api/people/");

                if (data?.Results != null)
                {
                    foreach (var character in data.Results)
                    {
                        if (!string.IsNullOrEmpty(character.homeworld))
                        {
                            character.PlanetInfo = await GetPlanet(character.homeworld);
                        }

                        if (character.vehicles_urls?.Count > 0)
                        {
                            var vehicleNames = await GetVehicles(character.vehicles_urls);
                            character.vehiclesList = vehicleNames;
                        }

                        if (character.starships_urls?.Count > 0)
                        {
                            var starshipNames = await GetStarships(character.starships_urls);
                            character.starshipsList = starshipNames;
                        }
                    }

                    return data.Results;
                }
                return new List<Character>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching characters: {ex.Message}");
                await new MessageDialog($"Error Fetching Characters: {ex.Message}").ShowAsync();
                return new List<Character>();
            }
        }

        private async Task<Planet> GetPlanet(string url)
        {
            try
            {
                return await _client.GetFromJsonAsync<Planet>(url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching planet: {ex.Message}");
                await new MessageDialog($"Error Fetching Planet: {ex.Message}").ShowAsync();
                return null;
            }
        }

        private async Task<List<string>> GetVehicles(List<string> urls)
        {
            var vehicles = new List<string>();
            foreach (var url in urls)
            {
                try
                {
                    var vehicle = await _client.GetFromJsonAsync<Vehicle>(url);
                    if (vehicle != null)
                    {
                        vehicles.Add(vehicle.name);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error fetching vehicle: {ex.Message}");
                    await new MessageDialog($"Error Fetching Vehicle: {ex.Message}").ShowAsync();
                }
            }
            return vehicles;
        }

        private async Task<List<string>> GetStarships(List<string> urls)
        {
            var starships = new List<string>();
            foreach (var url in urls)
            {
                try
                {
                    var starship = await _client.GetFromJsonAsync<Starship>(url);
                    if (starship != null)
                    {
                        starships.Add(starship.name);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error fetching starship: {ex.Message}");
                    await new MessageDialog($"Error Fetching Starship: {ex.Message}").ShowAsync();
                }
            }
            return starships;
        }


    }
}