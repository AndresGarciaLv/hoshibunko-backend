using hoshibunko.Models.DTOs;
using hoshibunko.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario == null) return NotFound();
            return Ok(usuario);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] UsuarioDTO usuarioDto, [FromQuery] string password)
        {
            var usuario = await _usuarioService.CreateAsync(usuarioDto, password);
            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, [FromBody] UsuarioDTO usuarioDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool esAdmin = User.IsInRole("Admin");

            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            if (!esAdmin && usuario.Id != userId)
            {
                return Forbid();
            }

            var updatedUsuario = await _usuarioService.UpdateAsync(id, usuarioDto);
            if (updatedUsuario == null) return BadRequest();

            return Ok(updatedUsuario);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _usuarioService.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }

        // ✅ Nuevo endpoint: Obtener datos del usuario autenticado
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetAuthenticatedUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("No se pudo obtener el usuario autenticado.");

            var usuario = await _usuarioService.GetAuthenticatedUserAsync(userId);

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            return Ok(usuario);
        }
    }
}
