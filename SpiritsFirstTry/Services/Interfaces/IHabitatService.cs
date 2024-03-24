using SpiritsFirstTry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritsFirstTry.Services.Interfaces
{
    public interface IHabitatService
    {
        public Task<List<MapHabitat>> LoadHabitats(ProgressBar progressBar); 
    }
}
