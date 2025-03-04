
using hoshibunko.Models.Entities;
using hoshibunko.Models.DTOs;
using hoshibunko.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Hoshibunko.Context;

namespace hoshibunko.Services.Implementations
{
    public class LikeService : ILikeService
    {
        private readonly ApplicationDbContext _context;

        public LikeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LikeDTO>> GetAllAsync()
        {
            return await _context.Likes
                .Select(l => new LikeDTO { Id = l.Id, UsuarioId = l.UsuarioId, LibroId = l.LibroId })
                .ToListAsync();
        }

        public async Task<LikeDTO> GetByIdAsync(int id)
        {
            var like = await _context.Likes.FindAsync(id);
            if (like == null) return null;
            return new LikeDTO { Id = like.Id, UsuarioId = like.UsuarioId, LibroId = like.LibroId };
        }

        public async Task<bool> CreateAsync(LikeDTO likeDto)
        {
            var like = new Like { UsuarioId = likeDto.UsuarioId, LibroId = likeDto.LibroId };
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var like = await _context.Likes.FindAsync(id);
            if (like == null) return false;

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<LikeDTO>> GetLikesByLibroIdAsync(int libroId)
        {
            return await _context.Likes
                .Where(l => l.LibroId == libroId)
                .Select(l => new LikeDTO { Id = l.Id, UsuarioId = l.UsuarioId, LibroId = l.LibroId })
                .ToListAsync();
        }

        public async Task<int> GetTotalLikesByLibroIdAsync(int libroId)
        {
            return await _context.Likes.CountAsync(l => l.LibroId == libroId);
        }
    }
}
