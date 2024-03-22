using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using SpiritsClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritsFirstTry
{
    //public enum SpiritType { Forest, Water, Home, Dark, Field}
    public class PolygonSelection
    {
        public string name { get; set; }
        public Graphic polygonGraphic { get; set; }

    }
    public class MapSpirit : Spirit

    {
        public static int maxzindex = 0;

        public MapPoint mapPoint { get; set; }
        public string ClassificationString
        {
            get
            {
                string result = "";
                for (int i = 0; i < Classification.Count(); i++)
                {
                    if (i == 0)
                    {
                        if (Classification[i] == SpiritType.Forest)
                        {
                            result = "Лясны";
                        }
                        else if (Classification[i] == SpiritType.Home)
                        {
                            result = "Хатні";
                        }
                        else if (Classification[i] == SpiritType.Dark)
                        {
                            result = "Цёмны (злыдзень)";
                        }
                        else if (Classification[i] == SpiritType.Water)
                        {
                            result = "Водны";
                        }
                        else if (Classification[i] == SpiritType.Field)
                        {
                            result = "Палявы";
                        }
                    }
                    else
                    {
                        if (Classification[i] == SpiritType.Forest)
                        {
                            result += ", лясны";
                        }
                        else if (Classification[i] == SpiritType.Home)
                        {
                            result += ", хатні";
                        }
                        else if (Classification[i] == SpiritType.Dark)
                        {
                            result += ", цёмны (злыдзень)";
                        }
                        else if (Classification[i] == SpiritType.Water)
                        {
                            result += ", водны";
                        }
                        else if (Classification[i] == SpiritType.Field)
                        {
                            result += ", палявы";
                        }
                    }
                }

                return result;
            }
        }
        public string HabitatsString { get; set; }
        public PictureMarkerSymbol markerSymbol { get; set; }
        public Graphic pinGraphic { get; set; }
        public Graphic polygonGraphic {  get; set; }

        private MarkerPoint? _markerPoint;
        public new MarkerPoint? MarkerLocation
        {
            get
            {
                return _markerPoint;
            }
            set { 
                _markerPoint = value;
                mapPoint = (MapPoint)(new MapPoint((double)_markerPoint.Latitude, (double)_markerPoint.Longitude,
                    SpatialReferences.Wgs84).Project(SpatialReferences.WebMercator));
            }
        }


        public MapSpirit() { }

        public MapSpirit(MarkerPoint markerPoint, string name, string description, string image_name,
            List<SpiritType> classification, List<Habitat> habitats)
        {
            Name = name;
            Description = description;
            MarkerImageName = image_name;
            CardImageName = image_name.Insert(image_name.Length - 4, "t");
            mapPoint = (MapPoint)((new MapPoint(
                (double)markerPoint.Longitude, (double)markerPoint.Latitude, SpatialReferences.Wgs84))
                .Project(SpatialReferences.WebMercator));
            Classification = classification;
            Habitats = habitats;
            HabitatsString = habitats[0].Name;


            List<MapPoint> polygonPoints = new List<MapPoint>
            {new MapPoint(26.2005218,55.0035572, SpatialReferences.Wgs84),new MapPoint(26.0576995,54.9499634, SpatialReferences.Wgs84),new MapPoint(25.8874114,54.934187, SpatialReferences.Wgs84),new MapPoint(25.7006438,54.8045861, SpatialReferences.Wgs84),new MapPoint(25.739096,54.5218216, SpatialReferences.Wgs84),new MapPoint(25.5907805,54.4100823, SpatialReferences.Wgs84),new MapPoint(25.5468352,54.3044485, SpatialReferences.Wgs84),new MapPoint(25.7336028,54.2755921, SpatialReferences.Wgs84),new MapPoint(25.7775481,54.2113944, SpatialReferences.Wgs84),new MapPoint(25.6566985,54.127788, SpatialReferences.Wgs84),new MapPoint(25.5028899,54.1760431, SpatialReferences.Wgs84),new MapPoint(25.5907805,54.2563433, SpatialReferences.Wgs84),new MapPoint(25.4644378,54.2884197, SpatialReferences.Wgs84),new MapPoint(25.2282317,54.249925, SpatialReferences.Wgs84),new MapPoint(25.1643192,54.1959284, SpatialReferences.Wgs84),new MapPoint(25.0956547,54.144479, SpatialReferences.Wgs84),new MapPoint(24.9775517,54.1460878, SpatialReferences.Wgs84),new MapPoint(24.9628808,54.1698986, SpatialReferences.Wgs84),new MapPoint(24.9120691,54.1602507, SpatialReferences.Wgs84),new MapPoint(24.8379114,54.1449701, SpatialReferences.Wgs84),new MapPoint(24.7706201,54.1107696, SpatialReferences.Wgs84),new MapPoint(24.8454645,54.0285731, SpatialReferences.Wgs84),new MapPoint(24.7026422,53.9631834, SpatialReferences.Wgs84),new MapPoint(24.6930292,54.0164716, SpatialReferences.Wgs84),new MapPoint(24.513517,53.9572525, SpatialReferences.Wgs84),new MapPoint(24.4448525,53.8925601, SpatialReferences.Wgs84),new MapPoint(24.1894203,53.9524041, SpatialReferences.Wgs84),new MapPoint(23.9422279,53.9184491, SpatialReferences.Wgs84),new MapPoint(23.9120155,53.965332, SpatialReferences.Wgs84),new MapPoint(23.7994057,53.8974155, SpatialReferences.Wgs84),new MapPoint(23.6626759,53.9349089, SpatialReferences.Wgs84),new MapPoint(23.5143605,53.9478423, SpatialReferences.Wgs84),new MapPoint(23.5473195,53.7729031, SpatialReferences.Wgs84),new MapPoint(23.6352101,53.5613569, SpatialReferences.Wgs84),new MapPoint(23.8384572,53.2502609, SpatialReferences.Wgs84),new MapPoint(23.8902083,53.0736536, SpatialReferences.Wgs84),new MapPoint(23.9259138,52.8619208, SpatialReferences.Wgs84),new MapPoint(23.9478865,52.7756079, SpatialReferences.Wgs84),new MapPoint(24.1345506,52.805573, SpatialReferences.Wgs84),new MapPoint(24.2348009,52.7582268, SpatialReferences.Wgs84),new MapPoint(24.3885904,52.771795, SpatialReferences.Wgs84),new MapPoint(24.50944,52.7485256, SpatialReferences.Wgs84),new MapPoint(24.5808512,52.7626549, SpatialReferences.Wgs84),new MapPoint(24.5712381,52.8771825, SpatialReferences.Wgs84),new MapPoint(24.6591287,52.9111506, SpatialReferences.Wgs84),new MapPoint(24.641276,52.9517118, SpatialReferences.Wgs84),new MapPoint(24.7236734,52.9748725, SpatialReferences.Wgs84),new MapPoint(24.7621256,52.941782, SpatialReferences.Wgs84),new MapPoint(24.877482,52.9566758, SpatialReferences.Wgs84),new MapPoint(25.0010782,52.9268829, SpatialReferences.Wgs84),new MapPoint(25.0312906,52.8904415, SpatialReferences.Wgs84),new MapPoint(25.0642496,52.9277108, SpatialReferences.Wgs84),new MapPoint(25.0697428,52.8340626, SpatialReferences.Wgs84),new MapPoint(25.1507669,52.8589447, SpatialReferences.Wgs84),new MapPoint(25.2084451,52.8440171, SpatialReferences.Wgs84),new MapPoint(25.235911,52.8771825, SpatialReferences.Wgs84),new MapPoint(25.278483,52.8813263, SpatialReferences.Wgs84),new MapPoint(25.3334146,52.8315736, SpatialReferences.Wgs84),new MapPoint(25.363627,52.8647484, SpatialReferences.Wgs84),new MapPoint(25.3196817,52.8746959, SpatialReferences.Wgs84),new MapPoint(25.3169351,52.8937556, SpatialReferences.Wgs84),new MapPoint(25.3498941,52.9210875, SpatialReferences.Wgs84),new MapPoint(25.3169351,52.9376438, SpatialReferences.Wgs84),new MapPoint(25.4089456,52.9450922, SpatialReferences.Wgs84),new MapPoint(25.4597574,53.0071115, SpatialReferences.Wgs84),new MapPoint(25.5325418,53.0236349, SpatialReferences.Wgs84),new MapPoint(25.5929666,53.0607896, SpatialReferences.Wgs84),new MapPoint(25.6010904,53.1684804, SpatialReferences.Wgs84),new MapPoint(25.6230631,53.3082028, SpatialReferences.Wgs84),new MapPoint(25.7109537,53.3852654, SpatialReferences.Wgs84),new MapPoint(25.9032144,53.4032812, SpatialReferences.Wgs84),new MapPoint(26.1970987,53.3656031, SpatialReferences.Wgs84),new MapPoint(26.3701334,53.3574078, SpatialReferences.Wgs84),new MapPoint(26.5404215,53.3934554, SpatialReferences.Wgs84),new MapPoint(26.5733804,53.4883437, SpatialReferences.Wgs84),new MapPoint(26.4662637,53.6107301, SpatialReferences.Wgs84),new MapPoint(26.2959757,53.7165121, SpatialReferences.Wgs84),new MapPoint(26.2311377,53.8641505, SpatialReferences.Wgs84),new MapPoint(26.364347,53.8941035, SpatialReferences.Wgs84),new MapPoint(26.37396,53.9280783, SpatialReferences.Wgs84),new MapPoint(26.4247718,53.9547534, SpatialReferences.Wgs84),new MapPoint(26.4014258,54.0137013, SpatialReferences.Wgs84),new MapPoint(26.3258948,53.985449, SpatialReferences.Wgs84),new MapPoint(26.2943091,53.9676806, SpatialReferences.Wgs84),new MapPoint(26.3286414,54.0548346, SpatialReferences.Wgs84),new MapPoint(26.2187781,54.0338698, SpatialReferences.Wgs84),new MapPoint(26.1652198,54.0185427, SpatialReferences.Wgs84),new MapPoint(26.1336341,54.0951219, SpatialReferences.Wgs84),new MapPoint(26.1336341,54.1305424, SpatialReferences.Wgs84),new MapPoint(26.1363807,54.1611084, SpatialReferences.Wgs84),new MapPoint(26.0745826,54.1410017, SpatialReferences.Wgs84),new MapPoint(26.1089148,54.1055902, SpatialReferences.Wgs84),new MapPoint(26.0196509,54.1353701, SpatialReferences.Wgs84),new MapPoint(26.0181769,54.1592728, SpatialReferences.Wgs84),new MapPoint(26.0580023,54.21231, SpatialReferences.Wgs84),new MapPoint(26.0786017,54.1821836, SpatialReferences.Wgs84),new MapPoint(26.1026343,54.1845945, SpatialReferences.Wgs84),new MapPoint(26.1795386,54.2251572, SpatialReferences.Wgs84),new MapPoint(26.2433966,54.2255586, SpatialReferences.Wgs84),new MapPoint(26.306568,54.2030735, SpatialReferences.Wgs84),new MapPoint(26.3477667,54.21231, SpatialReferences.Wgs84),new MapPoint(26.4269197,54.2799514, SpatialReferences.Wgs84),new MapPoint(26.494211,54.3152138, SpatialReferences.Wgs84),new MapPoint(26.5257967,54.3736508, SpatialReferences.Wgs84),new MapPoint(26.6150606,54.421619, SpatialReferences.Wgs84),new MapPoint(26.6535127,54.5070232, SpatialReferences.Wgs84),new MapPoint(26.6601572,54.5951466, SpatialReferences.Wgs84),new MapPoint(26.6903696,54.6508017, SpatialReferences.Wgs84),new MapPoint(26.5983591,54.6452396, SpatialReferences.Wgs84),new MapPoint(26.5406809,54.7166941, SpatialReferences.Wgs84),new MapPoint(26.4549323,54.8217607, SpatialReferences.Wgs84),new MapPoint(26.4192268,54.8320448, SpatialReferences.Wgs84),new MapPoint(26.368415,54.8225519, SpatialReferences.Wgs84),new MapPoint(26.3450691,54.8533956, SpatialReferences.Wgs84),new MapPoint(26.3653523,54.8851165, SpatialReferences.Wgs84),new MapPoint(26.3083607,54.923805, SpatialReferences.Wgs84),new MapPoint(26.2630421,54.9470791, SpatialReferences.Wgs84),new MapPoint(26.243816,54.9857081, SpatialReferences.Wgs84),new MapPoint(26.212917,55.0002829, SpatialReferences.Wgs84),new MapPoint(26.2005218,55.0035572, SpatialReferences.Wgs84)
            };
            var mahouRivieraPolygon = new Polygon(polygonPoints);

            // Create a fill symbol to display the polygon.
            var polygonSymbolOutline = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Orange, 2.0);
            var polygonFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.FromArgb(50, System.Drawing.Color.RebeccaPurple), polygonSymbolOutline);


            polygonGraphic = new Graphic(mahouRivieraPolygon, polygonFillSymbol);
            polygonGraphic.IsVisible = false;
        }


    }
}
