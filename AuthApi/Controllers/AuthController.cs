using AuthApi.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(AuthApi.DTO.RegisterDTO dto)
    {
        var existingUser = await _userManager.FindByNameAsync(dto.Username);
        if (existingUser != null)
            return BadRequest("User already exists");

        var user = new IdentityUser
        {
            UserName = dto.Username
        };

        var result = await _userManager.CreateAsync(user, dto.password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("User registered successfully");

    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUser(string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
            return NotFound("User not found");

        return Ok(new
        {
            
            user.UserName
        });
    }
    [HttpPut]
    public async Task<IActionResult> UpdateUser(UpdateUserDTO dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username);

        if (user == null)
            return NotFound("User not found");

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
                return BadRequest(passwordResult.Errors);
        }

        return Ok("User updated successfully");
    }

}
