using hoshibunko.Models.DTOs.Auth;
using hoshibunko.Models.Entities;
using hoshibunko.Services.IService;
using Hoshibunko.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace hoshibunko.Controllers;

[Route("api/Auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;

    public AuthController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, ITokenService tokenService, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var user = new Usuario
        {
            UserName = request.Username,
            Email = request.Email,
            Nombre = request.Nombre // Se mantiene la propiedad Nombre de Usuario
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, "User");

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateJwtToken(user, roles.FirstOrDefault() ?? "User");
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _tokenService.SaveRefreshToken(user, refreshToken);

        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userManager.FindByNameAsync(request.Username)
                   ?? await _userManager.FindByEmailAsync(request.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized("Usuario o contraseña incorrectos");

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateJwtToken(user, roles.FirstOrDefault() ?? "User");
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _tokenService.SaveRefreshToken(user, refreshToken);

        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var user = await _tokenService.GetUserByRefreshToken(request.RefreshToken);
        if (user == null)
            return Unauthorized("Refresh Token inválido");

        var roles = await _userManager.GetRolesAsync((Usuario)user);
        var newAccessToken = _tokenService.GenerateJwtToken(user, roles.FirstOrDefault() ?? "User");
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        await _tokenService.SaveRefreshToken(user, newRefreshToken);

        return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
    }
}

