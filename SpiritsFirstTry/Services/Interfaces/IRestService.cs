using SpiritsClassLibrary.DTOs.HabitatDTOs;
using SpiritsClassLibrary.DTOs.SpiritDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritsFirstTry.Services.Interfaces
{
    public interface IRestService
    {
        public Task<List<GetSpiritBasicsDTO>> GetAllSpiritsAsync();
        public Task<GetSpiritDTO> GetSpiritAsync(int id);
        public Task UpdateSpiritAsync(UpdateSpiritDTO updateSpiritDTO);
        public Task DeleteSpiritAsync(int id);
        public Task CreateSpiritAsync(CreateSpiritDTO createSpiritDTO);

        public Task<List<GetHabitatDTO>> GetAllHabitatsAsync();
        public Task<GetHabitatDTO> GetHabitatAsync(int id);
        public Task UpdateHabitatAsync(UpdateHabitatDTO updateHabitatDTO);
        public Task DeleteHabitatAsync(int id);
        public Task CreateHabitatAsync(CreateHabitatDTO createHabitatDTO);
    }
}
