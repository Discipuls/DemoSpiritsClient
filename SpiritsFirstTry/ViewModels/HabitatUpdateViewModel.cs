using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Editing;
using SpiritsClassLibrary.Models;
using SpiritsFirstTry.DTOs;
using SpiritsFirstTry.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritsFirstTry.ViewModels
{
    public partial class HabitatUpdateViewModel : ObservableObject
    {
        IMapper _mapper;
        IRestService _restService;
        [ObservableProperty]
        UpdateHabitatMapDTO habitatMapDTO;
        public MapView HabitatMapView { get; set; }
        public Graphic habitatGraphic { get; set; }




        public HabitatUpdateViewModel(IMapper mapper, IRestService restService)
        {
            _mapper = mapper;
            _restService = restService;
        }

        public async Task SetupMap(MapView mapView)
        {
            HabitatMapView = mapView;
            HabitatMapView.Map = new Esri.ArcGISRuntime.Mapping.Map(BasemapStyle.OSMNavigationDark);
            var point = new MapPoint((double)habitatMapDTO.Border[0].Latitude,
                (double)habitatMapDTO.Border[0].Longitude, SpatialReferences.Wgs84);

            HabitatMapView.Map.InitialViewpoint = new Viewpoint(point, 10000000);

            await CreateHabitatOverlay();
        }

        private async Task CreateHabitatOverlay()
        {
            var HabitatsOverlay = new GraphicsOverlay();
            HabitatMapView.GraphicsOverlays.Add(HabitatsOverlay);

            var polygonGraphicCopy = new Graphic(HabitatMapDTO.PolygonGraphic.Geometry,
                                                HabitatMapDTO.PolygonGraphic.Symbol);

            HabitatsOverlay.Graphics.Add(polygonGraphicCopy);
            habitatGraphic = polygonGraphicCopy;
        }

        [RelayCommand]
        public async Task EditHabitat()
        {
            HabitatMapView.GeometryEditor.Start(habitatGraphic.Geometry);
        }

        [RelayCommand]
        public async Task Undo()
        {
            HabitatMapView.GeometryEditor.Undo();
        }
        [RelayCommand]
        public async Task Redo()
        {
            HabitatMapView.GeometryEditor.Redo();
        }

        [RelayCommand]
        public async Task Complete()
        {
            habitatGraphic.Geometry = HabitatMapView.GeometryEditor.Stop();
            
        }
    }
}
