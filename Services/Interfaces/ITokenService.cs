namespace hoshibunko.Services.IService
{
    using hoshibunko.Models.Entities;
    using System.Threading.Tasks;

    public interface ITokenService
    {
        string GenerateJwtToken(Usuario user, string role);
        string GenerateRefreshToken();
        Task SaveRefreshToken(Usuario user, string refreshToken);
        Task<Usuario?> GetUserByRefreshToken(string refreshToken);
    }
}
