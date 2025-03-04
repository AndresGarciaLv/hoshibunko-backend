using hoshibunko.Models.DTOs;

namespace hoshibunko.Services.Interfaces
{
    public interface IAutorService
    {
        Task<IEnumerable<AutorDTO>> GetAllAsync();
        Task<AutorDTO> GetByIdAsync(int id);
        Task<bool> CreateAsync(AutorDTO autorDto);
        Task<bool> UpdateAsync(int id, AutorDTO autorDto);
        Task<bool> DeleteAsync(int id);
    }
}