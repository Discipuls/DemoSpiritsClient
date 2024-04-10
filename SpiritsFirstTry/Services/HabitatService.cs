using AutoMapper;
using SpiritsClassLibrary.DTOs.HabitatDTOs;
using SpiritsFirstTry.Models;
using SpiritsFirstTry.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace SpiritsFirstTry.Services
{
    public class HabitatService : IHabitatService
    {
        private IMapper _mapper;
        private IRestService _restService;
        private string dataDirectory;


        public HabitatService(IMapper mapper, IRestService restService) 
        {
            _mapper = mapper;
            _restService = restService;
        }

        public async Task<List<MapHabitat>> LoadHabitats(ProgressBar progressBar)
        {
            dataDirectory = FileSystem.AppDataDirectory;
            bool APIHabitatsAvailible = true;
            bool JSONHabitatsAvailible = true;
            var APIHabitats = new List<GetHabitatDTO>();
            var JSONHabitats = new List<GetHabitatDTO>();
            try
            {
                APIHabitats = await _restService.GetAllHabitatsAsync();
            }
            catch (Exception ex)
            {
                APIHabitatsAvailible = false;
                
            }

            try
            {
                JSONHabitats = await LoadJSONHabitats();
            }
            catch (Exception ex)
            {
                JSONHabitatsAvailible = false;
            }

            var resultHabitatsDTOs = new List<GetHabitatDTO>();

            if (APIHabitatsAvailible && JSONHabitatsAvailible)
            {
                resultHabitatsDTOs = APIHabitats;
                SaveHabitatsToJson(resultHabitatsDTOs);
            }
            else if (JSONHabitatsAvailible)
            {
                resultHabitatsDTOs = JSONHabitats;
            }
            else if (APIHabitatsAvailible)
            {
                resultHabitatsDTOs = APIHabitats;
                SaveHabitatsToJson(resultHabitatsDTOs);
            }
            else
            {
                throw new Exception("No sources to retrieve habitats available!");
            }

            var Habitats = resultHabitatsDTOs.Select(h => _mapper.Map<MapHabitat>(h)).ToList();
            return Habitats;
        }


        private async Task<List<GetHabitatDTO>> LoadJSONHabitats()
        {
            string localFilePath = Path.Combine(dataDirectory, "Habitats.json");
            using FileStream fileStream = File.OpenRead(localFilePath);
            var buffer = new byte[fileStream.Length];
            await fileStream.ReadAsync(buffer, 0, buffer.Length);
            string s = Encoding.Default.GetString(buffer);

            var JSONSpirits = JsonSerializer.Deserialize<List<GetHabitatDTO>>(s);//TODO rename
            return JSONSpirits;
        }


        private async Task SaveHabitatsToJson(List<GetHabitatDTO> habitats)
        {
            var json = JsonSerializer.Serialize(habitats);
            var habitatsMS = new MemoryStream();
            var habitatsSW = new StreamWriter(habitatsMS);
            habitatsSW.Write(json);
            habitatsSW.Flush();
            habitatsMS.Position = 0;

            string localFilePath = Path.Combine(dataDirectory, "Habitats.json");
            using FileStream fileStream = File.OpenWrite(localFilePath);

            await habitatsMS.CopyToAsync(fileStream);
        }

    }
}
