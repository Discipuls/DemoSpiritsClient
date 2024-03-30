// Ignore Spelling: Admin

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using SpiritsFirstTry.ViewModels;
using Microsoft.Extensions.Configuration;
using SpiritsFirstTry.Services.Interfaces;


#if ANDROID
using SpiritsFirstTry.Platforms.Android;
#endif

namespace SpiritsFirstTry
{
    public partial class MainPage : ContentPage
    {
        IConfiguration config;
        IGoogleAuthentificationService authentificationService;
        MainViewModel mainViewModel;
        public MainPage(MainViewModel vm, IConfiguration config,
            IGoogleAuthentificationService authentificationService)
        {
            this.authentificationService = authentificationService;
            this.config = config;
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
        //    _ = doAuth();
            mainViewModel.HideBottomPage();
            var navigaionParameter = new ShellNavigationQueryParameters
            {
                {"Spirits", mainViewModel.GetSpirits()},
                {"Habitats", mainViewModel.GetHabitats()}
            };
            Shell.Current.GoToAsync("//Habitats", navigaionParameter);
            Shell.Current.GoToAsync("//Spirits", navigaionParameter);
        }

        private async Task doAuth()
        {
            //var client_id = config["auth:client_id"];
            await authentificationService.LogoutAsync();
            await authentificationService.AythenticateAsync();
            var userDTO = await authentificationService.GetCurrentUserAsync();
        }


    }

}
