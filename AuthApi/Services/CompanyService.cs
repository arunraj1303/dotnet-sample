using AuthApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(AppDbContext context, ILogger<CompanyService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, IEnumerable<Company>? Data)> GetAllCompaniesAsync()
        {
            try
            {
                _logger.LogInformation("Service: Fetching all companies");

                var companies = await _context.Companies
                    .OrderBy(c => c.CompanyName)
                    .ToListAsync();

                _logger.LogInformation("Service: Found {Count} companies", companies.Count);

                return (true, $"Retrieved {companies.Count} companies successfully", companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error fetching companies");
                return (false, "Error retrieving companies", null);
            }
        }

        public async Task<(bool Success, string Message, Company? Data)> GetCompanyByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Service: Fetching company with ID: {Id}", id);

                var company = await _context.Companies.FindAsync(id);

                if (company == null)
                {
                    _logger.LogWarning("Service: Company not found with ID: {Id}", id);
                    return (false, "Company not found", null);
                }

                _logger.LogInformation("Service: Company found: {Name}", company.CompanyName);
                return (true, "Company retrieved successfully", company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error fetching company {Id}", id);
                return (false, "Error retrieving company", null);
            }
        }
    }
}
