using hoshibunko.Models.Entities;
using hoshibunko.Models.DTOs.Libro;
using hoshibunko.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Hoshibunko.Context;

namespace hoshibunko.Services.Implementations
{
    public class LibroService : ILibroService
    {
        private readonly ApplicationDbContext _context;

        public LibroService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetLibroDTO>> GetAllAsync()
        {
            var libros = await _context.Libros
     .Include(l => l.LibrosCategorias)
         .ThenInclude(lc => lc.Categoria)
     .Include(l => l.Autor)
     .Include(l => l.Imagenes) // ✅ Incluir imágenes
     .ToListAsync(); // ✅ Traer los datos a memoria antes de aplicar FirstOrDefault()

            return libros.Select(l => new GetLibroDTO
            {
                Id = l.Id,
                Nombre = l.Nombre,
                Descripcion = l.Descripcion,
                AutorId = l.AutorId,
                AutorNombre = l.Autor.Nombre,
                CategoriaIds = l.LibrosCategorias.Select(lc => lc.CategoriaId).ToList(),
                Categorias = l.LibrosCategorias.Select(lc => lc.Categoria.Nombre).ToList(),
                UrlImagen = l.Imagenes.FirstOrDefault()?.UrlImagen // ✅ Ahora se puede usar FirstOrDefault()
            }).ToList();
        }

        public async Task<GetLibroDTO> GetByIdAsync(int id)
        {
            var libro = await _context.Libros
                .Include(l => l.LibrosCategorias)
                .ThenInclude(lc => lc.Categoria)
                .Include(l => l.Autor)
                .Include(l => l.Imagenes)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (libro == null) return null;

            return new GetLibroDTO
            {
                Id = libro.Id,
                Nombre = libro.Nombre,
                Descripcion = libro.Descripcion,
                AutorId = libro.AutorId,
                AutorNombre = libro.Autor.Nombre,
                CategoriaIds = libro.LibrosCategorias.Select(lc => lc.CategoriaId).ToList(),
                Categorias = libro.LibrosCategorias.Select(lc => lc.Categoria.Nombre).ToList(),
                UrlImagen = libro.Imagenes.FirstOrDefault()?.UrlImagen
            };
        }

        public async Task<int> CreateAsync(CreateLibroDTO libroDto)
        {
            var autor = await _context.Autores.FindAsync(libroDto.AutorId);
            if (autor == null) return 0; // 0 indica error (o podrías retornar -1)

            var libro = new Libro
            {
                Nombre = libroDto.Nombre,
                Descripcion = libroDto.Descripcion,
                AutorId = libroDto.AutorId,
                Autor = autor
            };

            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();

            // Manejo de categorías...
            if (libroDto.CategoriaIds != null && libroDto.CategoriaIds.Any())
            {
                var categoriasValidas = await _context.Categorias
                    .Where(c => libroDto.CategoriaIds.Contains(c.Id))
                    .ToListAsync();

                foreach (var categoria in categoriasValidas)
                {
                    _context.LibrosCategorias.Add(new LibroCategoria
                    {
                        LibroId = libro.Id,
                        CategoriaId = categoria.Id,
                        Libro = libro,
                        Categoria = categoria
                    });
                }
                await _context.SaveChangesAsync();
            }

            // Retornamos el ID generado
            return libro.Id;
        }


        public async Task<bool> UpdateAsync(int id, CreateLibroDTO libroDto)
        {
            var libro = await _context.Libros.Include(l => l.LibrosCategorias).FirstOrDefaultAsync(l => l.Id == id);
            if (libro == null) return false;

            libro.Nombre = libroDto.Nombre;
            libro.Descripcion = libroDto.Descripcion;
            libro.AutorId = libroDto.AutorId;

            _context.LibrosCategorias.RemoveRange(libro.LibrosCategorias);
            await _context.SaveChangesAsync();

            if (libroDto.CategoriaIds != null && libroDto.CategoriaIds.Any())
            {
                var categoriasValidas = await _context.Categorias
                    .Where(c => libroDto.CategoriaIds.Contains(c.Id))
                    .ToListAsync();

                foreach (var categoria in categoriasValidas)
                {
                    _context.LibrosCategorias.Add(new LibroCategoria
                    {
                        LibroId = libro.Id,
                        CategoriaId = categoria.Id,
                        Libro = libro,
                        Categoria = categoria
                    });
                }
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null) return false;

            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(IEnumerable<GetLibroDTO> Libros, int TotalLibros, int TotalPaginas)> GetCatalogoAsync(int page, int pageSize)
        {
            var query = _context.Libros
                .Include(l => l.Autor)
                .Include(l => l.LibrosCategorias)
                    .ThenInclude(lc => lc.Categoria)
                .AsQueryable();

            int totalLibros = await query.CountAsync();
            int totalPaginas = (int)System.Math.Ceiling((double)totalLibros / pageSize);

            var libros = await query
                .OrderBy(l => l.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new GetLibroDTO
                {
                    Id = l.Id,
                    Nombre = l.Nombre,
                    Descripcion = l.Descripcion,
                    AutorId = l.AutorId,
                    AutorNombre = l.Autor.Nombre,
                    CategoriaIds = l.LibrosCategorias.Select(lc => lc.CategoriaId).ToList(),
                    Categorias = l.LibrosCategorias.Select(lc => lc.Categoria.Nombre).ToList()
                })
                .ToListAsync();

            return (libros, totalLibros, totalPaginas);
        }

        public async Task<IEnumerable<GetLibroDTO>> GetLibrosPorCategoriaAsync(int categoriaId)
        {
            var libros = await _context.Libros
                .Include(l => l.LibrosCategorias)
                    .ThenInclude(lc => lc.Categoria)
                .Include(l => l.Autor)
                .Include(l => l.Imagenes)
                .Where(l => l.LibrosCategorias.Any(lc => lc.CategoriaId == categoriaId)) // ✅ Filtra libros por categoría
                .ToListAsync();

            return libros.Select(l => new GetLibroDTO
            {
                Id = l.Id,
                Nombre = l.Nombre,
                Descripcion = l.Descripcion,
                AutorId = l.AutorId,
                AutorNombre = l.Autor.Nombre,
                CategoriaIds = l.LibrosCategorias.Select(lc => lc.CategoriaId).ToList(),
                Categorias = l.LibrosCategorias.Select(lc => lc.Categoria.Nombre).ToList(),
                UrlImagen = l.Imagenes.FirstOrDefault()?.UrlImagen
            }).ToList();
        }

    }
}


