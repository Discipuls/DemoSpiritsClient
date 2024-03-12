using Microsoft.VisualBasic;
using SpiritsFirstTry.DTOs.SpiritDTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpiritsFirstTry.Services
{
    public class RestService
    {


        HttpClient _client;
        JsonSerializerOptions _serializerOptions;

        public RestService() { 
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<List<GetSpiritBasicDTO>> GetAllAsync()
        {
            var spirits = new List<GetSpiritBasicDTO>();

            try
            {
                HttpResponseMessage respone = await _client.GetAsync("https://b701-46-53-235-28.ngrok-free.app/Spirit");
                if (respone.IsSuccessStatusCode)
                {
                    string content = await respone.Content.ReadAsStringAsync();
                    spirits = JsonSerializer.Deserialize<List<GetSpiritBasicDTO>>(content, _serializerOptions);
                }
            }catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return spirits;
        }
    }

}
