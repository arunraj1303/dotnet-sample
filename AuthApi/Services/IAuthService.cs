using AuthApi.DTO;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, string? Token)> LoginAsync(LoginDTO dto);
        Task<(bool Success, string Message, IEnumerable<IdentityError>? Errors)> RegisterUserAsync(RegisterDTO dto);
        Task<(bool Success, string Message, object? UserData)> GetUserAsync(string username);
        Task<(bool Success, string Message, IEnumerable<IdentityError>? Errors)> UpdateUserAsync(UpdateUserDTO dto);
    }
}
