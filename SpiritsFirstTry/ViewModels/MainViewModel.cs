using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Microsoft.Maui.Controls;
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
        IRestService restService;
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
        public Image adminImage;


        public MainViewModel(ISpiritService spiritService, IHabitatService habitatService, IRestService restService) {
            _spiritService = spiritService;
            _habitatService = habitatService;
            this.restService = restService;


            bottomSheeetVm = new BottomSheetViewModel();

            bottomSheeet = new BottomSheetView(bottomSheeetVm);
            bottomSheeet.IsCancelable = false;
           // bottomSheeet.Detents.Add(new MediumDetent());
            var rt08 = new RatioDetent();
            rt08.Ratio = 0.8f;
            var rt025 = new RatioDetent();
            rt025.Ratio = 0.3f;
            bottomSheeet.Detents.Add(rt025);

            bottomSheeet.Detents.Add(rt08);
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
        public async Task SetupAdminImage(Image image)
        {
            this.adminImage = image;
            if(!(await restService.GetIsAdminAsync()))
            {
                image.IsVisible = false;
            }
            else
            {
                image.IsVisible = true;
            }
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
                await progressBar.ProgressTo(0.0, 50, Easing.Linear);

                Habitats = await _habitatService.LoadHabitats(progressBar);
                await progressBar.ProgressTo(0.25, 50, Easing.Linear);

                Spirits = await _spiritService.LoadSpirits(progressBar, Habitats);

                await progressBar.ProgressTo(1, 50, Easing.Linear);
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
                    spirit.markerSymbol.Height = 40/1.3;
                    spirit.markerSymbol.Width = 40/1.3;
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

                try
                {
                    var habitat = Habitats.Where(h => h.Id == closest.Habitats[0].Id).First();
                    await MainMapView.SetViewpointGeometryAsync(habitat.PolygonGraphic.Geometry,50);
                    MapPoint mapPoint = new MapPoint(closest.mapPoint.X, closest.mapPoint.Y - MainMapView.MapScale/50, closest.mapPoint.SpatialReference);
                    Viewpoint viewpoint = new Viewpoint(mapPoint);
                    await MainMapView.SetViewpointAsync(viewpoint, TimeSpan.FromSeconds(3));
                }
                catch
                {
                    MapPoint mapPoint = new MapPoint(closest.mapPoint.X, closest.mapPoint.Y - 100000, closest.mapPoint.SpatialReference);
                    Viewpoint viewpoint = new Viewpoint(mapPoint,3000000);
                    await MainMapView.SetViewpointAsync(viewpoint, TimeSpan.FromSeconds(0.5));

                }

            }
            else
            {
                foreach (var spirit in Spirits)
                {
                    spirit.markerSymbol.Height = 40/1.3;
                    spirit.markerSymbol.Width = 40/1.3;
                }
                foreach( var habitat in Habitats){
                    habitat.PolygonGraphic.IsVisible = false;
                }
                bottomSheeet.SelectedDetent = bottomSheeet.Detents[2];
                await MainMapView.SetViewpointScaleAsync(6000000);
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
                pinSymbol.Width = 40/1.3;
                pinSymbol.Height = 40/1.3;

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
