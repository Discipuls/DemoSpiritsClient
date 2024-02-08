using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritsFirstTry.ViewModels
{
    public partial class BottomSheetViewModel : ObservableObject
    {
        [ObservableProperty]
        Spirit selected;
        [ObservableProperty]
        bool isSearchOpend;
        [ObservableProperty]
        bool isSpiritOpend;
        [ObservableProperty]
        String ffont;

        public MapView mapView { get; set; }

        public List<Spirit> spiritList = new List<Spirit>();

        [ObservableProperty]
        ObservableCollection<String> searchResults = new ObservableCollection<String>();
        public void SetSelectedSpirit(Spirit spirit)
        {
            Selected = spirit;
        }

        public void SetIsSearchOpend(bool b)
        {
            isSearchOpend = b;
            isSpiritOpend = !b;
        }
        [RelayCommand]
        public void performSearch(string query)
        {
            if(query != null)
            {
                SearchResults = new ObservableCollection<String>();
                foreach (var spirit in spiritList)
                {
                    if (spirit.Name.Contains(query))
                    {
                        SearchResults.Add(spirit.Name);
                    }
                }
            }
           
        }
        [RelayCommand]
        async Task Tap(string s)
        {
            foreach (var spirit in spiritList)
            {
                if (spirit.Name == s)
                {
                    Selected = spirit;
                    MapPoint mapPoint = new MapPoint(spirit.mapPoint.X, spirit.mapPoint.Y - 200000, spirit.mapPoint.SpatialReference);
                    Viewpoint viewpoint = new Viewpoint(mapPoint, 5000000);
                    await mapView.SetViewpointAsync(viewpoint, TimeSpan.FromSeconds(0.5));
                    break;
                }
            }
            IsSearchOpend = false;
            IsSpiritOpend = true;
            return;
        }
    }
}
