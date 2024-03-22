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
using SpiritsFirstTry.Services;
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
        IRestService _restService;
        IMapper _mapper;

        public MapView MainMapView { get; set; }
        public ProgressBar progressBar { get; set; }
        private double currentProgress { get; set; }
        public static List<MapSpirit> Spirits { get; set; }
        public static List<MapHabitat> Habitats {  get; set; }
        BottomSheetView bottomSheeet;
        BottomSheetViewModel bottomSheeetVm;
        public GraphicsOverlay polygonOverlay;
        public static List<Graphic> regions = new List<Graphic>();


        public MainViewModel(IRestService restService, IMapper mapper) {


            _restService = restService;
            _mapper = mapper;


            bottomSheeetVm = new BottomSheetViewModel();
            bottomSheeetVm.spiritList = Spirits;
            bottomSheeetVm.regions = regions;
            bottomSheeet = new BottomSheetView(bottomSheeetVm);
            bottomSheeet.IsCancelable = false;
            bottomSheeet.Detents.Add(new MediumDetent());
            bottomSheeet.Detents.Add(new FullscreenDetent());

            //     bottomSheeet.SelectedDetent = bottomSheeet.Detents[2];

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

        private async Task Initialize()
        {

            try
            {
                currentProgress = 0;
                await progressBar.ProgressTo(0.0, 500, Easing.Linear);
                await LoadSpirits();
                await progressBar.ProgressTo(0.75, 500, Easing.Linear);
                await LoadRegions();
                await progressBar.ProgressTo(1, 500, Easing.Linear);
                progressBar.IsVisible = false;

            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.ToString(), "OK");
            }


            try
            {
                await CreateSpiritMarkers();
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.ToString(), "OK");
            }


            try
            {
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


            MapPoint MinskLocation = new MapPoint(27.542744, 53.897867,  SpatialReferences.Wgs84);


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

                Console.WriteLine(closest.Name);
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
                    //spirit.polygonGraphic.IsVisible = false;
                }
                foreach( var polygon in regions){
                    polygon.IsVisible = false;
                }
                bottomSheeet.SelectedDetent = bottomSheeet.Detents[2];
            }
        }

        private async Task<List<GetSpiritBasicsDTO>> LoadJSONSpirits()
        {
            string localFilePath = Path.Combine(FileSystem.CacheDirectory, "Spirits.json");
            using FileStream fileStream = File.OpenRead(localFilePath);
            var buffer = new byte[fileStream.Length];
            await fileStream.ReadAsync(buffer, 0, buffer.Length);
            string s = Encoding.Default.GetString(buffer);

            var JSONSpirits = JsonSerializer.Deserialize<List<GetSpiritBasicsDTO>>(s);
            return JSONSpirits;
        }

        private async Task SaveSpiritsToJson(List<GetSpiritBasicsDTO> spirits)
        {
            var json = JsonSerializer.Serialize(spirits);
            var spiritsMS = new MemoryStream();
            var spiritsSW = new StreamWriter(spiritsMS);
            spiritsSW.Write(json);
            spiritsSW.Flush();
            spiritsMS.Position = 0;

            string localFilePath = Path.Combine(FileSystem.CacheDirectory, "Spirits.json");
            using FileStream fileStream = File.OpenWrite(localFilePath);

            await spiritsMS.CopyToAsync(fileStream);
        }

        private async Task<GetSpiritDTO> UpdateMissedSpirit(GetSpiritBasicsDTO spiritBasicsDTO)
        {
            var spirit = await _restService.GetSpiritAsync(spiritBasicsDTO.Id);

            MemoryStream markerImageMS = new MemoryStream(spirit.MarkerImage);
            string markerFilePath = Path.Combine(FileSystem.CacheDirectory, "MarkerImage_" + spirit.Id.ToString() + "_.png");
            using FileStream markerFileStream = File.OpenWrite(markerFilePath);
            await markerImageMS.CopyToAsync(markerFileStream);

            MemoryStream cardImageMS = new MemoryStream(spirit.CardImage);
            string cardFilePath = Path.Combine(FileSystem.CacheDirectory, "CardImage_" + spirit.Id.ToString() + "_.png");
            using FileStream cardFileStream = File.OpenWrite(cardFilePath);
            await cardImageMS.CopyToAsync(cardFileStream);

            return spirit;
        }

        public async Task LoadSpirits()
        {
            bool APISpiritsAvailible = true;
            bool JSONSpiritsAvailible = true;

            List<GetSpiritBasicsDTO> APISpirits = new List<GetSpiritBasicsDTO>();
            try
            {
                APISpirits = await _restService.GetAllSpiritsAsync();
            }catch(Exception ex)
            {
                APISpiritsAvailible = false;
                await Application.Current.MainPage.DisplayAlert("Error", ex.ToString(), "OK");

            }
            List<GetSpiritBasicsDTO> JSONSpirits = new List<GetSpiritBasicsDTO>();
            try
            {
                JSONSpirits = await LoadJSONSpirits();
            }catch (Exception ex)
            {
                JSONSpiritsAvailible = false;
                await Application.Current.MainPage.DisplayAlert("Error", ex.ToString(), "OK");

            }
            List<GetSpiritBasicsDTO> resultSpiritsBasicsDTOs = new List<GetSpiritBasicsDTO>();

            if(APISpiritsAvailible && JSONSpiritsAvailible)
            {
                foreach (var apiSpirit in APISpirits)
                {
                    var jsonSpirit = JSONSpirits.Where(s => s.Id == apiSpirit.Id).FirstOrDefault();
                    if (jsonSpirit != null)
                    {
                        if (jsonSpirit.LastUpdated < apiSpirit.LastUpdated)
                        {
                            var newSpirit = await UpdateMissedSpirit(jsonSpirit);
                        }
                    }
                    else
                    {
                        var newSpirit = await UpdateMissedSpirit(apiSpirit);
                    }

                    resultSpiritsBasicsDTOs.Add(apiSpirit);
                    currentProgress += (0.75 / APISpirits.Count);
                    await progressBar.ProgressTo(currentProgress, 500, Easing.Linear);
                }

                SaveSpiritsToJson(resultSpiritsBasicsDTOs);
            }
            else if(JSONSpiritsAvailible)
            {
                resultSpiritsBasicsDTOs = JSONSpirits;
            }else if (APISpiritsAvailible)
            {
                foreach(var s in APISpirits)
                {
                    await UpdateMissedSpirit(s);
                    currentProgress += (0.75 / APISpirits.Count);
                    await progressBar.ProgressTo(currentProgress, 500, Easing.Linear);
                }



                resultSpiritsBasicsDTOs = APISpirits;
                SaveSpiritsToJson(resultSpiritsBasicsDTOs);
            }
            else
            {
                throw new Exception("No sources to retrieve spirits available!");
            }

             Spirits = resultSpiritsBasicsDTOs.Select(s => _mapper.Map<MapSpirit>(s)).ToList();

        }


        private async Task<List<GetHabitatDTO>> LoadJSONHabitats()
        {
            string localFilePath = Path.Combine(FileSystem.CacheDirectory, "Habitats.json");
            using FileStream fileStream = File.OpenRead(localFilePath);
            var buffer = new byte[fileStream.Length];
            await fileStream.ReadAsync(buffer, 0, buffer.Length);
            string s = Encoding.Default.GetString(buffer);

            var JSONSpirits = JsonSerializer.Deserialize<List<GetHabitatDTO>>(s);
            return JSONSpirits;
        }


        private async Task SaveHabitatsToJson(List<GetHabitatDTO> habitats)
        {
            var json = JsonSerializer.Serialize(habitats);
            var habitatsMS = new MemoryStream();
            var habitatsSW = new StreamWriter(habitatsMS);
            habitatsSW.Write(json);
            habitatsSW.Flush();
            habitatsMS.Position = 0;

            string localFilePath = Path.Combine(FileSystem.CacheDirectory, "Habitats.json");
            using FileStream fileStream = File.OpenWrite(localFilePath);

            await habitatsMS.CopyToAsync(fileStream);
        }
        public async Task LoadRegions()
        {
            bool APIHabitatsAvailible = true;
            bool JSONHabitatsAvailible = true;
            var APIHabitats = new List<GetHabitatDTO>();
            var JSONHabitats = new List<GetHabitatDTO>();
            try
            {
                APIHabitats = await _restService.GetAllHabitatsAsync();
            }
            catch (Exception ex)
            {
                APIHabitatsAvailible = false;
                await Application.Current.MainPage.DisplayAlert("Error", ex.ToString(), "OK");
            }

            try
            {
                JSONHabitats = await LoadJSONHabitats();
            }
            catch (Exception ex)
            {
                JSONHabitatsAvailible = false;
                await Application.Current.MainPage.DisplayAlert("Error", ex.ToString(), "OK");
            }

            var resultHabitatsDTOs = new List<GetHabitatDTO>();

            if (APIHabitatsAvailible && JSONHabitatsAvailible)
            {
                resultHabitatsDTOs = APIHabitats;
                SaveHabitatsToJson(resultHabitatsDTOs);
            }
            else if (JSONHabitatsAvailible)
            {
                resultHabitatsDTOs = JSONHabitats;
            }
            else if (APIHabitatsAvailible)
            {
                resultHabitatsDTOs = APIHabitats;
                SaveHabitatsToJson(resultHabitatsDTOs);
            }
            else
            {
                throw new Exception("No sources to retrieve habitats available!");
            }

            Habitats = resultHabitatsDTOs.Select(h => _mapper.Map<MapHabitat>(h)).ToList();
            regions = Habitats.Select(h => h.PolygonGraphic).ToList();

        }

        private async Task CreateSpiritMarkers()
        {
            var SpiritsGraphicOverlay = new GraphicsOverlay();
            polygonOverlay = new GraphicsOverlay();
            GraphicsOverlayCollection overlays = MainMapView.GraphicsOverlays;
            overlays.Add(polygonOverlay);
            overlays.Add(SpiritsGraphicOverlay);

            MainMapView.GraphicsOverlays = overlays;

            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            foreach (var spirit in Spirits)
            {
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, "MarkerImage_" + spirit.Id.ToString() + "_.png");

                // using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenRead(localFilePath);

                PictureMarkerSymbol pinSymbol = await PictureMarkerSymbol.CreateAsync(localFileStream);
                spirit.markerSymbol = pinSymbol;
                pinSymbol.Width = 40;
                pinSymbol.Height = 40;

                Graphic pinGraphic = new Graphic(spirit.mapPoint, pinSymbol);
                spirit.pinGraphic = pinGraphic;
                MapSpirit.maxzindex = Math.Max(pinGraphic.ZIndex, MapSpirit.maxzindex);
               // polygonOverlay.Graphics.Add(spirit.polygonGraphic);
                foreach(var region in regions)
                {
                    region.Geometry = region.Geometry.Project(spirit.mapPoint.SpatialReference);
                    if (spirit.mapPoint.Within(region.Geometry))
                    {
                        spirit.polygonGraphic = region;
                        break;
                    }
                    else
                    {
                        spirit.polygonGraphic = new Graphic();
                    }
                }
                SpiritsGraphicOverlay.Graphics.Add(pinGraphic);
            }
            foreach (var polygon in regions)
            {
                polygonOverlay.Graphics.Add(polygon);
            }
        }
    }
}
