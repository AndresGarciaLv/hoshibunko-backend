using hoshibunko.Models.Entities;
using hoshibunko.Models.DTOs;
using hoshibunko.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Hoshibunko.Context;

namespace hoshibunko.Services.Implementations
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ApplicationDbContext _context;

        public CategoriaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoriaDTO>> GetAllAsync()
        {
            return await _context.Categorias
                .Select(c => new CategoriaDTO { Id = c.Id, Nombre = c.Nombre })
                .ToListAsync();
        }

        public async Task<CategoriaDTO> GetByIdAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return null;
            return new CategoriaDTO { Id = categoria.Id, Nombre = categoria.Nombre };
        }

        public async Task<bool> CreateAsync(CategoriaDTO categoriaDto)
        {
            var categoria = new Categoria { Nombre = categoriaDto.Nombre };
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, CategoriaDTO categoriaDto)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return false;

            categoria.Nombre = categoriaDto.Nombre;
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return false;

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
