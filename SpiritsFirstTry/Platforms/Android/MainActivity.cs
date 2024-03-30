using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Auth.Api.SignIn;
using Android.OS;

namespace SpiritsFirstTry
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public static event EventHandler<(bool Success, GoogleSignInAccount Account)> ResultGoogleAuth;

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            // Auth request code
            if (requestCode == 9001)
            {
                try
                {
                    // Read intent result data
                    var currentAccount = await GoogleSignIn.GetSignedInAccountFromIntentAsync(data);
                    // Rise static event for send data
                    ResultGoogleAuth?.Invoke(this, (currentAccount.Email != null, currentAccount));
                }
                catch (Exception ex)
                {
                    ResultGoogleAuth?.Invoke(this, (false, null));
                }
            }
        }
    }
}
