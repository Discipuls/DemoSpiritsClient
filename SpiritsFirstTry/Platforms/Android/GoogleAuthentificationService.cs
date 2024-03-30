using Android.App;
using Android.Gms.Auth.Api.SignIn;
using Microsoft.Extensions.Configuration;
using SpiritsFirstTry.DTOs;
using SpiritsFirstTry.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritsFirstTry.Platforms.Android
{
    public class GoogleAuthentificationService : IGoogleAuthentificationService
    {
        private Activity _activity;
        private GoogleSignInClient _googleSignInClient;
        private TaskCompletionSource<UserDTO> _taskCompletionSource;

        private Task<UserDTO> GoogleAuthentication
        {
            get => _taskCompletionSource.Task;
        }

        public GoogleAuthentificationService(IConfiguration config)
        {
            _activity = Platform.CurrentActivity;

            var _gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestIdToken(config["auth:client_id"])
                .RequestEmail()
                .RequestId()
                .RequestProfile()
                .Build();

            // Get client
            _googleSignInClient = GoogleSignIn.GetClient(_activity, _gso);
            MainActivity.ResultGoogleAuth += MainActivity_ResultGoogleAuth;

        }

        private void MainActivity_ResultGoogleAuth(object sender, (bool Success, GoogleSignInAccount Account) e)
        {
            if (e.Success)
                // Set result of Task
                _taskCompletionSource.SetResult(new UserDTO
                {
                    Email = e.Account.Email,
                    FullName = e.Account.DisplayName,
                    Id = e.Account.Id,
                    UserName = e.Account.GivenName,
                    Token = e.Account.IdToken
                });
            else
                // Set Exception
                _taskCompletionSource.SetException(new Exception("Error"));
        }

        public Task<UserDTO> AythenticateAsync()
        {
            _taskCompletionSource = new TaskCompletionSource<UserDTO>();
            _activity.StartActivityForResult(_googleSignInClient.SignInIntent, 9001);

            return GoogleAuthentication;
        }

        public async Task<UserDTO> GetCurrentUserAsync()
        {
            try
            {
                var user = await _googleSignInClient.SilentSignInAsync();
                return new UserDTO
                {
                    Email = user.Email,
                    FullName = $"{user.DisplayName}",
                    Id = user.Id,
                    UserName = user.GivenName
                    ,
                    Token = user.IdToken
                };

            }
            catch (Exception)
            {
                throw new Exception("Error");
            }
        }

        public async Task LogoutAsync() => _googleSignInClient.SignOutAsync();


    }
}
