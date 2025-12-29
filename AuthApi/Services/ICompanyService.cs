using AuthApi.Models;

namespace AuthApi.Services
{
    public interface ICompanyService
    {
        Task<(bool Success, string Message, IEnumerable<Company>? Data)> GetAllCompaniesAsync();
        Task<(bool Success, string Message, Company? Data)> GetCompanyByIdAsync(int id);
    }
}
