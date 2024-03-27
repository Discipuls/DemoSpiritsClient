using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using SpiritsClassLibrary.DTOs.SpiritDTOs;
using SpiritsClassLibrary.Models;
using SpiritsFirstTry.AutoMappers;
using SpiritsFirstTry.DTOs;
using SpiritsFirstTry.Models;
using SpiritsFirstTry.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using The49.Maui.BottomSheet;

namespace SpiritsFirstTry.ViewModels
{
    public partial class SpiritUpdateViewModel : ObservableObject
    {
        private IMapper _mapper;
        private IRestService _restService;
        [ObservableProperty]
        List<SpiritType> types = new List<SpiritType>();
        [ObservableProperty]
        UpdateMapSpiritDTO spiritDTO;
        [ObservableProperty]
        List<MapHabitat> habitats = new List<MapHabitat>();
        [ObservableProperty]
        List<UpdateHabitatMapDTO> habitatsDTOs = new List<UpdateHabitatMapDTO>();
        public bool create = false;


        public List<Picker> ClassificationPicker { get; set; } = new List<Picker>();
        public List<Picker> HabitatPicker { get; set; } = new List<Picker>();

        public MapView SpiritMapView { get; set; }
        public SpiritUpdateViewModel(IMapper mapper, IRestService restService)
        {
            var t =  Enum.GetValues(typeof(SpiritType));
            foreach(int i in t)
            {
                types.Add((SpiritType)i);
            }
            _mapper = mapper;
            _restService = restService;
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

            string localFilePath; ;
            FileStream localFileStream;
            PictureMarkerSymbol pinSymbol;

            if (create)
            {
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                Stream resourceStream =  currentAssembly.GetManifestResourceStream( "SpiritsFirstTry.Resources.Images.newspirit.png");
                 pinSymbol = await PictureMarkerSymbol.CreateAsync(resourceStream);
            }
            else
            {
                localFilePath = Path.Combine(FileSystem.CacheDirectory, "MarkerImage_" + SpiritDTO.Id.ToString() + "_.png");
                localFileStream = File.OpenRead(localFilePath);
                pinSymbol = await PictureMarkerSymbol.CreateAsync(localFileStream);
            }


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
            try
            {
                Console.WriteLine("Click save!");
                var updateSpiritDTO = _mapper.Map<UpdateSpiritDTO>(spiritDTO);
                foreach (var habitat in spiritDTO.HabitatsDTOs)
                {
                    updateSpiritDTO.HabitatsIds.Add(habitat.Id);
                }

                using FileStream markerFileStream = File.OpenRead(spiritDTO.MarkerImageRoute);
                var markerBuffer = new byte[markerFileStream.Length];
                await markerFileStream.ReadAsync(markerBuffer, 0, markerBuffer.Length);
                updateSpiritDTO.MarkerImage = markerBuffer;

                using FileStream cardFileStream = File.OpenRead(spiritDTO.CardImageRoute);
                var cardBuffer = new byte[cardFileStream.Length];
                await cardFileStream.ReadAsync(cardBuffer, 0, cardBuffer.Length);
                updateSpiritDTO.CardImage = cardBuffer;

                //TODO check chenges
                //TODO add and remove classification on page
                //TODO add and remove habitats on page

                updateSpiritDTO.Classification = ClassificationPicker.Select(p => (SpiritType)p.SelectedIndex).ToList();
                updateSpiritDTO.HabitatsIds = HabitatPicker.Select(p => habitatsDTOs[p.SelectedIndex].Id).ToList();
                updateSpiritDTO.MarkerLocation.Longitude = ((MapPoint)GeometryEngine.Project(spiritDTO.mapPoint, SpatialReferences.Wgs84)).Y;
                updateSpiritDTO.MarkerLocation.Latitude = ((MapPoint)GeometryEngine.Project(spiritDTO.mapPoint, SpatialReferences.Wgs84)).X;

                if (!create)
                {
                    await _restService.UpdateSpiritAsync(updateSpiritDTO);
                }
                else
                {
                    var createSpiritDTO = _mapper.Map<CreateSpiritDTO>(updateSpiritDTO);
                    await _restService.CreateSpiritAsync(createSpiritDTO);
                }


                var imageManagerDiskCache = Path.Combine(FileSystem.CacheDirectory, "image_manager_disk_cache");

                if (Directory.Exists(imageManagerDiskCache))
                {
                    foreach (var imageCacheFile in Directory.EnumerateFiles(imageManagerDiskCache))
                    {
                        Debug.WriteLine($"Deleting {imageCacheFile}");
                        File.Delete(imageCacheFile);
                    }
                }
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch(Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Smth went wrong", ex.Message, "Ok");
            }
        }

        public async Task EditMarkerImage(Image markerImage)
        {

            var result =  await MediaPicker.PickPhotoAsync();
            if(result.ContentType != "image/png")
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please select .png image", "OK");
            }
            else
            {

            //    File.Delete(markerFilePath);
                using FileStream localFileStream = File.OpenRead(result.FullPath);
                spiritDTO.MarkerImageRoute = result.FullPath;
            /*    var buffer = new byte[localFileStream.Length];
                await localFileStream.ReadAsync(buffer, 0, buffer.Length);*/
                localFileStream.Close();
                markerImage.Source = result.FullPath;
                /*
                 *                 string markerFilePath = Path.Combine(FileSystem.CacheDirectory, "MarkerImage_" + spiritDTO.Id.ToString() + "_.png");

                                MemoryStream markerImageMS = new MemoryStream(spiritDTO.MarkerImage);
                                using FileStream markerFileStream = File.OpenWrite(markerFilePath);
                                await markerImageMS.CopyToAsync(markerFileStream);
                                markerFileStream.Close();*/ //TODO move to save

            }
        }

        public async Task EditCardImage(Image cardImage)
        {

            var result = await MediaPicker.PickPhotoAsync();
            if (result.ContentType != "image/png")
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please select .png image", "OK");
            }
            else
            {
                using FileStream localFileStream = File.OpenRead(result.FullPath);
                spiritDTO.CardImageRoute = result.FullPath;
/*                var buffer = new byte[localFileStream.Length];
                await localFileStream.ReadAsync(buffer, 0, buffer.Length);*/
                localFileStream.Close();
                cardImage.Source = result.FullPath;
            }
        }
        public async Task EditMarker()
        {
            EditMarkerViewModel viewModel = new EditMarkerViewModel();
            viewModel.SpiritDTO = spiritDTO;
            viewModel.spiritMapView = SpiritMapView;
            ClassificationPicker.Clear();
            HabitatPicker.Clear();
            await Application.Current.MainPage.Navigation.PushAsync(new EditMarkerPage(viewModel));

        }
    }


}
