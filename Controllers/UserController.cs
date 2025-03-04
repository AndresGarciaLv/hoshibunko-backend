using hoshibunko.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using hoshibunko.Models.DTOs.Usuario;

namespace hoshibunko.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: api/Usuario
        // Solo administradores pueden ver la lista completa de usuarios.
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            return Ok(usuarios);
        }

        // GET: api/Usuario/{id}
        // Cualquier usuario autenticado puede consultar los detalles de un usuario.
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            return Ok(usuario);
        }

        // POST: api/Usuario?password=valor
        // Solo administradores pueden crear nuevos usuarios.
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Create([FromBody] CrearUsuarioDTO usuarioDto)
        {
            // Se crea el usuario y se asigna el password proporcionado.
            var usuario = await _usuarioService.CreateAsync(usuarioDto);
            // Retorna un código 201 Created con la ubicación del nuevo recurso.
            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }

        // PUT: api/Usuario/{id}
        // Permite actualizar un usuario; el propio usuario o un administrador pueden hacerlo.
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUsuarioDTO usuarioDto)
        {
            // Se obtiene el id del usuario autenticado.
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool esAdmin = User.IsInRole("ADMIN");

            // Se verifica que el usuario exista.
            var usuarioExistente = await _usuarioService.GetByIdAsync(id);
            if (usuarioExistente == null)
                return NotFound("Usuario no encontrado.");

            // Se verifica que el usuario tenga permisos para actualizar.
            if (!esAdmin && usuarioExistente.Id != userId)
                return Forbid("No tienes permisos para modificar este usuario.");

            // Se actualiza el usuario.
            var updatedUsuario = await _usuarioService.UpdateAsync(id, usuarioDto);
            if (updatedUsuario == null)
                return BadRequest("Error al actualizar el usuario.");

            return Ok(updatedUsuario);
        }

        // DELETE: api/Usuario/{id}
        // Solo administradores pueden eliminar un usuario.
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _usuarioService.DeleteAsync(id);
            if (!success)
                return NotFound("Usuario no encontrado o error al eliminar.");

            return NoContent();
        }

        // GET: api/Usuario/me
        // Obtiene la información del usuario autenticado, incluyendo sus roles.
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetAuthenticatedUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("No se pudo obtener el usuario autenticado.");

            var usuario = await _usuarioService.GetAuthenticatedUserAsync(userId);

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            return Ok(usuario);
        }
    }
}
