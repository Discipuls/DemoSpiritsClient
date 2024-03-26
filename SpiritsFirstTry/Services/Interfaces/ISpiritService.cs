using SpiritsClassLibrary.Models;
using SpiritsFirstTry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritsFirstTry.Services.Interfaces
{
    public interface ISpiritService
    {
        public Task<List<MapSpirit>> LoadSpirits(ProgressBar progressBar, List<MapHabitat> habitats);
    }
}
