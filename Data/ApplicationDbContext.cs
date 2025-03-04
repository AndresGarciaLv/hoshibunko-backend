using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using hoshibunko.Models.Entities;

namespace Hoshibunko.Context
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>  // Usuario hereda de IdentityUser
    {
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<LibroCategoria> LibrosCategorias { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<LibroImagen> LibroImagenes { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relación Usuario - Like (uno a muchos)
            builder.Entity<Like>()
                .HasOne(l => l.Usuario)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade); // Si se borra un usuario, se eliminan sus Likes

            // Relación Libro - Like (uno a muchos)
            builder.Entity<Like>()
                .HasOne(l => l.Libro)
                .WithMany(b => b.Likes)  // Se agregó la lista de Likes en Libro para evitar errores de navegación
                .HasForeignKey(l => l.LibroId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Libro - Categoria (muchos a muchos)
            builder.Entity<LibroCategoria>()
                .HasKey(lc => new { lc.LibroId, lc.CategoriaId });

            builder.Entity<LibroCategoria>()
                .HasOne(lc => lc.Libro)
                .WithMany(l => l.LibrosCategorias)
                .HasForeignKey(lc => lc.LibroId)
                .OnDelete(DeleteBehavior.Cascade); // Si se borra un libro, se eliminan sus relaciones con categorías

            builder.Entity<LibroCategoria>()
                .HasOne(lc => lc.Categoria)
                .WithMany(c => c.LibrosCategorias)
                .HasForeignKey(lc => lc.CategoriaId)
                .OnDelete(DeleteBehavior.Cascade); // Si se borra una categoría, se eliminan sus relaciones con libros

            builder.Entity<LibroImagen>()
                .HasOne(li => li.Libro)
                .WithMany(l => l.Imagenes)
                .HasForeignKey(li => li.LibroId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
