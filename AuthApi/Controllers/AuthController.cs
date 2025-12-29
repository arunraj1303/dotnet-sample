using AuthApi.Common;
using AuthApi.DTO;
using AuthApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            _logger.LogInformation("Login request received for username: {Username}", dto.Username);

            var (success, message, token) = await _authService.LoginAsync(dto);

            if (!success)
            {
                _logger.LogWarning("Login failed for username: {Username}", dto.Username);
                return Unauthorized(ApiResponse<object>.ErrorResponse(message));
            }

            var responseData = new
            {
                Token = token,
                Username = dto.Username
            };

            _logger.LogInformation("User logged in successfully: {Username}", dto.Username);
            return Ok(ApiResponse<object>.SuccessResponse(responseData, message));
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Get username from JWT token claims
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                var userId = User.FindFirst("userId")?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    _logger.LogWarning("Unable to extract username from token");
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid token"));
                }

                _logger.LogInformation("Profile request for user: {Username}", username);

                var (success, message, userData) = await _authService.GetUserAsync(username);

                if (!success)
                {
                    _logger.LogWarning("User not found: {Username}", username);
                    return NotFound(ApiResponse<object>.ErrorResponse(message));
                }

                _logger.LogInformation("Profile retrieved successfully for user: {Username}", username);
                return Ok(ApiResponse<object>.SuccessResponse(userData, "Profile retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            _logger.LogInformation("Test API called successfully");

            var testData = new
            {
                Status = "API is working!",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                DatabaseHost = Environment.GetEnvironmentVariable("DB_HOST"),
                DatabaseName = Environment.GetEnvironmentVariable("DB_NAME"),
                Timestamp = DateTime.UtcNow
            };

            return Ok(ApiResponse<object>.SuccessResponse(testData, "Test API executed successfully"));
        }
    }
}
