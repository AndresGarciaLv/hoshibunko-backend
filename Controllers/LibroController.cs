using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using hoshibunko.Services.Interfaces;
using hoshibunko.Models.DTOs.Libro;

namespace hoshibunko.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LibroController : ControllerBase
    {
        private readonly ILibroService _libroService;

        public LibroController(ILibroService libroService)
        {
            _libroService = libroService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GetLibroDTO>>> GetAll()
        {
            return Ok(await _libroService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<GetLibroDTO>> GetById(int id)
        {
            var libro = await _libroService.GetByIdAsync(id);
            if (libro == null) return NotFound();
            return Ok(libro);
        }
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<int>> Create([FromBody] CreateLibroDTO libroDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Ahora `CreateAsync` devuelve un entero (ID)
            var newLibroId = await _libroService.CreateAsync(libroDto);

            if (newLibroId == 0)
            {
                // Significa que no se pudo crear (autor no existe o algo así)
                return BadRequest("No se pudo crear el libro.");
            }

            // Retornamos el ID del libro recién creado
            return Ok(newLibroId);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateLibroDTO libroDto)
        {
            var updated = await _libroService.UpdateAsync(id, libroDto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _libroService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("catalogo")]
        public async Task<ActionResult> GetCatalogo([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (libros, totalLibros, totalPaginas) = await _libroService.GetCatalogoAsync(page, pageSize);

            return Ok(new
            {
                TotalLibros = totalLibros,
                PaginaActual = page,
                TotalPaginas = totalPaginas,
                Libros = libros
            });
        }

        [HttpGet("categoria/{categoriaId}")]
        public async Task<ActionResult<IEnumerable<GetLibroDTO>>> GetLibrosPorCategoria(int categoriaId)
        {
            var libros = await _libroService.GetLibrosPorCategoriaAsync(categoriaId);
            if (libros == null || !libros.Any()) return NotFound("No hay libros en esta categoría.");
            return Ok(libros);
        }


    }
}
