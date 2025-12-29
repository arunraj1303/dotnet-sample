using AuthApi.Common;
using AuthApi.Models;
using AuthApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // 🔐 Authentication required for all endpoints
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ICompanyService companyService, ILogger<CompanyController> logger)
        {
            _companyService = companyService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            _logger.LogInformation("Controller: GetAllCompanies called");
            
            var result = await _companyService.GetAllCompaniesAsync();
            
            if (!result.Success)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(result.Message));
            }
            
            return Ok(ApiResponse<IEnumerable<Company>>.SuccessResponse(result.Data!, result.Message));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            _logger.LogInformation("Controller: GetCompanyById called with ID: {Id}", id);
            
            var result = await _companyService.GetCompanyByIdAsync(id);
            
            if (!result.Success)
            {
                if (result.Message == "Company not found")
                {
                    return NotFound(ApiResponse<object>.ErrorResponse(result.Message));
                }
                return StatusCode(500, ApiResponse<object>.ErrorResponse(result.Message));
            }
            
            return Ok(ApiResponse<Company>.SuccessResponse(result.Data!, result.Message));
        }
    }
}
