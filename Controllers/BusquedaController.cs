using hoshibunko.Models.DTOs.Libro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hoshibunko.Context;

namespace hoshibunko.Controllers
{
    [ApiController]
    [Route("api/busqueda")]
    public class BusquedaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BusquedaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetLibroDTO>>> BuscarLibros(
            [FromQuery] string? titulo,
            [FromQuery] int? autorId,
            [FromQuery] int? categoriaId)
        {
            var query = _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.LibrosCategorias)
                    .ThenInclude(lc => lc.Categoria)
                .AsQueryable();

            if (!string.IsNullOrEmpty(titulo))
                query = query.Where(l => l.Nombre.Contains(titulo));

            if (autorId.HasValue)
                query = query.Where(l => l.AutorId == autorId);

            if (categoriaId.HasValue)
                query = query.Where(l => l.LibrosCategorias.Any(c => c.CategoriaId == categoriaId));

            var libros = await query
                .Select(l => new GetLibroDTO
                {
                    Id = l.Id,
                    Nombre = l.Nombre,
                    Descripcion = l.Descripcion,
                    AutorId = l.AutorId,
                    AutorNombre = l.Autor.Nombre, // ✅ Devuelve el nombre del autor
                    CategoriaIds = l.LibrosCategorias.Select(lc => lc.CategoriaId).ToList(), // ✅ Devuelve IDs de categorías
                    Categorias = l.LibrosCategorias.Select(lc => lc.Categoria.Nombre).ToList() // ✅ Devuelve nombres de categorías
                })
                .ToListAsync();

            return Ok(libros);
        }
    }
}
