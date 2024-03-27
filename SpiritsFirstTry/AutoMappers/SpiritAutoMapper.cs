using AutoMapper;
using SpiritsClassLibrary.DTOs.GeoPointDTOs;
using SpiritsClassLibrary.DTOs.SpiritDTOs;
using SpiritsClassLibrary.Models;
using SpiritsFirstTry.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritsFirstTry.AutoMappers
{
    public class SpiritAutoMapper : Profile
    {
        public SpiritAutoMapper() {
            CreateMap<GetSpiritBasicsDTO, Spirit>();
            CreateMap<GetSpiritBasicsDTO, MapSpirit>();
            CreateMap<GetGeoPointDTO, MarkerPoint>();
            CreateMap<GetSpiritDTO, MapSpirit>();
            CreateMap<UpdateMapSpiritDTO, MapSpirit>().ReverseMap();
            CreateMap<UpdateSpiritDTO, UpdateMapSpiritDTO>().ReverseMap();
            CreateMap<UpdateSpiritDTO, CreateSpiritDTO>(); 
        }
    }
}
