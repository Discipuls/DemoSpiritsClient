using SpiritsFirstTry.DTOs;

namespace SpiritsFirstTry.Services.Interfaces
{
    public interface IGoogleAuthenticationService
    {
        Task<UserDTO> AythenticateAsync();
        Task LogoutAsync();
        Task<UserDTO> GetCurrentUserAsync();

        Task<bool> getIsGuest();
        Task setIsGuest(bool isGuest);
    }
}
