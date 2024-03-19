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

namespace SpiritsFirstTry.Services
{
    public class RestService : IRestService
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;
        string _baseUrl = "https://husky-evolved-radically.ngrok-free.app";

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
            throw new NotImplementedException();
        }

        public async Task CreateSpiritAsync(CreateSpiritDTO createSpiritDTO)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteHabitatAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteSpiritAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GetHabitatDTO>> GetAllHabitatsAsync()
        {
            var habitats = new List<GetHabitatDTO>();

            try
            {
                HttpResponseMessage respone = await _client.GetAsync(_baseUrl + "/Habitat");
                if (respone.IsSuccessStatusCode)
                {
                    string content = await respone.Content.ReadAsStringAsync();
                    habitats = JsonSerializer.Deserialize<List<GetHabitatDTO>>(content, _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public async Task UpdateSpiritAsync(UpdateSpiritDTO updateSpiritDTO)
        {
            throw new NotImplementedException();
        }
    }

}
