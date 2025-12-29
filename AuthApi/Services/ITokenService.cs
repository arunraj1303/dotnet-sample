namespace AuthApi.Services
{
    public interface ITokenService
    {
        string GenerateToken(string username, int userId);
    }
}
