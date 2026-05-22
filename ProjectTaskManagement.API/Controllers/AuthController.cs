using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Auth;
using ProjectTaskManagement.Application.Interfaces;
using ProjectTaskManagement.Infrastructure.Identity;

namespace ProjectTaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var userExists = await _userManager.FindByEmailAsync(dto.Email);
        if (userExists != null)
            return BadRequest(ApiResponse<string>.Fail("User already exists"));

        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(ApiResponse<string>.Fail(
                string.Join(", ", result.Errors.Select(e => e.Description))));

        var token = _jwtService.GenerateToken(user.Id, user.Email!, user.UserName!);

        return Ok(ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            UserName = user.UserName!
        }, "User registered successfully"));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Unauthorized(ApiResponse<string>.Fail("Invalid credentials"));

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
            return Unauthorized(ApiResponse<string>.Fail("Invalid credentials"));

        var token = _jwtService.GenerateToken(user.Id, user.Email!, user.UserName!);

        return Ok(ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            UserName = user.UserName!
        }, "Login successful"));
    }
}