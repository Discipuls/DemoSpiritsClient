using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using SpiritsClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritsFirstTry.Models
{
    public class MapHabitat : Habitat
    {
        private List<BorderPoint>? _border;
        public new List<BorderPoint>? Border
        {
            get
            {
                return _border;
            }
            set
            {
                if (value == null)
                {
                    _border = new List<BorderPoint>();
                }
                else
                {
                    _border = value;
                }

                var polygonPoints = new List<MapPoint>();
                foreach(var point in _border)
                {
                    polygonPoints.Add(
                                    new MapPoint((double)point.Latitude,
                                                (double)point.Longitude, 
                                                SpatialReferences.Wgs84));
                }
                var polygon = new Polygon(polygonPoints);
                var polygonSymbolOutline = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Orange, 2.0);
                var polygonFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(50, System.Drawing.Color.RebeccaPurple), polygonSymbolOutline);

                _polygonGraphic = new Graphic(polygon, polygonFillSymbol);

                _polygonGraphic.IsVisible = false;
            }
        } 

        private Graphic _polygonGraphic;
        public Graphic PolygonGraphic { get { return _polygonGraphic; } }
    }
}
