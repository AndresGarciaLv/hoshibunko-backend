using hoshibunko.Models.Entities;
using hoshibunko.Services.Interfaces;
using Hoshibunko.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace hoshibunko.Services.Implementations
{
    public class LibroImagenService : ILibroImagenService
    {
        private readonly ApplicationDbContext _context;
        private readonly List<string> formatosPermitidos = new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".avif" }; // ✅ Formatos permitidos

        public LibroImagenService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// ✅ Subir o actualizar la portada de un libro (solo 1 imagen por libro)
        /// </summary>
        public async Task<string> SubirImagenAsync(int libroId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            string extension = Path.GetExtension(file.FileName).ToLower();
            if (!formatosPermitidos.Contains(extension))
                return null; // ❌ Formato no permitido

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(uploadsFolder, fileName);
            string urlImagen = $"/images/{fileName}";

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // ✅ Verificar si el libro ya tiene una imagen, si existe la elimina y la reemplaza
            var imagenExistente = await _context.LibroImagenes.FirstOrDefaultAsync(i => i.LibroId == libroId);
            if (imagenExistente != null)
            {
                // ❌ Eliminar la imagen anterior del servidor
                string filePathAnterior = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagenExistente.UrlImagen.TrimStart('/'));
                if (File.Exists(filePathAnterior))
                    File.Delete(filePathAnterior);

                // 🔄 Actualizar la URL de la imagen en la base de datos
                imagenExistente.UrlImagen = urlImagen;
            }
            else
            {
                // 📌 Si no tenía imagen, agregar una nueva
                _context.LibroImagenes.Add(new LibroImagen { LibroId = libroId, UrlImagen = urlImagen });
            }

            await _context.SaveChangesAsync();
            return urlImagen;
        }

        /// <summary>
        /// ✅ Obtener la imagen de portada de un libro
        /// </summary>
        public async Task<string> ObtenerImagenAsync(int libroId)
        {
            var imagen = await _context.LibroImagenes.FirstOrDefaultAsync(i => i.LibroId == libroId);
            return imagen?.UrlImagen;
        }

        /// <summary>
        /// ✅ Eliminar la imagen de un libro
        /// </summary>
        public async Task<bool> EliminarImagenAsync(int libroId)
        {
            var imagen = await _context.LibroImagenes.FirstOrDefaultAsync(i => i.LibroId == libroId);
            if (imagen == null) return false;

            // ❌ Eliminar la imagen física
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagen.UrlImagen.TrimStart('/'));
            if (File.Exists(filePath))
                File.Delete(filePath);

            _context.LibroImagenes.Remove(imagen);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
