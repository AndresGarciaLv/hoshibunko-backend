using hoshibunko.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hoshibunko.Services.IService
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDTO>> GetAllAsync();
        Task<UsuarioDTO> GetByIdAsync(string id);
        Task<UsuarioDTO> CreateAsync(UsuarioDTO usuarioDto, string password);
        Task<UsuarioDTO> UpdateAsync(string id, UsuarioDTO usuarioDto);
        Task<bool> DeleteAsync(string id);

        // ✅ Nuevo método para obtener datos del usuario autenticado
        Task<UsuarioDTO> GetAuthenticatedUserAsync(string userId);
    }
}

