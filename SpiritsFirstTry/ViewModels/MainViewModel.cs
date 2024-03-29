using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using SpiritsClassLibrary.DTOs.GeoPointDTOs;
using SpiritsClassLibrary.DTOs.HabitatDTOs;
using SpiritsClassLibrary.DTOs.SpiritDTOs;
using SpiritsClassLibrary.Models;
using SpiritsFirstTry.Models;
using SpiritsFirstTry.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using The49.Maui.BottomSheet;
using Map = Esri.ArcGISRuntime.Mapping.Map;

namespace SpiritsFirstTry.ViewModels
{

    public partial class MainViewModel : ObservableObject
    {
        ISpiritService _spiritService;
        IHabitatService _habitatService;

        public MapView MainMapView { get; set; }
        public ProgressBar progressBar { get; set; }
        public static List<MapSpirit> Spirits { get; set; }
        public static List<MapHabitat> Habitats {  get; set; }
        BottomSheetView bottomSheeet;
        BottomSheetViewModel bottomSheeetVm;
        public GraphicsOverlay polygonOverlay;
        private string dataDirectory;


        public MainViewModel(ISpiritService spiritService, IHabitatService habitatService) {
            _spiritService = spiritService;
            _habitatService = habitatService;


            bottomSheeetVm = new BottomSheetViewModel();

            bottomSheeet = new BottomSheetView(bottomSheeetVm);
            bottomSheeet.IsCancelable = false;
            bottomSheeet.Detents.Add(new MediumDetent());
            bottomSheeet.Detents.Add(new FullscreenDetent());
            dataDirectory = FileSystem.AppDataDirectory;
            //     bottomSheeet.SelectedDetent = bottomSheeet.Detents[2];

        }

        public List<MapSpirit> GetSpirits()
        {
            return Spirits;
        }
        public List<MapHabitat> GetHabitats()
        {
            return Habitats;
        }
        public void SetupMap(MapView mapView)
        {
            MainMapView = mapView;
            bottomSheeetVm.mapView = MainMapView;
            MainMapView.Map = new Map(BasemapStyle.OSMNavigationDark);

            var MinskPoint = new MapPoint(27.542744, 53.897867, SpatialReferences.Wgs84);

            MainMapView.Map.InitialViewpoint = new Viewpoint(MinskPoint, 10000000);

            _ = Initialize();
        }

        public async Task SetupProgressBar(ProgressBar pg)
        {
            this.progressBar = pg;
        }

        public async Task HideBottomPage()
        {
            if(bottomSheeet.IsLoaded) 
            bottomSheeet.DismissAsync();
        }

        private async Task Initialize()
        {

            try
            {
                await progressBar.ProgressTo(0.0, 500, Easing.Linear);

                Habitats = await _habitatService.LoadHabitats(progressBar);
                await progressBar.ProgressTo(0.25, 500, Easing.Linear);

                Spirits = await _spiritService.LoadSpirits(progressBar, Habitats);

                await progressBar.ProgressTo(1, 500, Easing.Linear);
                progressBar.IsVisible = false;

                await CreateSpiritMarkers();

                bottomSheeetVm.Spirits = Spirits;
                bottomSheeetVm.Habitats = Habitats;

                this.MainMapView.GeoViewTapped += GeoViewTapped;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.ToString(), "OK");
            }

        }

        public void OpenSearch()
        {
            if (bottomSheeet.Detents.Count < 3)
            {
                var ZeroDetent = new RatioDetent();
                ZeroDetent.Ratio = 0.1f;
                bottomSheeet.Detents.Add(ZeroDetent);
            }
            bottomSheeetVm.IsSpiritOpend = false;
            bottomSheeetVm.IsSearchOpend = true;

            if (!bottomSheeet.IsLoaded)
            {
                bottomSheeet.ShowAsync();
            }

            bottomSheeet.SelectedDetent = bottomSheeet.Detents[0];

        }

