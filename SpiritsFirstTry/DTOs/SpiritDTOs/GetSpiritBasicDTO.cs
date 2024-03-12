using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritsFirstTry.DTOs.SpiritDTOs
{
    public enum SpiritType { Forest, Water, Home, Dark, Field }
    public class GetGeoPointDTO
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
    public class GetSpiritBasicDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<SpiritType> Classification { get; set; }
        public List<int> HabitatsIds { get; set; }
        public DateTime LastUpdated { get; set; }
        public GetGeoPointDTO MarkerLocation { get; set; }

    }
}
