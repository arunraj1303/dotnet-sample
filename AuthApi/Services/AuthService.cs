using AuthApi.DTO;
using AuthApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace AuthApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<IdentityUser> userManager, 
            AppDbContext context,
            ITokenService tokenService,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, string? Token)> LoginAsync(LoginDTO dto)
        {
            try
            {
                _logger.LogInformation("Login attempt for username: {Username}", dto.Username);

                // Check user from v_users table
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == dto.Username);

                if (user == null)
                {
                    _logger.LogWarning("User not found: {Username}", dto.Username);
                    return (false, "Invalid username or password", null);
                }

                // Verify BCrypt hashed password
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
                
                if (!isPasswordValid)
                {
                    _logger.LogWarning("Invalid password for user: {Username}", dto.Username);
                    return (false, "Invalid username or password", null);
                }

                // Generate JWT token
                var token = _tokenService.GenerateToken(user.Username, user.Id);

                _logger.LogInformation("User logged in successfully: {Username}", dto.Username);
                return (true, "Login successful", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for username: {Username}", dto.Username);
                throw;
            }
        }

        public async Task<(bool Success, string Message, IEnumerable<IdentityError>? Errors)> RegisterUserAsync(RegisterDTO dto)
        {
            try
            {
                var existingUser = await _userManager.FindByNameAsync(dto.Username);
                if (existingUser != null)
                {
                    return (false, "User already exists", null);
                }

                var user = new IdentityUser
                {
                    UserName = dto.Username
                };

                var result = await _userManager.CreateAsync(user, dto.password);

                if (!result.Succeeded)
                {
                    return (false, "Registration failed", result.Errors);
                }

                _logger.LogInformation("User {Username} registered successfully", dto.Username);
                return (true, "User registered successfully", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Username}", dto.Username);
                throw;
            }
        }

        public async Task<(bool Success, string Message, object? UserData)> GetUserAsync(string username)
        {
            try
            {
                // Get user from v_users table
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return (false, "User not found", null);
                }

                var userData = new
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return (true, "User found", userData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {Username}", username);
                throw;
            }
        }

        public async Task<(bool Success, string Message, IEnumerable<IdentityError>? Errors)> UpdateUserAsync(UpdateUserDTO dto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(dto.Username);

                if (user == null)
                {
                    return (false, "User not found", null);
                }

                // Update Password
                if (!string.IsNullOrEmpty(dto.NewPassword))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(
                        user,
                        token,
                        dto.NewPassword
                    );

                    if (!passwordResult.Succeeded)
                    {
                        return (false, "Password update failed", passwordResult.Errors);
                    }
                }

                _logger.LogInformation("User {Username} updated successfully", dto.Username);
                return (true, "User updated successfully", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {Username}", dto.Username);
                throw;
            }
        }
    }
}
