using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using SpiritsClassLibrary.DTOs.SpiritDTOs;
using SpiritsClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritsFirstTry.DTOs
{
    public class UpdateMapSpiritDTO : UpdateSpiritDTO
    {
        public string CardImageRoute { get; set; }
        public string MarkerImageRoute { get; set; }
        public MapPoint mapPoint { get; set; }
        public PictureMarkerSymbol markerSymbol { get; set; }
        public Graphic pinGraphic { get; set; }
        public List<Habitat>? Habitats { get; set; } = new List<Habitat>();
        public List<UpdateHabitatMapDTO> HabitatsDTOs { get; set; } = new List<UpdateHabitatMapDTO>();

        private MarkerPoint? _markerPoint;
        public new MarkerPoint? MarkerLocation
        {
            get
            {
                return _markerPoint;
            }
            set
            {
                _markerPoint = value;
                mapPoint = (MapPoint)new MapPoint((double)_markerPoint.Latitude, (double)_markerPoint.Longitude,
                    SpatialReferences.Wgs84).Project(SpatialReferences.WebMercator);
            }
        }

    }
}