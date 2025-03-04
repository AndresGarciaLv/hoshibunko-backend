namespace hoshibunko.Services.Service
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using hoshibunko.Services.IService;
    using hoshibunko.Models.Entities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<Usuario> _userManager;

        public TokenService(IConfiguration config, UserManager<Usuario> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public string GenerateJwtToken(Usuario user, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["JwtSettings:ExpirationMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task SaveRefreshToken(Usuario user, string refreshToken)
        {
            var existingToken = await _userManager.GetAuthenticationTokenAsync(user, "MyApp", "RefreshToken");

            if (existingToken != null)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
            }

            await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", refreshToken);
        }

        public async Task<Usuario?> GetUserByRefreshToken(string refreshToken)
        {
            return await _userManager.Users.FirstOrDefaultAsync(user =>
                _userManager.GetAuthenticationTokenAsync(user, "MyApp", "RefreshToken").Result == refreshToken);
        }

        public string GenerateJwtToken(IdentityUser user, string role)
        {
            throw new NotImplementedException();
        }

        public Task SaveRefreshToken(IdentityUser user, string refreshToken)
        {
            throw new NotImplementedException();
        }

       

    }
}
