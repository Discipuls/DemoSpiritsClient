using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SpiritsClassLibrary.DTOs;
using SpiritsClassLibrary.Models;
using SpiritsClassLibrary.DTOs.SpiritDTOs;
using SpiritsClassLibrary.DTOs.HabitatDTOs;
using SpiritsFirstTry.Services.Interfaces;

namespace SpiritsFirstTry.Services
{
    public class RestService : IRestService
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;
        string _baseUrl = "https://heroic-naturally-reptile.ngrok-free.app";

        public RestService() {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task CreateHabitatAsync(CreateHabitatDTO createHabitatDTO)
        {
            using StringContent stringContent = new(
                JsonSerializer.Serialize(createHabitatDTO), Encoding.UTF8, "application/json");
            HttpResponseMessage respone = await _client.PostAsync(_baseUrl + "/Habitat", stringContent);
        }

        public async Task CreateSpiritAsync(CreateSpiritDTO createSpiritDTO)
        {
            using StringContent stringContent = new(
                JsonSerializer.Serialize(createSpiritDTO), Encoding.UTF8, "application/json");
            HttpResponseMessage respone = await _client.PostAsync(_baseUrl + "/Habitat", stringContent);
        }
        public async Task DeleteHabitatAsync(int id)
        {
            HttpResponseMessage respone = await _client.DeleteAsync(_baseUrl + "/habitat/" + id.ToString());
        }

        public async Task DeleteSpiritAsync(int id)
        {
            HttpResponseMessage respone = await _client.DeleteAsync(_baseUrl + "/spirit/" + id.ToString());
        }

        public async Task<List<GetHabitatDTO>> GetAllHabitatsAsync()
        {
            var habitats = new List<GetHabitatDTO>();

            HttpResponseMessage respone = await _client.GetAsync(_baseUrl + "/Habitat");
            if (respone.IsSuccessStatusCode)
            {
                string content = await respone.Content.ReadAsStringAsync();
                habitats = JsonSerializer.Deserialize<List<GetHabitatDTO>>(content, _serializerOptions);
            }
            else
            {
                throw new Exception("Spirits API unavailable");
            }
            return habitats;
        }

        public async Task<List<GetSpiritBasicsDTO>> GetAllSpiritsAsync()
        {
            var spirits = new List<GetSpiritBasicsDTO>();

            HttpResponseMessage respone = await _client.GetAsync(_baseUrl + "/spirit");
            if (respone.IsSuccessStatusCode)
            {
                string content = await respone.Content.ReadAsStringAsync();
                spirits = JsonSerializer.Deserialize<List<GetSpiritBasicsDTO>>(content, _serializerOptions);
            }
            else
            {
                throw new Exception("Spirits API unavailable");
            }
            return spirits;
        }

        public async Task<GetHabitatDTO> GetHabitatAsync(int id)
        {
            var habitat = new GetHabitatDTO();

            HttpResponseMessage respone = await _client.GetAsync(_baseUrl + "/spirit/" + id.ToString());
            if (respone.IsSuccessStatusCode)
            {
                string content = await respone.Content.ReadAsStringAsync();
                habitat = JsonSerializer.Deserialize<GetHabitatDTO>(content, _serializerOptions);
            }

            return habitat;
        }

        public async Task<GetSpiritDTO> GetSpiritAsync(int id)
        {
            var spirit = new GetSpiritDTO();

            HttpResponseMessage respone = await _client.GetAsync(_baseUrl + "/spirit/" + id.ToString());
            if (respone.IsSuccessStatusCode)
            {
                string content = await respone.Content.ReadAsStringAsync();
                spirit = JsonSerializer.Deserialize<GetSpiritDTO>(content, _serializerOptions);
            }

            return spirit;
        }

        public async Task UpdateHabitatAsync(UpdateHabitatDTO updateHabitatDTO)
        {
            using StringContent stringContent = new(
                JsonSerializer.Serialize(updateHabitatDTO), Encoding.UTF8, "application/json");
            HttpResponseMessage respone = await _client.PutAsync(_baseUrl + "/Habitat", stringContent);
        }

        public async Task UpdateSpiritAsync(UpdateSpiritDTO updateSpiritDTO)
        {
            using StringContent stringContent = new(
                JsonSerializer.Serialize(updateSpiritDTO), Encoding.UTF8, "application/json");
            HttpResponseMessage respone = await _client.PutAsync(_baseUrl + "/Spirit", stringContent);
        }
    }

}
