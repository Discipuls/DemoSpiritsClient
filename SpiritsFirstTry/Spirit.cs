using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritsFirstTry
{
    public enum SpiritsClassification { forest, water, home, dark, field}
    public class Spirit
    {
        public static int maxzindex = 0;
        public string Name {  get; set; }
        public string Description { get; set; }

        public string Image_name { get; set; }
        public MapPoint mapPoint { get; set; }
        public List<SpiritsClassification> Classification { get; set; }
        public string ClassificationString { get; set; }
        public List<string> Habitats { get; set; }
        public string HabitatsString { get; set; }
        public string Image_name_t {  get; set; }
        public PictureMarkerSymbol markerSymbol { get; set; }
        public Graphic pinGraphic { get; set; }
        
        public Spirit(double longitude, double latitude, string name, string description, string image_name,
            List<SpiritsClassification> classification, List<string> habitats)
        {
            Name = name;
            Description = description;
            Image_name = image_name;
            Image_name_t = image_name.Insert(image_name.Length - 4, "t");

            mapPoint = (MapPoint)((new MapPoint(longitude, latitude, SpatialReferences.Wgs84))
                .Project(SpatialReferences.WebMercator));
            Classification = classification;
            Habitats = habitats;
            HabitatsString = habitats[0];

            for (int i = 0; i < classification.Count(); i++)
            {
                if (i == 0)
                {
                    if (classification[i] == SpiritsClassification.forest)
                    {
                        ClassificationString = "Лясны";
                    }else if (classification[i] == SpiritsClassification.home)
                    {
                        ClassificationString = "Хатні";
                    }
                    else if (classification[i] == SpiritsClassification.dark)
                    {
                        ClassificationString = "Цёмны (злыдзень)";
                    }
                    else if (classification[i] == SpiritsClassification.water)
                    {
                        ClassificationString = "Водны";
                    }
                    else if (classification[i] == SpiritsClassification.field)
                    {
                        ClassificationString = "Палявы";
                    }
                }
                else
                {
                    if (classification[i] == SpiritsClassification.forest)
                    {
                        ClassificationString += ", лясны";
                    }
                    else if (classification[i] == SpiritsClassification.home)
                    {
                        ClassificationString += ", хатні";
                    }
                    else if (classification[i] == SpiritsClassification.dark)
                    {
                        ClassificationString += ", цёмны (злыдзень)";
                    }
                    else if (classification[i] == SpiritsClassification.water)
                    {
                        ClassificationString += ", водны";
                    }
                    else if (classification[i] == SpiritsClassification.field)
                    {
                        ClassificationString += ", палявы";
                    }
                }
            }
        }


    }
}
