using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Editing;
using Microsoft.Maui.Controls.Shapes;
using SpiritsClassLibrary.DTOs.GeoPointDTOs;
using SpiritsClassLibrary.DTOs.HabitatDTOs;
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
        public Esri.ArcGISRuntime.Geometry.Polygon defaultGeometry {  get; set; }

        public bool create;

        public HabitatUpdateViewModel(IMapper mapper, IRestService restService)
        {
            _mapper = mapper;
            _restService = restService;
        }

        public async Task SetupMap(MapView mapView)
        {


            List<MapPoint> points = new List<MapPoint> {
                new MapPoint(25.542744, 54.897867, SpatialReferences.Wgs84) ,
            new MapPoint(27.542744, 53.897867, SpatialReferences.Wgs84),
            new MapPoint(26.542744, 54.897867, SpatialReferences.Wgs84)
            };
            defaultGeometry = new Esri.ArcGISRuntime.Geometry.Polygon(points);

            if (create)
            {
                HabitatMapDTO.Border = new List<CreateGeoPointDTO> { new CreateGeoPointDTO { Latitude = 25, Longitude = 54 } };
                var polygonSymbolOutline = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Orange, 2.0);
                var polygonFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(50, System.Drawing.Color.RebeccaPurple), polygonSymbolOutline);

                HabitatMapDTO.PolygonGraphic = new Graphic(defaultGeometry, polygonSymbolOutline);
                HabitatMapDTO.Name = "Імя";
            }

            HabitatMapView = mapView;
            HabitatMapView.Map = new Esri.ArcGISRuntime.Mapping.Map(BasemapStyle.OSMNavigationDark);
            var point = new MapPoint((double)habitatMapDTO.Border[0].Latitude,
                (double)habitatMapDTO.Border[0].Longitude, SpatialReferences.Wgs84);

            HabitatMapView.Map.InitialViewpoint = new Viewpoint(point, 10000000);

            await CreateHabitatOverlay();
        }

        private async Task CreateHabitatOverlay()
        {
  
            try
            {
                var HabitatsOverlay = new GraphicsOverlay();
                HabitatMapView.GraphicsOverlays.Add(HabitatsOverlay);

                var polygonGraphicCopy = new Graphic(HabitatMapDTO.PolygonGraphic.Geometry,
                                                    HabitatMapDTO.PolygonGraphic.Symbol);

                HabitatsOverlay.Graphics.Add(polygonGraphicCopy);
                habitatGraphic = polygonGraphicCopy;
            }
            catch
            {
                Application.Current.MainPage.DisplayAlert("Error", "Smth went wrong with this habitat. Recreated basic habitat.", "Ok");
                var polygonSymbolOutline = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Orange, 2.0);
                var polygonFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(50, System.Drawing.Color.RebeccaPurple), polygonSymbolOutline);

                HabitatMapDTO.PolygonGraphic = new Graphic(defaultGeometry, polygonFillSymbol);

                var HabitatsOverlay = new GraphicsOverlay();
                HabitatMapView.GraphicsOverlays.Add(HabitatsOverlay);

                var polygonGraphicCopy = new Graphic(HabitatMapDTO.PolygonGraphic.Geometry,
                                                    HabitatMapDTO.PolygonGraphic.Symbol);

                HabitatsOverlay.Graphics.Add(polygonGraphicCopy);
                habitatGraphic = polygonGraphicCopy;
            }

        }

        [RelayCommand]
        public async Task EditHabitat()
        {
            try
            {
                HabitatMapView.GeometryEditor.Start(habitatGraphic.Geometry);
            }
            catch
            {
                Application.Current.MainPage.DisplayAlert("Error", "Smth went wrong with this habitat. Recreated basic habitat.", "Ok");
                var polygonSymbolOutline = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Orange, 2.0);
                var polygonFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(50, System.Drawing.Color.RebeccaPurple), polygonSymbolOutline);

                habitatGraphic = new Graphic(defaultGeometry, polygonFillSymbol);
                HabitatMapDTO.PolygonGraphic = habitatGraphic;
                await CreateHabitatOverlay();
                HabitatMapView.GeometryEditor.Start(habitatGraphic.Geometry);
            }
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
        public async Task DeleteSelected()
        {
            try
            {
                HabitatMapView.GeometryEditor.DeleteSelectedElement();
            }
            catch
            {
                HabitatMapView.GeometryEditor.Redo();
            }
        }

        [RelayCommand]
        public async Task Complete()
        {
            try
            {
                var geometry = HabitatMapView.GeometryEditor.Stop();
                if (geometry != null)
                {
                    habitatGraphic.Geometry = geometry;
                }
            }
            catch
            {

            }


        }

        [RelayCommand]
        public async Task Save()
        {
                var polygon = (Esri.ArcGISRuntime.Geometry.Polygon)Esri.ArcGISRuntime.Geometry.Geometry.FromJson(habitatGraphic.Geometry.ToJson());
                List<CreateGeoPointDTO> createGeoPoints = new List<CreateGeoPointDTO>();

                foreach (var part in polygon.Parts)
                {
                    foreach (var point in part.Points)
                    {
                        var projectedPoint = (MapPoint)point.Project(SpatialReferences.Wgs84);
                        createGeoPoints.Add(new CreateGeoPointDTO { Latitude = projectedPoint.X, Longitude = projectedPoint.Y });
                    }

                }
                if (createGeoPoints.Count < 3)
                {
                    await Application.Current.MainPage.DisplayAlert("Not enough points!", "", "Ok");
                    return;
                }
                habitatMapDTO.Border = createGeoPoints;
                var habitatUpdateDTO = _mapper.Map<UpdateHabitatDTO>(habitatMapDTO);
            if (!create)
            {
                await _restService.UpdateHabitatAsync(habitatUpdateDTO);

            }
            else 
            {
                var createHabitatDTO = _mapper.Map<CreateHabitatDTO>(habitatUpdateDTO);
                await _restService.CreateHabitatAsync(createHabitatDTO);
            }
            await Application.Current.MainPage.Navigation.PopAsync();

        }
    }
}
