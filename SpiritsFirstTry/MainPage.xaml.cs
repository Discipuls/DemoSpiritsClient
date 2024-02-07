using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using SpiritsFirstTry.ViewModels;
using Map = Esri.ArcGISRuntime.Mapping.Map;

namespace SpiritsFirstTry
{
    public partial class MainPage : ContentPage
    {

        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            vm.SetupMap(MainMapView);
        }


    }

}
