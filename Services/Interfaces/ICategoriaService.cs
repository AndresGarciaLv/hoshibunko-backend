using hoshibunko.Models.DTOs;

namespace hoshibunko.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaDTO>> GetAllAsync();
        Task<CategoriaDTO> GetByIdAsync(int id);
        Task<bool> CreateAsync(CategoriaDTO categoriaDto);
        Task<bool> UpdateAsync(int id, CategoriaDTO categoriaDto);
        Task<bool> DeleteAsync(int id);
    }
}
