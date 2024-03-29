// Ignore Spelling: Admin

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
            vm.SetupProgressBar(this.DataLoadingProgressBar);
            vm.SetupMap(MainMapView);
        }
        public void TapSearch(object sender, TappedEventArgs args)
        {
            mainViewModel.OpenSearch();
        }
        public void TapAdmin(object sender, TappedEventArgs args)
        {
            mainViewModel.HideBottomPage();
            //  Navigation.PushAsync(new AdminPage());
            var navigaionParameter = new ShellNavigationQueryParameters
            {
                {"Spirits", mainViewModel.GetSpirits()},
                {"Habitats", mainViewModel.GetHabitats()}
            };
            Shell.Current.GoToAsync("//Habitats", navigaionParameter);
            Shell.Current.GoToAsync("//Spirits", navigaionParameter);
        }



    }

}
