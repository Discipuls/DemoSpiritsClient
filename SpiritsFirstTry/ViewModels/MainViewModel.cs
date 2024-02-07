using CommunityToolkit.Mvvm.ComponentModel;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map = Esri.ArcGISRuntime.Mapping.Map;

namespace SpiritsFirstTry.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public MapView MainMapView { get; set; }
        public MainViewModel() {
        }

        public void SetupMap(MapView mapView)
        {
            MainMapView = mapView;
            MainMapView.Map = new Map(BasemapStyle.OSMNavigationDark);

            var MinskPoint = new MapPoint(27.542744, 53.897867, SpatialReferences.Wgs84);

            MainMapView.Map.InitialViewpoint = new Viewpoint(MinskPoint, 10000000);
        }
    }
}
