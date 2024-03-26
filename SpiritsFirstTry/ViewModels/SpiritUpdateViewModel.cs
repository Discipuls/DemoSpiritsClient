using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using SpiritsClassLibrary.Models;
using SpiritsFirstTry.DTOs;
using SpiritsFirstTry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The49.Maui.BottomSheet;

namespace SpiritsFirstTry.ViewModels
{
    public partial class SpiritUpdateViewModel : ObservableObject
    {
        [ObservableProperty]
        List<SpiritType> types = new List<SpiritType>();
        [ObservableProperty]
        UpdateMapSpiritDTO spiritDTO;
        [ObservableProperty]
        List<MapHabitat> habitats = new List<MapHabitat>();
        [ObservableProperty]
        List<UpdateHabitatMapDTO> habitatsDTOs = new List<UpdateHabitatMapDTO>();

        public MapView SpiritMapView { get; set; }
        public SpiritUpdateViewModel()
        {
            var t =  Enum.GetValues(typeof(SpiritType));
            foreach(int i in t)
            {
                types.Add((SpiritType)i);
            }
        }

        public async Task SetupMap(MapView mapView)
        {
            SpiritMapView = mapView;
            SpiritMapView.Map = new Esri.ArcGISRuntime.Mapping.Map(BasemapStyle.OSMNavigationDark);

            SpiritMapView.Map.InitialViewpoint = new Viewpoint(spiritDTO.mapPoint, 10000000);

            await Initialize();
        }


        private async Task Initialize()
        {
            try
            {
                this.SpiritMapView.GeoViewTapped += GeoViewTapped;
                await CreateSpiritMarker();
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.ToString(), "OK");
            }
        }

        private async Task CreateSpiritMarker()
        {
            var SpiritsGraphicOverlay = new GraphicsOverlay();
            GraphicsOverlayCollection overlays = SpiritMapView.GraphicsOverlays;
            overlays.Add(SpiritsGraphicOverlay);
            SpiritMapView.GraphicsOverlays = overlays;

            string localFilePath = Path.Combine(FileSystem.CacheDirectory, "MarkerImage_" + SpiritDTO.Id.ToString() + "_.png");

            using FileStream localFileStream = File.OpenRead(localFilePath);

            PictureMarkerSymbol pinSymbol = await PictureMarkerSymbol.CreateAsync(localFileStream);
            SpiritDTO.markerSymbol = pinSymbol;
            pinSymbol.Width = 40;
            pinSymbol.Height = 40;

            Graphic pinGraphic = new Graphic(SpiritDTO.mapPoint, pinSymbol);
            SpiritDTO.pinGraphic = pinGraphic;
            SpiritsGraphicOverlay.Graphics.Add(pinGraphic);

        }

        private async void GeoViewTapped(object sender, Esri.ArcGISRuntime.Maui.GeoViewInputEventArgs e)
        {

            MapPoint tappedLocation = (MapPoint)e.Location.NormalizeCentralMeridian();

        }

        [RelayCommand]
        async Task Save()
        {
            Console.WriteLine("Click!");
        }
    }


}
