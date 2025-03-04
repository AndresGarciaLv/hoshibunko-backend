using hoshibunko.Models.DTOs;
using hoshibunko.Models.Entities;
using hoshibunko.Services.IService;
using Hoshibunko.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace hoshibunko.Services.Service
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<Usuario> _userManager;

        public UsuarioService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<Usuario> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<IEnumerable<UsuarioDTO>> GetAllAsync()
        {
            return await _context.Users
                .Select(u => new UsuarioDTO
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Email = u.Email
                })
                .ToListAsync();
        }

        public async Task<UsuarioDTO> GetByIdAsync(string id)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario == null) return null;

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email
            };
        }

        public async Task<UsuarioDTO> CreateAsync(UsuarioDTO usuarioDto, string password)
        {
            var usuario = new Usuario
            {
                UserName = usuarioDto.Email,
                Email = usuarioDto.Email,
                Nombre = usuarioDto.Nombre
            };

            var result = await _userManager.CreateAsync(usuario, password);
            if (!result.Succeeded)
            {
                throw new System.Exception("Error al crear el usuario: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email
            };
        }

        public async Task<UsuarioDTO> UpdateAsync(string id, UsuarioDTO usuarioDto)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario == null) return null;

            if (!UsuarioPuedeModificarOEliminar(usuario))
            {
                throw new UnauthorizedAccessException("No tienes permisos para modificar este usuario.");
            }

            usuario.Nombre = usuarioDto.Nombre;
            usuario.Email = usuarioDto.Email;

            var result = await _userManager.UpdateAsync(usuario);
            if (!result.Succeeded)
            {
                throw new System.Exception("Error al actualizar el usuario: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email
            };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario == null) return false;

            var result = await _userManager.DeleteAsync(usuario);
            return result.Succeeded;
        }

        // ✅ Nuevo método: Obtener datos del usuario autenticado
        public async Task<UsuarioDTO> GetAuthenticatedUserAsync(string userId)
        {
            var usuario = await _context.Users.FindAsync(userId);
            if (usuario == null) return null;

            var roles = await _userManager.GetRolesAsync(usuario); // ✅ Obtiene los roles

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Roles = roles.ToList() // ✅ Agrega los roles al DTO
            };
        }


        private bool UsuarioPuedeModificarOEliminar(Usuario usuario)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool esAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;

            return esAdmin || usuario.Id == userId;
        }
    }
}


