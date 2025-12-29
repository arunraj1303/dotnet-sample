using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;

        public TokenService(ILogger<TokenService> logger)
        {
            _logger = logger;
        }

        public string GenerateToken(string username, int userId)
        {
            try
            {
                var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "YourSuperSecretKeyForJWTTokenGeneration12345678";
                var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "AuthApi";
                var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "AuthApiUsers";
                var jwtExpiryMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES") ?? "60");

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("userId", userId.ToString()),
                    new Claim(ClaimTypes.Name, username)
                };

                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(jwtExpiryMinutes),
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                
                _logger.LogInformation("Token generated successfully for user: {Username}", username);
                
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating token for user: {Username}", username);
                throw;
            }
        }
    }
}
