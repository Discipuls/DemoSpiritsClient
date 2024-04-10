using SpiritsFirstTry.Models;

namespace SpiritsFirstTry.Services.Interfaces
{
    public interface ISpiritService
    {
        public Task<List<MapSpirit>> LoadSpirits(ProgressBar progressBar, List<MapHabitat> habitats);
    }
}
