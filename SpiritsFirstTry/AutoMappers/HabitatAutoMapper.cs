using AutoMapper;
using SpiritsClassLibrary.DTOs.HabitatDTOs;
using SpiritsClassLibrary.Models;
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
            CreateMap<GetHabitatDTO, Habitat>();
        }
    }
}
