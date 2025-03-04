using hoshibunko.Models.DTOs.Usuario;
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

        public async Task<IEnumerable<GetUsuarioDTO>> GetAllAsync()
        {
            // Recupera todos los usuarios desde la base de datos.
            var usuarios = await _context.Users.ToListAsync();
            var usuariosDto = new List<GetUsuarioDTO>();

            // Itera sobre cada usuario para obtener sus roles.
            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                usuariosDto.Add(new GetUsuarioDTO
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Username = usuario.UserName,
                    Email = usuario.Email,
                    Roles = roles.ToList()
                });
            }
            return usuariosDto;
        }

        public async Task<GetUsuarioDTO> GetByIdAsync(string id)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario == null) return null;

            var roles = await _userManager.GetRolesAsync(usuario);
            return new GetUsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Username = usuario.UserName,
                Email = usuario.Email,
                Roles = roles.ToList()
            };
        }

        public async Task<CrearUsuarioDTO> CreateAsync(CrearUsuarioDTO usuarioDto)
        {
            // Se crea la entidad Usuario
            var usuario = new Usuario
            {
                UserName = usuarioDto.Username,
                Email = usuarioDto.Email,
                Nombre = usuarioDto.Nombre
            };

            // Se usa la propiedad Password del DTO
            var result = await _userManager.CreateAsync(usuario, usuarioDto.Password);
            if (!result.Succeeded)
            {
                throw new System.Exception("Error al crear el usuario: "
                    + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Asignar roles si existen en el DTO
            if (usuarioDto.Roles != null && usuarioDto.Roles.Any())
            {
                foreach (var role in usuarioDto.Roles)
                {
                    var roleResult = await _userManager.AddToRoleAsync(usuario, role);
                    if (!roleResult.Succeeded)
                    {
                        throw new System.Exception("Error asignando el rol " + role + ": "
                            + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                }
            }

            // Se recuperan los roles asignados (en caso de haberlos)
            var assignedRoles = await _userManager.GetRolesAsync(usuario);

            return new CrearUsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Username = usuario.UserName,
                Roles = assignedRoles.ToList()
            };
        }

        public async Task<UpdateUsuarioDTO> UpdateAsync(string id, UpdateUsuarioDTO usuarioDto)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario == null) return null;

            if (!UsuarioPuedeModificarOEliminar(usuario))
            {
                throw new UnauthorizedAccessException("No tienes permisos para modificar este usuario.");
            }

            // Actualizamos los datos básicos del usuario
            usuario.Nombre = usuarioDto.Nombre;
            usuario.UserName = usuarioDto.Username;
            usuario.Email = usuarioDto.Email;

            // Actualizamos el usuario (sin contraseña)
            var result = await _userManager.UpdateAsync(usuario);
            if (!result.Succeeded)
            {
                throw new System.Exception("Error al actualizar el usuario: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Si se proporcionó una nueva contraseña, se actualiza
            if (!string.IsNullOrEmpty(usuarioDto.Password))
            {
                // Si el usuario ya tiene contraseña, se remueve primero
                if (await _userManager.HasPasswordAsync(usuario))
                {
                    var removePassResult = await _userManager.RemovePasswordAsync(usuario);
                    if (!removePassResult.Succeeded)
                    {
                        throw new System.Exception("Error al remover la contraseña: " + string.Join(", ", removePassResult.Errors.Select(e => e.Description)));
                    }
                }

                // Se agrega la nueva contraseña
                var addPassResult = await _userManager.AddPasswordAsync(usuario, usuarioDto.Password);
                if (!addPassResult.Succeeded)
                {
                    throw new System.Exception("Error al actualizar la contraseña: " + string.Join(", ", addPassResult.Errors.Select(e => e.Description)));
                }
            }

            // Opcional: Aquí se pueden actualizar los roles si la lógica de negocio lo requiere.

            var roles = await _userManager.GetRolesAsync(usuario);
            return new UpdateUsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Username = usuario.UserName,
                Email = usuario.Email,
                Roles = roles.ToList()
            };
        }


        public async Task<bool> DeleteAsync(string id)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario == null) return false;

            var result = await _userManager.DeleteAsync(usuario);
            return result.Succeeded;
        }

        // Método para obtener datos del usuario autenticado, incluyendo roles.
        public async Task<GetUsuarioDTO> GetAuthenticatedUserAsync(string userId)
        {
            var usuario = await _context.Users.FindAsync(userId);
            if (usuario == null) return null;

            var roles = await _userManager.GetRolesAsync(usuario);

            return new GetUsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Username = usuario.UserName,
                Email = usuario.Email,
                Roles = roles.ToList()
            };
        }

        private bool UsuarioPuedeModificarOEliminar(Usuario usuario)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool esAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("ADMIN") ?? false;

            return esAdmin || usuario.Id == userId;
        }
    }
}

