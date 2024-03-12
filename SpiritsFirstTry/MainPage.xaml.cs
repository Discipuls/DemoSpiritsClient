using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using SpiritsFirstTry.ViewModels;

namespace SpiritsFirstTry
{
    public partial class MainPage : ContentPage
    {
        MainViewModel mainViewModel;
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Dark;
            BindingContext = vm;
            mainViewModel = vm;
            vm.SetupMap(MainMapView);
        }
        public void Tap(object sender, TappedEventArgs args)
        {
            mainViewModel.OpenSearch();
        }

    }

}
