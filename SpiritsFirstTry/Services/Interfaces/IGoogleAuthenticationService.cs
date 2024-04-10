using SpiritsFirstTry.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