        private async void GeoViewTapped(object sender, Esri.ArcGISRuntime.Maui.GeoViewInputEventArgs e)
        {

            MapPoint tappedLocation = (MapPoint)e.Location.NormalizeCentralMeridian();

            double min_dist = double.MaxValue;
            MapSpirit closest = null;
            foreach (var spirit in Spirits)
            {
                var dist = tappedLocation.Distance(spirit.mapPoint);
                if (dist < min_dist)
                {
                    min_dist = dist;
                    closest = spirit;
                }
            }

            if (bottomSheeet.Detents.Count < 3)
            {
                var ZeroDetent = new RatioDetent();
                ZeroDetent.Ratio = 0.1f;
                bottomSheeet.Detents.Add(ZeroDetent);
            }
            foreach (var spirit in Spirits)
            {
                spirit.pinGraphic.IsVisible = true;
            }
            if (min_dist / MainMapView.MapScale * 100 < 1)
            {
                bottomSheeetVm.IsSpiritOpend = true;
                bottomSheeetVm.IsSearchOpend = false;

                foreach(var spirit in Spirits)
                {
                    spirit.markerSymbol.Height = 40;
                    spirit.markerSymbol.Width = 40;
                    spirit.polygonGraphic.IsVisible = false;
                }

                closest.polygonGraphic.IsVisible = true;
                closest.markerSymbol.Height = 60;
                closest.markerSymbol.Width = 60;
                closest.pinGraphic.ZIndex = MapSpirit.maxzindex + 1;

                bottomSheeetVm.SetSelectedSpirit(closest);
                if (!bottomSheeet.IsLoaded)
                {
                    bottomSheeet.ShowAsync();
                }

                bottomSheeet.SelectedDetent = bottomSheeet.Detents[0];
                MapPoint mapPoint = new MapPoint(closest.mapPoint.X, closest.mapPoint.Y - 200000, closest.mapPoint.SpatialReference);
                Viewpoint viewpoint = new Viewpoint(mapPoint, 5000000);
                await this.MainMapView.SetViewpointAsync(viewpoint, TimeSpan.FromSeconds(0.5));
            }
            else
            {
                foreach (var spirit in Spirits)
                {
                    spirit.markerSymbol.Height = 40;
                    spirit.markerSymbol.Width = 40;
                }
                foreach( var habitat in Habitats){
                    habitat.PolygonGraphic.IsVisible = false;
                }
                bottomSheeet.SelectedDetent = bottomSheeet.Detents[2];
            }
        }



        private async Task CreateSpiritMarkers()
        {
            var SpiritsGraphicOverlay = new GraphicsOverlay();
            polygonOverlay = new GraphicsOverlay();
            GraphicsOverlayCollection overlays = MainMapView.GraphicsOverlays;
            overlays.Add(polygonOverlay);
            overlays.Add(SpiritsGraphicOverlay);

            MainMapView.GraphicsOverlays = overlays;

            foreach (var spirit in Spirits)
            {
                string localFilePath = Path.Combine(dataDirectory, "MarkerImage_" + spirit.Id.ToString() + "_.png");

                using FileStream localFileStream = File.OpenRead(localFilePath);

                PictureMarkerSymbol pinSymbol = await PictureMarkerSymbol.CreateAsync(localFileStream);
                localFileStream.Close();
                spirit.markerSymbol = pinSymbol;
                pinSymbol.Width = 40;
                pinSymbol.Height = 40;

                Graphic pinGraphic = new Graphic(spirit.mapPoint, pinSymbol);
                spirit.pinGraphic = pinGraphic;
                MapSpirit.maxzindex = Math.Max(pinGraphic.ZIndex, MapSpirit.maxzindex);

                try {
                    spirit.polygonGraphic = Habitats.Where(h => h.Id == spirit.Habitats[0].Id).First().PolygonGraphic;
                }
                catch
                {
                    spirit.polygonGraphic = new Graphic();
                }
                SpiritsGraphicOverlay.Graphics.Add(pinGraphic);
            }
            foreach(var habitat in Habitats)
            {
                polygonOverlay.Graphics.Add(habitat.PolygonGraphic);
            }

        }
    }
}
