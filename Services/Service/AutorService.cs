
using hoshibunko.Models.Entities;
using hoshibunko.Models.DTOs;
using hoshibunko.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Hoshibunko.Context;

namespace hoshibunko.Services.Implementations
{
    public class AutorService : IAutorService
    {
        private readonly ApplicationDbContext _context;

        public AutorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AutorDTO>> GetAllAsync()
        {
            return await _context.Autores
                .Select(a => new AutorDTO { Id = a.Id, Nombre = a.Nombre })
                .ToListAsync();
        }

        public async Task<AutorDTO> GetByIdAsync(int id)
        {
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null) return null;
            return new AutorDTO { Id = autor.Id, Nombre = autor.Nombre };
        }

        public async Task<bool> CreateAsync(AutorDTO autorDto)
        {
            var autor = new Autor { Nombre = autorDto.Nombre };
            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, AutorDTO autorDto)
        {
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null) return false;

            autor.Nombre = autorDto.Nombre;
            _context.Autores.Update(autor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null) return false;

            _context.Autores.Remove(autor);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}