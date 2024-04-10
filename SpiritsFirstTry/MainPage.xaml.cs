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
        IGoogleAuthenticationService authentificationService;
        IRestService restService;
        MainViewModel mainViewModel;
        public MainPage(MainViewModel vm, IConfiguration config,
            IGoogleAuthenticationService authentificationService,
            IRestService restService)
        {
           // Shell.Current.GoToAsync("//LoginPage");
           this.restService = restService;
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
        public void TapLogin(object sender, TappedEventArgs args)
        {

            _ = doLogin();
        }

        private async Task doLogin()
        {
            try
            {
                await authentificationService.LogoutAsync();
            }
            catch
            {

            }
            try
            {
                await authentificationService.AythenticateAsync();
                var token = (await authentificationService.GetCurrentUserAsync()).Token;
                await restService.AddAuthHeader(token);
                await mainViewModel.SetupAdminImage(adminImage);
            }
            catch (Exception e)
            {

            }
        }


    }

}
