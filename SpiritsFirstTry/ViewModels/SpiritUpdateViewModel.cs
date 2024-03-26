using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
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

            var MinskPoint = new MapPoint(27.542744, 53.897867, SpatialReferences.Wgs84);

            SpiritMapView.Map.InitialViewpoint = new Viewpoint(MinskPoint, 10000000);

            await Initialize();
        }


        private async Task Initialize()
        {
            try
            {
                this.SpiritMapView.GeoViewTapped += GeoViewTapped;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.ToString(), "OK");
            }
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
