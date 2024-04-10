using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using SpiritsFirstTry.DTOs;
using System.Reflection;

namespace SpiritsFirstTry.ViewModels
{
    public enum MapAction { Drag, Action }
    public partial class EditMarkerViewModel : ObservableObject
    {
        [ObservableProperty]
        UpdateMapSpiritDTO spiritDTO;
        public MapView MarkerMapView { get; set; }
        public MapAction mapAction { get; set; }
        public MapView spiritMapView {get;set ;}
        public string dataDirectory;

        public EditMarkerViewModel()
        {
            dataDirectory = FileSystem.AppDataDirectory;
        }

        public async Task SetupMap(MapView mapView)
        {
            MarkerMapView = mapView;
            MarkerMapView.Map = new Esri.ArcGISRuntime.Mapping.Map(BasemapStyle.OSMNavigationDark);

            MarkerMapView.Map.InitialViewpoint = new Viewpoint(spiritDTO.mapPoint, 10000000);

            await CreateSpiritMarker();
        }


        private async Task CreateSpiritMarker()
        {
            var SpiritsGraphicOverlay = new GraphicsOverlay();
            GraphicsOverlayCollection overlays = MarkerMapView.GraphicsOverlays;
            overlays.Add(SpiritsGraphicOverlay);
            MarkerMapView.GraphicsOverlays = overlays;
            PictureMarkerSymbol pinSymbol;
            try
            {
                string localFilePath = System.IO.Path.Combine(dataDirectory, "MarkerImage_" + SpiritDTO.Id.ToString() + "_.png");

                using FileStream localFileStream = File.OpenRead(localFilePath);
                pinSymbol = await PictureMarkerSymbol.CreateAsync(localFileStream);

            }
            catch
            {
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                Stream resourceStream = currentAssembly.GetManifestResourceStream("SpiritsFirstTry.Resources.Images.newspirit.png");
                pinSymbol = await PictureMarkerSymbol.CreateAsync(resourceStream);
            }


            SpiritDTO.markerSymbol = pinSymbol;
            pinSymbol.Width = 40/1.3;
            pinSymbol.Height = 40/1.3;

            Graphic pinGraphic = new Graphic(SpiritDTO.mapPoint, pinSymbol);
            SpiritsGraphicOverlay.Graphics.Add(pinGraphic);

        }

        [RelayCommand]
        public async Task MoveMarker()
        {
            var oldGraphics = MarkerMapView.GraphicsOverlays[0].Graphics[0];
            Esri.ArcGISRuntime.Geometry.Geometry newGeometry = await MarkerMapView.SketchEditor.StartAsync(oldGraphics.Geometry);
            oldGraphics.Geometry = newGeometry;
            SpiritDTO.pinGraphic.Geometry = newGeometry;
            SpiritDTO.mapPoint = (MapPoint)newGeometry;
        }

        [RelayCommand]
        public async Task Complete()
        {
            try
            {
                MarkerMapView.SketchEditor.CompleteCommand.Execute(this);

                Viewpoint viewpoint = new Viewpoint(SpiritDTO.pinGraphic.Geometry, spiritMapView.Scale);
                await spiritMapView.SetViewpointAsync(viewpoint);
            }
            catch
            {
                
            }

        }
    }
}
