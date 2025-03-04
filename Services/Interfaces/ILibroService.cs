using hoshibunko.Models.DTOs.Libro;
namespace hoshibunko.Services.Interfaces
{
    public interface ILibroService
    {
        Task<IEnumerable<GetLibroDTO>> GetAllAsync();
        Task<GetLibroDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateLibroDTO libroDto);
        Task<bool> UpdateAsync(int id, CreateLibroDTO libroDto);
        Task<bool> DeleteAsync(int id);

        // ✅ Método para el catálogo con paginación
        Task<(IEnumerable<GetLibroDTO> Libros, int TotalLibros, int TotalPaginas)> GetCatalogoAsync(int page, int pageSize);

        Task<IEnumerable<GetLibroDTO>> GetLibrosPorCategoriaAsync(int categoriaId);
    }
}
