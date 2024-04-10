using SpiritsFirstTry.Models;

namespace SpiritsFirstTry.Services.Interfaces
{
    public interface IHabitatService
    {
        public Task<List<MapHabitat>> LoadHabitats(ProgressBar progressBar); 
    }
}
