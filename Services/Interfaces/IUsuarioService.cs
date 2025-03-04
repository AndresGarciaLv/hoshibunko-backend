using hoshibunko.Models.DTOs.Usuario;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hoshibunko.Services.IService
{
    public interface IUsuarioService
    {
        Task<IEnumerable<GetUsuarioDTO>> GetAllAsync();
        Task<GetUsuarioDTO> GetByIdAsync(string id);
        Task<CrearUsuarioDTO> CreateAsync(CrearUsuarioDTO usuarioDto);
        Task<UpdateUsuarioDTO> UpdateAsync(string id, UpdateUsuarioDTO usuarioDto);
        Task<bool> DeleteAsync(string id);

        // ✅ Nuevo método para obtener datos del usuario autenticado
        Task<GetUsuarioDTO> GetAuthenticatedUserAsync(string userId);
    }
}

