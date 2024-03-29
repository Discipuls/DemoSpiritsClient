using AutoMapper;
using SpiritsClassLibrary.DTOs.GeoPointDTOs;
using SpiritsClassLibrary.DTOs.HabitatDTOs;
using SpiritsClassLibrary.Models;
using SpiritsFirstTry.DTOs;
using SpiritsFirstTry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritsFirstTry.AutoMappers
{
    class HabitatAutoMapper : Profile
    {
        public HabitatAutoMapper() {
            CreateMap<GetHabitatDTO, MapHabitat>();
            CreateMap<GetGeoPointDTO, BorderPoint>();
            CreateMap<CreateGeoPointDTO, BorderPoint>().ReverseMap();
            CreateMap<CreateGeoPointDTO, MarkerPoint>().ReverseMap();
            CreateMap<UpdateHabitatMapDTO, MapHabitat>().ReverseMap();
            CreateMap<UpdateHabitatDTO, UpdateHabitatMapDTO>().ReverseMap();
            CreateMap<UpdateHabitatDTO, CreateHabitatDTO>();

        }
    }
}
