using hoshibunko.Models.DTOs;

namespace hoshibunko.Services.Interfaces
{
    public interface ILikeService
    {
        Task<IEnumerable<LikeDTO>> GetAllAsync();
        Task<LikeDTO> GetByIdAsync(int id);
        Task<bool> CreateAsync(LikeDTO likeDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<LikeDTO>> GetLikesByLibroIdAsync(int libroId);

        Task<int> GetTotalLikesByLibroIdAsync(int libroId);
    }
}
