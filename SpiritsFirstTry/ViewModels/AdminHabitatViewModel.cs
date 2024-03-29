using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiritsClassLibrary.Models;
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
    [QueryProperty(nameof(Habitats), "Habitats")]
    public partial class AdminHabitatViewModel : ObservableObject
    {
        private IRestService _restService { get; set; }
        private IMapper _mapper;

        [ObservableProperty]
        List<MapHabitat> habitats;

        public AdminHabitatViewModel(IRestService restService, IMapper mapper) {
            this._restService = restService;
            this._mapper = mapper;
        }

        [RelayCommand]
        public async Task Edit(MapHabitat habitat)
        {
            HabitatUpdateViewModel viewModel = new HabitatUpdateViewModel(_mapper, _restService);
            viewModel.HabitatMapDTO = _mapper.Map<UpdateHabitatMapDTO>(habitat);
       
            await Application.Current.MainPage.Navigation.PushAsync(new HabitatUpdatePage(viewModel));

        }

        [RelayCommand]
        public async Task Delete(MapHabitat habitat)
        {
            var answer = await Application.Current.MainPage.DisplayAlert("Delete", $"Are you sure you want to delete {habitat.Name}?", "Delete", "Cancel");
            if (answer)
            {
                _restService.DeleteHabitatAsync(habitat.Id);
                Habitats.Remove(habitat);
            }
        }

        public async Task Create()
        {
            HabitatUpdateViewModel viewModel = new HabitatUpdateViewModel(_mapper, _restService);
            viewModel.HabitatMapDTO = new UpdateHabitatMapDTO();
            viewModel.create = true;
            await Application.Current.MainPage.Navigation.PushAsync(new HabitatUpdatePage(viewModel));

        }
    }


}
