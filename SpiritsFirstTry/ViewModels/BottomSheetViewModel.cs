using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.UI;
using SpiritsFirstTry.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        public Image CurrentCardImage { get; set; }

        public MapView mapView { get; set; }

        public List<MapSpirit> Spirits = new List<MapSpirit>();
        public List<MapHabitat> Habitats = new List<MapHabitat>();

        public string dataDirectory;

        public BottomSheetViewModel()
        {
            dataDirectory = FileSystem.AppDataDirectory;
        }

        [ObservableProperty]
        ObservableCollection<String> searchResults = new ObservableCollection<String>();
        public void SetSelectedSpirit(MapSpirit spirit)
        {
            Selected = spirit;

            string localFilePath = Path.Combine(dataDirectory, "CardImage_" + spirit.Id.ToString() + "_.png");
            CurrentCardImage.Source = localFilePath;
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


                foreach (var spirit in Spirits)
                {
                    if (spirit.Name.ToLower().Contains(query))
                    {
                        SearchResults.Add(spirit.Name);
                    }
                }   


                foreach (var habitat in Habitats)
                {
                    if (habitat.Name.ToLower().Contains(query))
                    {
                        SearchResults.Add(habitat.Name);
                    }
                }
            }
           
        }
        [RelayCommand]
        async Task Tap(string searchedName)
        {
            MapHabitat currentHabitat = Habitats.Where(h => h.Name == searchedName).FirstOrDefault();
           

            if (currentHabitat != null)
            {

                foreach (var habitat in Habitats)
                {
                    habitat.PolygonGraphic.IsVisible = false;
                }
                currentHabitat.PolygonGraphic.IsVisible = true;
                bool moved = false;
                foreach (var spirit in Spirits)
                {
                    if (spirit.Habitats.Where(h => h.Id == currentHabitat.Id).Count() != 0)
                    {

                        spirit.pinGraphic.IsVisible = true;
                        if (!moved)
                        {
                            await mapView.SetViewpointGeometryAsync(currentHabitat.PolygonGraphic.Geometry, 70);
                            MapPoint mapPoint = new MapPoint(spirit.mapPoint.X, spirit.mapPoint.Y - mapView.MapScale / 20, spirit.mapPoint.SpatialReference);
                            Viewpoint viewpoint = new Viewpoint(mapPoint);
                            await mapView.SetViewpointAsync(viewpoint, TimeSpan.FromSeconds(3));

                            moved = true;
                        }

                    }
                    else
                    {
                        spirit.pinGraphic.IsVisible = false;
                    }
                }
                SearchResults = new ObservableCollection<string>();

                foreach (var spirit in Spirits)
                {
                    if (spirit.pinGraphic.IsVisible)
                    {
                        searchResults.Add(spirit.Name);
                    }
                }
            }
            else
            {
                foreach (var habitat in Habitats)
                {
                    habitat.PolygonGraphic.IsVisible = false;
                    
                }
                foreach (var spirit in Spirits)
                {
                    if (spirit.Name == searchedName)
                    {
                        Selected = spirit;
                        string localFilePath = Path.Combine(dataDirectory, "CardImage_" + spirit.Id.ToString() + "_.png");
                        CurrentCardImage.Source = localFilePath;
                        foreach (var sp in Spirits)
                        {
                            sp.markerSymbol.Height = 40 / 1.3;
                            sp.markerSymbol.Width = 40 / 1.3;
                            sp.polygonGraphic.IsVisible = false;
                        }
                        spirit.polygonGraphic.IsVisible = true;
                        spirit.markerSymbol.Height = 60;
                        spirit.markerSymbol.Width = 60;
                        spirit.pinGraphic.ZIndex = MapSpirit.maxzindex + 1;
                        var habitat = Habitats.Where(h => h.Id == spirit.Habitats[0].Id).First();
                        await mapView.SetViewpointGeometryAsync(habitat.PolygonGraphic.Geometry, 50);
                        MapPoint mapPoint = new MapPoint(spirit.mapPoint.X, spirit.mapPoint.Y - mapView.MapScale / 30, spirit.mapPoint.SpatialReference);
                        Viewpoint viewpoint = new Viewpoint(mapPoint);
                        await mapView.SetViewpointAsync(viewpoint, TimeSpan.FromSeconds(3));




                        break;
                    }
                }
                IsSearchOpend = false;
                IsSpiritOpend = true;
            }
        }
    }
}
