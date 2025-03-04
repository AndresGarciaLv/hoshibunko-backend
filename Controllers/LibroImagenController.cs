using hoshibunko.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace hoshibunko.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId}/imagen")]
    public class LibroImagenController : ControllerBase
    {
        private readonly ILibroImagenService _libroImagenService;

        public LibroImagenController(ILibroImagenService libroImagenService)
        {
            _libroImagenService = libroImagenService;
        }

        /// <summary>
        /// ✅ Subir una nueva imagen o actualizar la portada de un libro
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SubirImagen(int libroId, IFormFile file)
        {
            var url = await _libroImagenService.SubirImagenAsync(libroId, file);
            if (url == null) return BadRequest("Formato de imagen no válido o error al subir.");
            return Ok(new { mensaje = "Imagen subida correctamente.", url });
        }

        /// <summary>
        /// ✅ Obtener la imagen de portada de un libro
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObtenerImagen(int libroId)
        {
            var url = await _libroImagenService.ObtenerImagenAsync(libroId);
            if (url == null) return NotFound("Este libro no tiene una imagen de portada.");
            return Ok(new { url });
        }

        /// <summary>
        /// ✅ Eliminar la imagen de un libro
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> EliminarImagen(int libroId)
        {
            var eliminado = await _libroImagenService.EliminarImagenAsync(libroId);
            if (!eliminado) return NotFound("No se encontró la imagen.");
            return NoContent();
        }

        /// <summary>
        /// ✅ Actualizar la portada de un libro (Reemplazar la imagen existente)
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> ActualizarImagen(int libroId, IFormFile file)
        {
            var url = await _libroImagenService.SubirImagenAsync(libroId, file); // ✅ Reutilizamos la misma lógica de `SubirImagenAsync`
            if (url == null) return BadRequest("Formato de imagen no válido o error al actualizar.");
            return Ok(new { mensaje = "Imagen actualizada correctamente.", url });
        }
    }
}


