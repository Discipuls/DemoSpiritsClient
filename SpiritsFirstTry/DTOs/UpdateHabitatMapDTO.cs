using Esri.ArcGISRuntime.UI;
using SpiritsClassLibrary.DTOs.HabitatDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritsFirstTry.DTOs
{
    public class UpdateHabitatMapDTO : UpdateHabitatDTO
    {
        public int index { get; set; }
        public Graphic PolygonGraphic {  get; set; }
    }
}
