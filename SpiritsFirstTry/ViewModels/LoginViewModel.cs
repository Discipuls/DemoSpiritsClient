using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpiritsFirstTry.Services.Interfaces;

namespace SpiritsFirstTry.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        IGoogleAuthenticationService googleAuthentificationService;
        IRestService restService;
        public LoginViewModel(IGoogleAuthenticationService googleAuthenticationService,
            IRestService restService) 
        {
            this.googleAuthentificationService = googleAuthenticationService;
            this.restService = restService;

        }

        [RelayCommand]
        async Task GoGuest()
        {
            await googleAuthentificationService.setIsGuest(true);
            await GoMain();
        }

        [RelayCommand]
        async Task Login()
        {
            await googleAuthentificationService.AythenticateAsync();
            await restService.AddAuthHeader((await googleAuthentificationService.GetCurrentUserAsync()).Token);
            await googleAuthentificationService.setIsGuest(false);

            await GoMain();
        }

        private async Task GoMain()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
