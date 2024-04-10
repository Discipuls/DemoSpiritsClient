using Microsoft.Extensions.Configuration;
using SpiritsClassLibrary.DTOs.HabitatDTOs;
using SpiritsClassLibrary.DTOs.SpiritDTOs;
using SpiritsFirstTry.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SpiritsFirstTry.Services
{
    public class RestService : IRestService
    {
        private IGoogleAuthenticationService _googleAuthService;
        private IConfiguration Configuration;
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;
        string _baseUrl;


        public RestService(IGoogleAuthenticationService googleAuthenticationService,
            IConfiguration configuration) {
            Configuration = configuration;
            _baseUrl = configuration["Api:BaseUrl"];
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            _googleAuthService = googleAuthenticationService;
        }

        public async Task AddAuthHeader(string token)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
            HttpResponseMessage respone = await _client.PostAsync(_baseUrl + "/Spirit", stringContent);
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

        public async Task<bool> GetIsAdminAsync()
        {
            HttpResponseMessage respone = await _client.GetAsync(_baseUrl + "/Admin");
            if (respone.IsSuccessStatusCode)
            {
                string content = await respone.Content.ReadAsStringAsync();
                if (content == "true")
                {
                    return true;
                }
            }
            return false;
            
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
