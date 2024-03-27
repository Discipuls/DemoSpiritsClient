using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiritsClassLibrary.DTOs.SpiritDTOs;
using SpiritsFirstTry.AutoMappers;
using SpiritsFirstTry.DTOs;
using SpiritsFirstTry.Models;
using SpiritsFirstTry.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritsFirstTry.ViewModels
{
    [QueryProperty(nameof(Spirits), "Spirits")]
    [QueryProperty(nameof(Habitats), "Habitats")]
    public partial class AdminSpiritsViewModel : ObservableObject
    {
        private IMapper _mapper;
        private IRestService _restService;
        [ObservableProperty]
        List<MapSpirit> spirits = new List<MapSpirit>();
        [ObservableProperty]
        string cacheDirectory;
        [ObservableProperty]
        List<MapHabitat> habitats = new List<MapHabitat>();

        public AdminSpiritsViewModel(IRestService restService, IMapper mapper) { 
            cacheDirectory = FileSystem.CacheDirectory;
            _restService = restService;
            _mapper = mapper;
        }

        [RelayCommand]
        public async Task Delete(MapSpirit spirit)
        {
            var answer =  await Application.Current.MainPage.DisplayAlert("Delete", $"Are you sure you want delete {spirit.Name}?", "Delete", "Cancel");
            if (answer)
            {
                _restService.DeleteSpiritAsync(spirit.Id);
                Spirits.Remove(spirit);
            }
        }

        [RelayCommand]
        public async Task Edit(MapSpirit spirit)
        {
            UpdateSpiritDTO updateSpiritDTO = new UpdateSpiritDTO();
            SpiritUpdateViewModel viewModel = new SpiritUpdateViewModel(_mapper, _restService);
            viewModel.SpiritDTO = _mapper.Map<UpdateMapSpiritDTO>(spirit);
            viewModel.Habitats = habitats;
            viewModel.HabitatsDTOs = Habitats.Select( h=> _mapper.Map<UpdateHabitatMapDTO>(h)).ToList();
            viewModel.SpiritDTO.HabitatsDTOs 
                = viewModel.SpiritDTO.Habitats.Select(h => _mapper.Map<UpdateHabitatMapDTO>(h)).ToList();
            
            foreach(var habitatDTO in viewModel.SpiritDTO.HabitatsDTOs) 
            {
                habitatDTO.index = viewModel.HabitatsDTOs.FindIndex(0,(h => h.Id == habitatDTO.Id));
            }
            await Application.Current.MainPage.Navigation.PushAsync(new SpiritUpdatePage(viewModel));

            Console.WriteLine();
        }
    }
}
