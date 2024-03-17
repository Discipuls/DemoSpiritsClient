using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.UI;
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
        MapSpirit selected;
        [ObservableProperty]
        bool isSearchOpend;
        [ObservableProperty]
        bool isSpiritOpend;
        [ObservableProperty]
        String ffont;

        public MapView mapView { get; set; }

        public List<MapSpirit> spiritList = new List<MapSpirit>();
        public List<Graphic> regions = new List<Graphic>();

        [ObservableProperty]
        ObservableCollection<String> searchResults = new ObservableCollection<String>();
        public void SetSelectedSpirit(MapSpirit spirit)
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
            if (query == null) {
                return;
            }
            query = query.ToLower();
            if(query != null)
            {
                SearchResults = new ObservableCollection<String>();

                var regionsNames = new ObservableCollection<String>();
                regionsNames.Add("Віцебская вобласць");
                regionsNames.Add("Гомельская вобласць");
                regionsNames.Add("Гродненская вобласць");
                regionsNames.Add("Брэсцкая вобласць");
                regionsNames.Add("Мінская вобласць");
                regionsNames.Add("Магілёўская вобласць");

                foreach (var spirit in spiritList)
                {
                    if (spirit.Name.ToLower().Contains(query))
                    {
                        SearchResults.Add(spirit.Name);
                    }
                }   


                foreach (var spirit in regionsNames)
                {
                    if (spirit.ToLower().Contains(query))
                    {
                        SearchResults.Add(spirit);
                    }
                }
            }
           
        }
        [RelayCommand]
        async Task Tap(string s)
        {

            var regionsNames = new ObservableCollection<String>();
            regionsNames.Add("Віцебская вобласць");
            regionsNames.Add("Гомельская вобласць");
            regionsNames.Add("Гродненская вобласць");
            regionsNames.Add("Брэсцкая вобласць");
            regionsNames.Add("Мінская вобласць");
            regionsNames.Add("Магілёўская вобласць");
            bool tapRegion = false;
            Graphic curRegion = new Graphic();

            for (int i = 0;i < regionsNames.Count;i++)
            {
                if (regionsNames[i] == s)
                {
                    tapRegion = true;
                    if(i == 0)
                    {
                        curRegion = regions[1];
                    }else if (i == 1)
                    {
                        curRegion = regions[3];
                    }else if (i == 2)
                    {
                        curRegion = regions[0];
                    }else if (i == 3)
                    {
                        curRegion = regions[2];
                    }else if (i == 4)
                    {
                        curRegion = regions[5];
                    }else
                    {
                        curRegion = regions[4];
                    }

                    
                }

            }
            if (tapRegion)
            {
                foreach (var reg in regions)
                {
                    reg.IsVisible = false;
                }
                curRegion.IsVisible = true;
                foreach (var spirit in spiritList)
                {
                    if (spirit.mapPoint.Within(curRegion.Geometry))
                    {
                        spirit.pinGraphic.IsVisible = true;
                    }
                    else
                    {
                        spirit.pinGraphic.IsVisible = false;
                    }
                }
                SearchResults = new ObservableCollection<string>();

                foreach (var spirit in spiritList)
                {
                    if (spirit.pinGraphic.IsVisible)
                    {
                        searchResults.Add(spirit.Name);
                    }
                }
            }
            else
            {
                foreach (var reg in regions)
                {
                    reg.IsVisible = false;
                }
                foreach (var spirit in spiritList)
                {
                    if (spirit.Name == s)
                    {
                        Selected = spirit;
                        MapPoint mapPoint = new MapPoint(spirit.mapPoint.X, spirit.mapPoint.Y - 200000, spirit.mapPoint.SpatialReference);
                        Viewpoint viewpoint = new Viewpoint(mapPoint, 5000000);
                        await mapView.SetViewpointAsync(viewpoint, TimeSpan.FromSeconds(0.5));



                        foreach (var sp in spiritList)
                        {
                            sp.markerSymbol.Height = 40;
                            sp.markerSymbol.Width = 40;
                            sp.polygonGraphic.IsVisible = false;
                        }
                        spirit.polygonGraphic.IsVisible = true;
                        spirit.markerSymbol.Height = 60;
                        spirit.markerSymbol.Width = 60;
                        spirit.pinGraphic.ZIndex = MapSpirit.maxzindex + 1;
                        break;
                    }
                }
                IsSearchOpend = false;
                IsSpiritOpend = true;
            }
        }
    }
}
